using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWCF
{
    public partial class TestWCFClient : Form
    {
        public TestWCFClient()
        {
            InitializeComponent();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fileContent = File.ReadAllBytes(openFileDialog1.FileName);
                var start = DateTime.Now;
                Vanrise.Common.ServiceClientFactory.CreateTCPServiceClient<IFileWCFService>(txtURL.Text, (client) =>
                    {
                        client.SendFile(new WCFFileRequest { Content = fileContent });
                        MessageBox.Show(String.Format("File uploaded in '{0}'", (DateTime.Now - start)));
                    });
            }
        }
    }
}
