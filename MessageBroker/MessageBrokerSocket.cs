using Library;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MessageBroker
{
    class MessageBrokerSocket
    {
        private static Socket socket;
        private static MessageStorage2 messageStorage;
        private static ConnectionStorage connectionStorage;

        public MessageBrokerSocket()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            messageStorage = new MessageStorage2();
            connectionStorage = new ConnectionStorage();
        }
        public void Connect(string ip, int port)
        {
            // Устанавливаем удаленный хост для сокета
            IPAddress ipAddr = IPAddress.Parse(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);
            // Используя метод Bind (), связываем
            // сетевой адрес c Серверным Сокетом.
            // Все клиенты, которые будут подключаться к
            // Серверному Сокету должны знать этот сетевой
            // адрес.
            socket.Bind(localEndPoint);

            Console.WriteLine("Message Broker подключен...");
            // Используя метод Listen (), создаем
            // список клиентов, которые смогут
            // подключится к Серверу.
            socket.Listen(10);
            Console.WriteLine("Ожидание соединений...");
            // начинает асинхронную операцию для принятия попытки входящего подключения
            socket.BeginAccept(AcceptCallback, null);
        }
        private static void AcceptCallback(IAsyncResult ar)
        {
            ConnectInfo info = new ConnectInfo();
            try
            {
                info.Socket = socket.EndAccept(ar);
                info.Address = info.Socket.RemoteEndPoint.ToString();
                // Начинает выполнение асинхронного приема данных с подключенного объекта Socket
                info.Socket.BeginReceive(info.Data, 0, info.Data.Length, 0,
                new AsyncCallback(ReadCallback), info);
            }
            catch
            {
                Console.WriteLine("Ошибка принятия входящих подключений. ");
            }
            finally
            {
                Console.WriteLine("Попытка принятия входящих подключений. ");
                socket.BeginAccept(AcceptCallback, null);
            }
        }
        public static void ReadCallback(IAsyncResult ar)
        {
            ConnectInfo info = ar.AsyncState as ConnectInfo;
            try
            {
                int bytesRead = info.Socket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    byte[] bytesArray = new byte[bytesRead];
                    Array.Copy(info.Data, bytesArray, bytesArray.Length);

                    messageStorage.Handle(bytesArray, info); // если от Sender'a, то в список сообщений
                    connectionStorage.Handle(bytesArray, info); // если от Receiver'a, то в список подкл.
                }
            }
            catch
            {
                Console.WriteLine("Ошибка: невозможно прочитать пакет.");
            }
            finally
            {
                try
                {
                    info.Socket.BeginReceive(info.Data, 0, info.Data.Length, 0,
                                new AsyncCallback(ReadCallback), info);
                }
                catch
                {
                    // если Sender/Reciever были отключены, закрыть сокет, исключить из списка соединений
                    var address = info.Socket.RemoteEndPoint.ToString();
                    info.Socket.Close();
                    connectionStorage.Remove(address);
                    Console.WriteLine("Соединение прервано!");
                }
            }

        }
        // Отправка сообщений от отправителя получателю
        public void Send()
        {
            while (true)
            {
                while (!messageStorage.IsEmpty())
                {
                    var msg = messageStorage.Get(); // вытащить последнее сообщение из списка
                    if (msg != null)
                    {
                        var cons = connectionStorage.GetRcByTopic(msg.Topic); // получить всех Receiver'ов,
                                                                              // подписанных на топик сообщения
                        if (cons.Count > 0)
                        {
                            foreach (var con in cons)
                            {
                                var dataString = JsonConvert.SerializeObject(msg);
                                byte[] bytesArray = Encoding.UTF8.GetBytes(dataString);
                                con.Socket.Send(bytesArray); // отправить сообщение
                                Thread.Sleep(3000); // подождать 3 сек.
                            }
                            messageStorage.Remove(msg); // удалить сообщение из списка
                        }
                    }
                }
            }
        }
    }
}
