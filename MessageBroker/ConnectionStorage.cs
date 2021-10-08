using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBroker
{
    public class ConnectionStorage
    {
        private List<ConnectInfo> infos;
        private object locker;
        public ConnectionStorage()
        {
            infos = new List<ConnectInfo>();
            locker = new object();
        }
        public void Add(ConnectInfo info)
        {
            lock (locker)
            {
                infos.Add(info);
            }
        }
        public void Remove(string address)
        {
            lock (locker)
            {
                infos.RemoveAll(x => x.Address == address);
            }
        }
        public List<ConnectInfo> GetRcByTopic(string topic)
        {
            lock (locker)
            {
                return infos.Where(x => x.Topic == topic).ToList();
            }
        }
        public List<ConnectInfo> Get()
        {
            lock (locker)
            {
                return infos.ToList();
            }
        }
        public void Handle(byte[] bytes, ConnectInfo connectInfo)
        {
            string handler = Encoding.ASCII.GetString(bytes);
            if (handler.StartsWith("player1"))
            {
                connectInfo.Topic = handler.Split(new string[] { "player1" },
                    StringSplitOptions.None).LastOrDefault();
                lock (locker)
                {
                    infos.Add(connectInfo);
                } // добавить в список отправителя
                Console.WriteLine("Reciever добавлен в очередь.");
            }
        }
    }
}
