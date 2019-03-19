using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ServerTCPListener
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static TcpListener s_tcpListener;
        private void btnStart_Click(object sender, EventArgs e)
        {
            s_tcpListener = new TcpListener(IPAddress.Any, int.Parse(txtPortNumber.Text));
            s_tcpListener.Start();
            OpenThreadToListenToTCPRequests();
            btnStart.Enabled = false;
        }

        private void OpenThreadToListenToTCPRequests()
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                using (TcpClient tcpClient = s_tcpListener.AcceptTcpClient())
                {
                    OpenThreadToListenToTCPRequests();
                    using (NetworkStream s = tcpClient.GetStream())
                    {
                        using (StreamReader sr = new StreamReader(s))
                        {

                            string receivedValue = sr.ReadToEnd();
                            using (StreamWriter sw = new StreamWriter(txtOutputPath.Text, true))
                            {
                                sw.WriteLine(receivedValue);
                                sw.Close();
                            }
                            sr.Close();
                        }
                        s.Close();
                    }
                }
            }));
            t.Start();
        }
    }
}
