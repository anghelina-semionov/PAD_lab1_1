using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class ConnectInfo
    {
        public byte[] Data { get; set; }
        public Socket Socket { get; set; }
        public string Address { get; set; }
        public string Topic { get; set; }
        public ConnectInfo()
        {
            Data = new byte[1024];
        }
    }
}
