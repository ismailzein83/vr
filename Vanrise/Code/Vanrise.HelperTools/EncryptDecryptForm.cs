using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace Vanrise.HelperTools
{
    public partial class EncryptDecryptForm : Form
    {
        public EncryptDecryptForm()
        {
            InitializeComponent();
        }

        private void btnEncryptCode_Click(object sender, EventArgs e)
        {
            txtEncyptedCode.Text = Vanrise.Common.Cryptography.Encrypt(txtDecryptedCode.Text, System.Configuration.ConfigurationSettings.AppSettings["EncryptionSecretKey"]);
        }

        private void btnDecryptCode_Click(object sender, EventArgs e)
        {
            txtDecryptedCode.Text = Vanrise.Common.Cryptography.Decrypt(txtEncyptedCode.Text, System.Configuration.ConfigurationSettings.AppSettings["EncryptionSecretKey"]);
        }

    }
}
