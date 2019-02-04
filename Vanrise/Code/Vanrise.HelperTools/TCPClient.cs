using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vanrise.HelperTools
{
    public partial class TCPClient : Form
    {
        static TcpListener s_tcpListener;

        public TCPClient()
        {
            InitializeComponent();

        }

        private void btn_start_Click(object sender, EventArgs e)
        {

            s_tcpListener = new TcpListener(IPAddress.Any, int.Parse(portNumber_txt.Text));
            s_tcpListener.Start();

            Listen();
        }

        void Listen()  // Listen to incoming connections.
        {
            while (true)
            {
                TcpClient tcpClient = s_tcpListener.AcceptTcpClient();  // Accept incoming connection.
                Client client = new Client(tcpClient);     // Handle in another thread.
            }
        }
    }
    public class Client
    {
        static X509Certificate2 cert = new X509Certificate2("server.pfx", "instant");
        static List<string> messages = new List<string>() { "return: 0", "RETCODE:0", "RETCODE:0" };
        static int index = 0;

        public TcpClient tcpClient;
        public NetworkStream netStream;  // Raw-data stream of connection.
        public SslStream ssl;            // Encrypts connection using SSL.
        public BinaryReader br;
        public BinaryWriter bw;

        public Client(TcpClient c)
        {
            tcpClient = c;

            // Handle client in another thread.
            (new Thread(new ThreadStart(SetupConn))).Start();
        }

        void SetupConn()  // Setup connection and login or register.
        {
            try
            {
                using (NetworkStream s = tcpClient.GetStream())
                {
                    SslStream ssl = new SslStream(s, false);
                    ssl.AuthenticateAsServer(cert, false, SslProtocols.Tls, true);

                    BinaryReader binaryReader = new BinaryReader(ssl);
                    BinaryWriter binaryWriter = new BinaryWriter(ssl);

                    while (tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected)
                    {
                        string receivedValue = binaryReader.ReadString();
                        binaryWriter.Write(messages[index++]);
                        binaryWriter.Flush();
                    }

                    binaryWriter.Close();
                    binaryReader.Close();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
