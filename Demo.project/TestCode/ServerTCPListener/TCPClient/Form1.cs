using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace TCPClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(txtServer.Text, int.Parse(txtPortNumber.Text));
                using (var stream = tcpClient.GetStream())
                {
                    using (var streamWriter = new StreamWriter(stream))
                    {
                        streamWriter.AutoFlush = true;
                        streamWriter.Write(txtMsg.Text);
                        streamWriter.Close();
                    }

                    stream.Close();
                }
                tcpClient.Close();
            }
        }
    }
}
