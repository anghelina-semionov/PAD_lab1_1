using Library;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Sender
{
    public partial class Sender : Form
    {
        private ConnectParam con = new ConnectParam();
        private SenderSocket senderSocket;
        public Sender()
        {
            InitializeComponent();
        }

        private void Sender_Load(object sender, EventArgs e)
        {
            lblError.Visible = false;
            btnSend.Enabled = false;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (senderSocket.IsConnected)
            {
                TransData data = new TransData();
                string dataString;
                if (txtMeal.Text.Length > 0) // топик = прием пищи
                    data.Topic = txtMeal.Text.ToLower();
                if (txtDish.Text.Length > 0) // сообщение = названия блюд
                    data.Dish = txtDish.Text.ToLower();
                if (data.Topic.Length > 0 && data.Dish.Length > 0)
                {
                    dataString = JsonConvert.SerializeObject(data); // Сериализация указанного объекта в строку JSON
                    byte[] messageSent = Encoding.ASCII.GetBytes(dataString);
                    senderSocket.Send(messageSent);
                }
                else
                {
                    lblError.Visible = true;
                    Thread.Sleep(2000);
                    lblError.Visible = false;
                }
            }
            else
            {
                lblError.Text = "Невозможно подключиться!";
                lblError.Visible = true;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            senderSocket = new SenderSocket();
            senderSocket.Connect(con.IP, con.PORT);
            btnStart.Enabled = false;
            btnSend.Enabled = true;
        }
    }
}
