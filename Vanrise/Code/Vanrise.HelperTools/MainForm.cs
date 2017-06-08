using Dean.Edwards;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vanrise.HelperTools
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenForm(new GenerateProtoBufTypeMetaForm());
        }

        private void OpenForm(Form form)
        {
            form.Show();
            form.FormClosed += (sender, e) => this.Close();
            this.Hide();
        }

        private void EncryptDecrypt_Click(object sender, EventArgs e)
        {
            OpenForm(new EncryptDecryptForm());
        }

        private void bcpCommand_Click(object sender, EventArgs e)
        {
            OpenForm(new TraceBCPCommand());
        }

        private void CompressJS_Click(object sender, EventArgs e)
        {
            OpenForm(new Packer());
        }
    }
}
