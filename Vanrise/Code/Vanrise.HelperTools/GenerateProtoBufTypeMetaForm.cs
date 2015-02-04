using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vanrise.HelperTools
{
    public partial class GenerateProtoBufTypeMetaForm : Form
    {
        public GenerateProtoBufTypeMetaForm()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            this.FormClosed += GenerateProtoBufTypeMetaForm_FormClosed;
        }

        void GenerateProtoBufTypeMetaForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string dllDirectory = lblDLL.Text.Substring(0, lblDLL.Text.LastIndexOf('\\'));
            string dllPath = Path.Combine(dllDirectory, args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll");
            return System.Reflection.Assembly.LoadFile(dllPath);
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                Type selectedType = cmbTypes.SelectedItem as Type;
                if (selectedType == null)
                {
                    MessageBox.Show("Please select Type!");
                    return;
                }
                List<string> newProperties = new List<string>();
                foreach (var p in selectedType.GetProperties())
                {
                    newProperties.Add(p.Name);
                }

                List<string> existingProperties = new List<string>();
                foreach(var p in txtCurrentMeta.Text.Split(','))
                {
                    if (p.Length == 0)
                        continue;
                    int startingIndex = p.IndexOf('\"') + 1;
                    int length = p.LastIndexOf('\"') - startingIndex;
                    if (startingIndex < 0 || length <= 0)
                        continue;
                    existingProperties.Add(p.Substring(startingIndex, length));
                }

                List<string> finalProperties = new List<string>();
                foreach(var p in existingProperties)
                {
                    if (newProperties.Contains(p))
                        finalProperties.Add(p);
                }
                foreach(var p in newProperties)
                {
                    if (!finalProperties.Contains(p))
                        finalProperties.Add(p);
                }

                StringBuilder builder = new StringBuilder("");
                int addedNumber = 0;
                foreach (var p in finalProperties)
                {
                    if (builder.Length > 0)
                        builder.Append(", "); addedNumber++;
                    if (addedNumber == 7)
                    {
                        builder.AppendLine();
                        addedNumber = 0;
                    }
                    builder.Append("\"" + p + "\"");
                }
               
                txtNewMeta.Text = builder.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSelectDLL_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                cmbTypes.SelectedItem = null;
                cmbTypes.Items.Clear();
                txtCurrentMeta.Text = "";
                txtNewMeta.Text = "";
                chkNoCurrentMeta.Checked = false;
                lblDLL.Text = openFileDialog1.FileName;
                try
                {
                    GetTypesFromDLL(lblDLL.Text);
                }
                catch(Exception ex)
                {
                    lblDLL.Text = "";
                    MessageBox.Show(ex.Message);
                }
                SetGenerateButtonAccessibility();
            }
        }

        private void GetTypesFromDLL(string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            
            foreach(var t in assembly.GetTypes())
            {
                if(t.IsClass)
                {
                    cmbTypes.Items.Add(t);
                }
            }
            
        }

        private void chkNoCurrentMeta_CheckedChanged(object sender, EventArgs e)
        {
            SetGenerateButtonAccessibility();
        }

        private void txtCurrentMeta_TextChanged(object sender, EventArgs e)
        {
            SetGenerateButtonAccessibility();
        }

        private void SetGenerateButtonAccessibility()
        {
            btnGenerate.Enabled = !String.IsNullOrEmpty(lblDLL.Text) 
                && cmbTypes.SelectedItem != null
                && (chkNoCurrentMeta.Checked || !String.IsNullOrWhiteSpace(txtCurrentMeta.Text));
        }

        private void cmbTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetGenerateButtonAccessibility();
        }
    }
}
