using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ChatApp_2nd_year_project
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initializing and setting up socket
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Rdm, ProtocolType.IP);

            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // Get Local or user IP
            textLocal_IP.Text = GetLocalIP();

        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach( IPAddress ip in host.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }

            return "127.0.0.1";
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            // Binding socket
            //epLocal = new IPEndPoint(IPAddress.Parse(textLocal_IP.Text), Convert.ToInt32(textLocalPort.Text));
            //sck.Bind(epLocal);

            // Connecting to remote computer
            epRemote = new IPEndPoint(IPAddress.Parse(textRemoteIP.Text), Convert.ToInt32(textRemotPort.Text));
            sck.Connect(epRemote);

            // Listening to specific port
            buffer = new byte[1500];
            sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
            byte[] recivedData = new byte[1500];
            recivedData = (byte[])aResult.AsyncState;

            // Convrting data to message
            UnicodeEncoding uEncoding = new UnicodeEncoding();
            string recievedMessage = uEncoding.GetString(recivedData);

            // Adding or printing message to list
            listMessage.Items.Add("Friend:>>" + recievedMessage);

            // Again recieving data from remote computer
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
            // Converting string message to byte
            UnicodeEncoding uEncoding = new UnicodeEncoding();
            byte[] sendingMessage = new byte[1500];
            sendingMessage = uEncoding.GetBytes(textMessage.Text);

            // Sending encoded message to socket
            sck.Send(sendingMessage);

            // Adding local message to list
            listMessage.Items.Add("Me:>> " + textMessage.Text);
            textMessage.Text = "";

        }

        private void buttonStartServer_Click(object sender, EventArgs e)
        {
            // Binding socket
            epLocal = new IPEndPoint(IPAddress.Parse(textLocal_IP.Text), Convert.ToInt32(textLocalPort.Text));
            sck.Bind(epLocal);
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            formAbout aboutDialog = new formAbout();
            aboutDialog.Show();
        }
    }
}
