using Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBroker
{
    class MessageStorage2
    {
        private List<TransData> transDatas;
        private object locker;
        public MessageStorage2()
        {
            transDatas = new List<TransData>();
            locker = new object();
        }
        public void Remove(TransData data) // удалить из списка
        {
            lock (locker)
            {
                transDatas.Remove(data);
            }
        }
        public int Count() // количество смс
        {
            lock (locker)
            {
                return transDatas.Count();
            }
        }
        public TransData Get() // получить последнее смс
        {
            lock (locker)
            {
                return transDatas.Last();
            }
        }
        public bool IsEmpty() // проверить, пустой ли список
        {
            lock (locker)
            {
                if (transDatas.Count == 0)
                    return true;
                else return false;
            }
        }
        public void Handle(byte[] bytes, ConnectInfo connectInfo) // добавить сообщение в список
        {
            string handler = Encoding.ASCII.GetString(bytes);
            if (!handler.StartsWith("player1"))
            {
                TransData data = JsonConvert.DeserializeObject<TransData>(handler);
                lock (locker)
                {
                    transDatas.Add(data);// добавить в список топик/блюдо
                }
                Console.WriteLine("Сообщение Sender'а добавлено в очередь.");
            }
        }
    }
}
