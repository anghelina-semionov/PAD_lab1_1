using Library;
using System;
using System.Threading.Tasks;

namespace MessageBroker
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectParam con = new ConnectParam();
            MessageBrokerSocket socket = new MessageBrokerSocket();
            socket.Connect(con.IP, con.PORT);
            // создает и запускает задачу
            Task.Factory.StartNew(socket.Send, TaskCreationOptions.LongRunning);
            Console.ReadLine();
        }
    }
}
