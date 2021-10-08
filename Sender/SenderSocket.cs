using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Sender
{
    class SenderSocket
    {
        // Сокет - это конечная точка соединения «Кому» и «От» (двунаправленная) 
        // между двумя программами (сервер и клиент), работающими в одной сети.
        private Socket socket;
        public bool IsConnected;
        public SenderSocket()

        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Connect(string ip, int port)
        {
            // устанавливаем удаленный хост для сокета
            IPAddress ipAddr = IPAddress.Parse(ip); 
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port); 

            // подключиться к удаленной конечной точке  
            socket.BeginConnect(localEndPoint,
                new AsyncCallback(ConnectCallback), socket);
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            IsConnected = socket.Connected;
        }
        public void Send(byte[] data)
        {
            try
            {
                socket.Send(data);
            }
            catch (Exception e)
            {
                
            }
        }
    }
}
