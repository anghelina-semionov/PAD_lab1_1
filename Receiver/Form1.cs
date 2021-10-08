using Library;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Receiver
{
    public partial class Form1 : Form
    {
        private ConnectParam con = new ConnectParam();

        static Socket socket;
        public bool IsConnected;
        string d = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtDish.Text = "";
            lblError.Visible = false;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        private void btnGet_Click(object sender, EventArgs e)
        {
            if (txtMeal.Text.Length > 0)
                d = txtMeal.Text.ToLower();
            if (d.Length > 0)
            {
                Connect(con.IP, con.PORT);
            }
            else
            {
                lblError.Text = "Не введены данные!";
                lblError.Visible = true;
            }
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
            if (socket.Connected)
            {
                Send(d);
                GetResult();
            }
            else
            {
                lblError.Text = "Reciever не подключен!";
                lblError.Visible = true;
            }
        }
        public void Send(string topic)
        {
            var data = Encoding.ASCII.GetBytes("player1" + topic);
            try
            {
                socket.Send(data);
            }
            catch
            {
                lblError.Text = "Невозможно отправить данные!";
                lblError.Visible = true;
            }
        }
        private void GetResult()
        {
            ConnectInfo info = new ConnectInfo();
            info.Socket = socket;
            // подключиться к удаленной конечной точке  
            socket.BeginReceive(info.Data, 0, info.Data.Length, 0,
                ReadCallback, info);
        }
        public void ReadCallback(IAsyncResult ar)
        {
            ConnectInfo info = ar.AsyncState as ConnectInfo;
            try
            {
                SocketError response;
                int bytesRead = info.Socket.EndReceive(ar, out response);
                if (response == SocketError.Success)
                {
                    byte[] bytesArray = new byte[bytesRead];
                    Array.Copy(info.Data, bytesArray, bytesArray.Length);

                    string handler = Encoding.ASCII.GetString(bytesArray);
                    TransData data = JsonConvert.DeserializeObject<TransData>(handler);

                    if (data.Dish != null)
                    {
                        txtDish.AppendText(data.Dish);
                        txtDish.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        lblError.Text = "Проблема с сообщением от Broker'a!";
                        lblError.Visible = true;
                    }
                }
            }
            catch
            {
                lblError.Text = "Ошибка при чтении данных!";
                lblError.Visible = true;

            }
            finally
            {
                try
                {
                    socket.BeginReceive(info.Data, 0, info.Data.Length, 0,
                new AsyncCallback(ReadCallback), info);

                }
                catch
                {
                    info.Socket.Close();
                    lblError.Text = "Соединение прервано!";
                    lblError.Visible = true;
                }
            }
        }
    }
}
