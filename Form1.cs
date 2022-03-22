using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ChatApp
{
    public partial class ChatApp : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;

        public ChatApp()
        {
            InitializeComponent();
        }

        private void ChatApp_Load(object sender, EventArgs e)
        {
            //set up socket
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            //get user IP
            textLocalIP.Text = GetLocalIP();
            textRemoteIP.Text = GetLocalIP();
        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            //binding socket
            epLocal = new IPEndPoint(IPAddress.Parse(textLocalIP.Text), Convert.ToInt32(textLocalPort.Text));
            sck.Bind(epLocal);
            //Connecting to Remote IP
            epRemote = new IPEndPoint(IPAddress.Parse(textRemoteIP.Text), Convert.ToInt32(textRemotePort.Text));
            sck.Connect(epRemote);
            //Listening the specific port
            buffer = new byte[1500];
            sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

        }

        private void MessageCallBack(IAsyncResult eResult)
        {
            try
            {
                byte[] receivedData = new byte[1500];
                receivedData = (byte[])eResult.AsyncState;
                //Converting byte[] to string
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string receivedMessage = aEncoding.GetString(receivedData);

                //Adding this message into list box
                listMassage.Items.Add("Friend: " + receivedMessage);

                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            //Convert string to byte[]
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] sendingMessage = new byte[1500];
            sendingMessage = aEncoding.GetBytes(textMassage.Text);
            //Sending the encoded message
            sck.Send(sendingMessage);
            //add to the list box
            listMassage.Items.Add("Me: " + textMassage.Text);
            textMassage.Text = "";
        }
    }
}
