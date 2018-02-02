using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Vanrise.HelperTools;

namespace Dean.Edwards
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Packer : System.Windows.Forms.Form
    {
        private System.Windows.Forms.TextBox tbSource;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.Button pack;
        private System.Windows.Forms.ComboBox Encoding;
        private System.Windows.Forms.CheckBox fastDecode;
        private System.Windows.Forms.CheckBox specialChars;
        private System.Windows.Forms.LinkLabel llPaste;
        private System.Windows.Forms.LinkLabel llCopy;
        private System.Windows.Forms.Button bLoad;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bClear;
        private System.Windows.Forms.OpenFileDialog ofdSource;
        private System.Windows.Forms.SaveFileDialog sfdResult;
        private CheckBox replaceFiles;
        private Button btnBrowse;
        private TextBox txtDirectory;
        private Label label1;
        private FolderBrowserDialog folderBrowserDialog1;
        private ProgressBar pBar1;
        private Button btnGroupFiles;
        private Button btnGroupSQLFiles;
        private CheckBox chkOverriddenNode;
        private Button bnGenerateDBScript;
        private CheckedListBox chklstDatabases;
        private Button btnEnums;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public Packer()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbSource = new System.Windows.Forms.TextBox();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.pack = new System.Windows.Forms.Button();
            this.Encoding = new System.Windows.Forms.ComboBox();
            this.fastDecode = new System.Windows.Forms.CheckBox();
            this.specialChars = new System.Windows.Forms.CheckBox();
            this.llPaste = new System.Windows.Forms.LinkLabel();
            this.llCopy = new System.Windows.Forms.LinkLabel();
            this.bLoad = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.bClear = new System.Windows.Forms.Button();
            this.ofdSource = new System.Windows.Forms.OpenFileDialog();
            this.sfdResult = new System.Windows.Forms.SaveFileDialog();
            this.replaceFiles = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.pBar1 = new System.Windows.Forms.ProgressBar();
            this.btnGroupFiles = new System.Windows.Forms.Button();
            this.btnGroupSQLFiles = new System.Windows.Forms.Button();
            this.chkOverriddenNode = new System.Windows.Forms.CheckBox();
            this.bnGenerateDBScript = new System.Windows.Forms.Button();
            this.chklstDatabases = new System.Windows.Forms.CheckedListBox();
            this.btnEnums = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbSource
            // 
            this.tbSource.AcceptsReturn = true;
            this.tbSource.AcceptsTab = true;
            this.tbSource.AllowDrop = true;
            this.tbSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSource.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSource.Location = new System.Drawing.Point(8, 45);
            this.tbSource.Multiline = true;
            this.tbSource.Name = "tbSource";
            this.tbSource.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbSource.Size = new System.Drawing.Size(761, 42);
            this.tbSource.TabIndex = 0;
            this.tbSource.Visible = false;
            // 
            // tbResult
            // 
            this.tbResult.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tbResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbResult.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbResult.Location = new System.Drawing.Point(-2, 288);
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.ReadOnly = true;
            this.tbResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResult.Size = new System.Drawing.Size(778, 189);
            this.tbResult.TabIndex = 0;
            this.tbResult.Visible = false;
            // 
            // pack
            // 
            this.pack.BackColor = System.Drawing.Color.LightSkyBlue;
            this.pack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pack.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pack.Location = new System.Drawing.Point(8, 140);
            this.pack.Name = "pack";
            this.pack.Size = new System.Drawing.Size(119, 23);
            this.pack.TabIndex = 6;
            this.pack.Text = "Compress JS Files";
            this.pack.UseVisualStyleBackColor = false;
            this.pack.Click += new System.EventHandler(this.pack_Click);
            // 
            // Encoding
            // 
            this.Encoding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Encoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Encoding.Location = new System.Drawing.Point(648, 211);
            this.Encoding.Name = "Encoding";
            this.Encoding.Size = new System.Drawing.Size(121, 21);
            this.Encoding.TabIndex = 5;
            this.Encoding.Visible = false;
            this.Encoding.SelectedIndexChanged += new System.EventHandler(this.Encoding_SelectedIndexChanged);
            // 
            // fastDecode
            // 
            this.fastDecode.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.fastDecode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fastDecode.Location = new System.Drawing.Point(553, 213);
            this.fastDecode.Name = "fastDecode";
            this.fastDecode.Size = new System.Drawing.Size(92, 20);
            this.fastDecode.TabIndex = 4;
            this.fastDecode.Text = "Fast Decode";
            this.fastDecode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.fastDecode.Visible = false;
            // 
            // specialChars
            // 
            this.specialChars.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.specialChars.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.specialChars.Location = new System.Drawing.Point(422, 213);
            this.specialChars.Name = "specialChars";
            this.specialChars.Size = new System.Drawing.Size(116, 19);
            this.specialChars.TabIndex = 3;
            this.specialChars.Text = "Special Characters";
            this.specialChars.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.specialChars.Visible = false;
            // 
            // llPaste
            // 
            this.llPaste.Location = new System.Drawing.Point(5, 240);
            this.llPaste.Name = "llPaste";
            this.llPaste.Size = new System.Drawing.Size(56, 16);
            this.llPaste.TabIndex = 4;
            this.llPaste.TabStop = true;
            this.llPaste.Text = "Paste:";
            this.llPaste.Visible = false;
            this.llPaste.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llPaste_LinkClicked);
            // 
            // llCopy
            // 
            this.llCopy.Location = new System.Drawing.Point(8, 256);
            this.llCopy.Name = "llCopy";
            this.llCopy.Size = new System.Drawing.Size(56, 16);
            this.llCopy.TabIndex = 4;
            this.llCopy.TabStop = true;
            this.llCopy.Text = "Copy:";
            this.llCopy.Visible = false;
            this.llCopy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llCopy_LinkClicked);
            // 
            // bLoad
            // 
            this.bLoad.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.bLoad.BackColor = System.Drawing.Color.LightSkyBlue;
            this.bLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bLoad.Location = new System.Drawing.Point(158, 527);
            this.bLoad.Name = "bLoad";
            this.bLoad.Size = new System.Drawing.Size(75, 23);
            this.bLoad.TabIndex = 1;
            this.bLoad.Text = "&Load";
            this.bLoad.UseVisualStyleBackColor = false;
            this.bLoad.Visible = false;
            this.bLoad.Click += new System.EventHandler(this.bLoad_Click);
            // 
            // bSave
            // 
            this.bSave.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.bSave.BackColor = System.Drawing.Color.LightSkyBlue;
            this.bSave.Enabled = false;
            this.bSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSave.Location = new System.Drawing.Point(158, 498);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 1;
            this.bSave.Text = "&Save";
            this.bSave.UseVisualStyleBackColor = false;
            this.bSave.Visible = false;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bClear
            // 
            this.bClear.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.bClear.BackColor = System.Drawing.Color.LightSkyBlue;
            this.bClear.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bClear.Location = new System.Drawing.Point(238, 498);
            this.bClear.Name = "bClear";
            this.bClear.Size = new System.Drawing.Size(75, 23);
            this.bClear.TabIndex = 1;
            this.bClear.Text = "&Clear";
            this.bClear.UseVisualStyleBackColor = false;
            this.bClear.Visible = false;
            this.bClear.Click += new System.EventHandler(this.bClear_Click);
            // 
            // ofdSource
            // 
            this.ofdSource.DefaultExt = "js";
            this.ofdSource.Filter = "ECMAScript Files|*.js|All files|*.*";
            this.ofdSource.Title = "Choose a file";
            // 
            // sfdResult
            // 
            this.sfdResult.DefaultExt = "js";
            this.sfdResult.Filter = "ECMAScript Files|*.js|All files|*.*";
            // 
            // replaceFiles
            // 
            this.replaceFiles.AutoSize = true;
            this.replaceFiles.Checked = true;
            this.replaceFiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.replaceFiles.Location = new System.Drawing.Point(679, 146);
            this.replaceFiles.Name = "replaceFiles";
            this.replaceFiles.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.replaceFiles.Size = new System.Drawing.Size(90, 17);
            this.replaceFiles.TabIndex = 2;
            this.replaceFiles.Text = "Replace Files";
            this.replaceFiles.UseVisualStyleBackColor = true;
            // 
            // btnBrowse
            // 
            this.btnBrowse.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnBrowse.Location = new System.Drawing.Point(687, 12);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(82, 21);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtDirectory
            // 
            this.txtDirectory.Location = new System.Drawing.Point(131, 13);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.Size = new System.Drawing.Size(554, 20);
            this.txtDirectory.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "TargetDirectory:";
            // 
            // pBar1
            // 
            this.pBar1.Location = new System.Drawing.Point(8, 176);
            this.pBar1.Name = "pBar1";
            this.pBar1.Size = new System.Drawing.Size(762, 23);
            this.pBar1.TabIndex = 9;
            this.pBar1.Tag = "Progress";
            // 
            // btnGroupFiles
            // 
            this.btnGroupFiles.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnGroupFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGroupFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGroupFiles.Location = new System.Drawing.Point(133, 140);
            this.btnGroupFiles.Name = "btnGroupFiles";
            this.btnGroupFiles.Size = new System.Drawing.Size(99, 23);
            this.btnGroupFiles.TabIndex = 10;
            this.btnGroupFiles.Text = "Group JS Files";
            this.btnGroupFiles.UseVisualStyleBackColor = false;
            this.btnGroupFiles.Click += new System.EventHandler(this.btnGroupFiles_Click);
            // 
            // btnGroupSQLFiles
            // 
            this.btnGroupSQLFiles.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnGroupSQLFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGroupSQLFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGroupSQLFiles.Location = new System.Drawing.Point(238, 140);
            this.btnGroupSQLFiles.Name = "btnGroupSQLFiles";
            this.btnGroupSQLFiles.Size = new System.Drawing.Size(109, 23);
            this.btnGroupSQLFiles.TabIndex = 11;
            this.btnGroupSQLFiles.Text = "Group SQL Files";
            this.btnGroupSQLFiles.UseVisualStyleBackColor = false;
            this.btnGroupSQLFiles.Click += new System.EventHandler(this.btnGroupSQLFiles_Click);
            // 
            // chkOverriddenNode
            // 
            this.chkOverriddenNode.AutoSize = true;
            this.chkOverriddenNode.Location = new System.Drawing.Point(532, 146);
            this.chkOverriddenNode.Name = "chkOverriddenNode";
            this.chkOverriddenNode.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkOverriddenNode.Size = new System.Drawing.Size(129, 17);
            this.chkOverriddenNode.TabIndex = 12;
            this.chkOverriddenNode.Text = "Override Group Name";
            this.chkOverriddenNode.UseVisualStyleBackColor = true;
            // 
            // bnGenerateDBScript
            // 
            this.bnGenerateDBScript.BackColor = System.Drawing.Color.LightSkyBlue;
            this.bnGenerateDBScript.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bnGenerateDBScript.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bnGenerateDBScript.Location = new System.Drawing.Point(353, 140);
            this.bnGenerateDBScript.Name = "bnGenerateDBScript";
            this.bnGenerateDBScript.Size = new System.Drawing.Size(90, 23);
            this.bnGenerateDBScript.TabIndex = 14;
            this.bnGenerateDBScript.Text = "DB Structure";
            this.bnGenerateDBScript.UseVisualStyleBackColor = false;
            this.bnGenerateDBScript.Click += new System.EventHandler(this.bnGenerateDBScript_Click);
            // 
            // chklstDatabases
            // 
            this.chklstDatabases.FormattingEnabled = true;
            this.chklstDatabases.Location = new System.Drawing.Point(408, 45);
            this.chklstDatabases.Name = "chklstDatabases";
            this.chklstDatabases.Size = new System.Drawing.Size(360, 94);
            this.chklstDatabases.TabIndex = 16;
            // 
            // btnEnums
            // 
            this.btnEnums.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnEnums.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnums.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnums.Location = new System.Drawing.Point(448, 140);
            this.btnEnums.Name = "btnEnums";
            this.btnEnums.Size = new System.Drawing.Size(90, 23);
            this.btnEnums.TabIndex = 17;
            this.btnEnums.Text = "Enums";
            this.btnEnums.UseVisualStyleBackColor = false;
            this.btnEnums.Click += new System.EventHandler(this.btnEnums_Click);
            // 
            // Packer
            // 
            this.AcceptButton = this.pack;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.bClear;
            this.ClientSize = new System.Drawing.Size(774, 253);
            this.Controls.Add(this.btnEnums);
            this.Controls.Add(this.chklstDatabases);
            this.Controls.Add(this.bnGenerateDBScript);
            this.Controls.Add(this.chkOverriddenNode);
            this.Controls.Add(this.btnGroupSQLFiles);
            this.Controls.Add(this.btnGroupFiles);
            this.Controls.Add(this.pBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDirectory);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.replaceFiles);
            this.Controls.Add(this.llPaste);
            this.Controls.Add(this.fastDecode);
            this.Controls.Add(this.Encoding);
            this.Controls.Add(this.pack);
            this.Controls.Add(this.tbSource);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.specialChars);
            this.Controls.Add(this.llCopy);
            this.Controls.Add(this.bLoad);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.bClear);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Packer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Compress JavaScript files / Group to Top Directory / Create Database Structure Sc" +
    "ript";
            this.Load += new System.EventHandler(this.Packer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        
        private void pack_Click(object sender, System.EventArgs e)
        {
            string jsFilesPath = txtDirectory.Text;
            if (!string.IsNullOrEmpty(jsFilesPath))
            {
                DisplayProgressBar(2);
                Thread.Sleep(1000);
                pBar1.PerformStep();
                Common.CompressJSFiles("currentDateShort", "Javascripts", jsFilesPath,"");
                pBar1.PerformStep();
            }
            else
            {
                MessageBox.Show("Please select a directory!");
            }
        }

        private void CompressFile(string file, string ExtraFilename, ECMAScriptPacker p)
        {
            tbSource.Text = string.Format("Current: {0}\r\n", Path.GetFullPath(file));
            tbSource.Text += string.Format("New: {0}\\{1}{2}{3}\r\n", Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file), ExtraFilename, Path.GetExtension(file));

            File.WriteAllText(string.Format("{0}\\{1}{2}{3}", Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file), ExtraFilename, Path.GetExtension(file)), p.Pack(File.ReadAllText(Path.GetFullPath(file))));

            pBar1.PerformStep();
        }

        private void Packer_Load(object sender, System.EventArgs e)
        {
            Encoding.Items.Add(ECMAScriptPacker.PackerEncoding.None);
            Encoding.Items.Add(ECMAScriptPacker.PackerEncoding.Numeric);
            Encoding.Items.Add(ECMAScriptPacker.PackerEncoding.Mid);
            Encoding.Items.Add(ECMAScriptPacker.PackerEncoding.Normal);
            Encoding.Items.Add(ECMAScriptPacker.PackerEncoding.HighAscii);
            Encoding.SelectedItem = ECMAScriptPacker.PackerEncoding.None;

            //load DBs
            if (string.IsNullOrEmpty(Common.ActiveDBs))
            {
                Server myServer = new Server(Common.ServerIP);
                foreach (Database db in myServer.Databases)
                {
                    if (db.Name.StartsWith("Standard"))
                    {
                        chklstDatabases.Items.Add(db.Name);
                    }
                }
            }
            else
            {
                chklstDatabases.Items.AddRange(Common.ActiveDBs.Split('#'));
            }
        }

        private void Encoding_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            fastDecode.Enabled = ((ECMAScriptPacker.PackerEncoding)Encoding.SelectedItem != ECMAScriptPacker.PackerEncoding.None);
        }

        private void llPaste_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            tbSource.Text = (string)Clipboard.GetDataObject().GetData(typeof(string));
        }

        private void llCopy_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetDataObject(tbResult.Text, true);
        }

        private void bClear_Click(object sender, System.EventArgs e)
        {
            tbResult.Text = tbSource.Text = string.Empty;
            bSave.Enabled = false;
        }

        private void bLoad_Click(object sender, System.EventArgs e)
        {
            DialogResult r = ofdSource.ShowDialog(this);
            if (r == DialogResult.OK)
            {
                Stream s = ofdSource.OpenFile();
                TextReader rd = new StreamReader(s);
                string content = rd.ReadToEnd();
                rd.Close();
                s.Close();
                Regex regex = new Regex("([^\r])(\n+)");
                tbSource.Text = regex.Replace(content, new MatchEvaluator(changeUnixLineEndings));
            }
        }

        private string changeUnixLineEndings(Match match)
        {
            return match.Value.Replace("\n", "\r\n");
        }

        private void bSave_Click(object sender, System.EventArgs e)
        {
            DialogResult r = sfdResult.ShowDialog(this);
            if (r == DialogResult.OK)
            {
                Stream s = sfdResult.OpenFile();
                TextWriter rd = new StreamWriter(s);
                rd.Write(tbResult.Text);
                rd.Close();
                s.Close();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = txtDirectory.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDirectory.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnGroupFiles_Click(object sender, EventArgs e)
        {
            string jsFilesPath = txtDirectory.Text;
            if (!string.IsNullOrEmpty(jsFilesPath))
            {
                DisplayProgressBar(2);
                Thread.Sleep(1000);
                pBar1.PerformStep();
                Common.GroupJSFiles("currentDateShort", "Javascripts", false, jsFilesPath,"");
                pBar1.PerformStep();
            }
            else
            {
                MessageBox.Show("Please select a directory!");
            }
        }

        private void btnGroupSQLFiles_Click(object sender, EventArgs e)
        {
            string jsFilesPath = txtDirectory.Text;
            if (!string.IsNullOrEmpty(jsFilesPath))
            {
                DisplayProgressBar(2);
                Thread.Sleep(1000);
                pBar1.PerformStep();
                Common.GroupSQLPostScriptFiles("currentDateShort", chkOverriddenNode.Checked, jsFilesPath,"");
                pBar1.PerformStep();
            }
            else
            {
                MessageBox.Show("Please select a directory!");
            }
        }

        private void bnGenerateDBScript_Click(object sender, EventArgs e)
        {
            string sqlFilesOutputPath = txtDirectory.Text;

            if (string.IsNullOrEmpty(sqlFilesOutputPath))
            {
                MessageBox.Show("Please select Output directory!");
                txtDirectory.Focus();
                return;
            }

            if (chklstDatabases.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select Database!");
                chklstDatabases.Focus();
                return;
            }

            string currentDate = DateTime.Now.ToString("yyyMMddHHmm");
            string currentDateShort = DateTime.Now.ToString("yyyMMdd");

            List<string> lstDBs = new List<string>();
            for (int i = 0; i < chklstDatabases.Items.Count; i++)
            {
                if (chklstDatabases.GetItemCheckState(i) == CheckState.Checked)
                {
                    lstDBs.Add(chklstDatabases.Items[i].ToString());
                }
            }

            DisplayProgressBar(2);
            Thread.Sleep(1000);
            pBar1.PerformStep();
            Common.GenerateDBStructure(currentDate, currentDateShort, lstDBs, sqlFilesOutputPath,"");
            pBar1.PerformStep();
        }

        private void DisplayProgressBar(int maximum)
        {
            // Display the ProgressBar control.
            pBar1.Visible = true;
            // Set Minimum to 1 to represent the first file being copied.
            pBar1.Minimum = 1;
            // Set Maximum to the total number of files to copy.
            pBar1.Maximum = maximum;
            // Set the initial value of the ProgressBar.
            pBar1.Value = 1;
            // Set the Step property to a value of 1 to represent each file being copied.
            pBar1.Step = 1;
        }
        private void GenerateSQLDBScript(string item, string sqlFilesOutputPath, string currentDate, string currentDateShort)
        {
            var sb = new StringBuilder();

            Server myServer = new Server(Common.ServerIP);
            Scripter scripter = new Scripter(myServer);
            Database dbname = myServer.Databases[item];

            StringCollection transferScript = GetTransferScript(dbname);

            ScriptingOptions scriptOptions = new ScriptingOptions();
            scriptOptions.ScriptDrops = true;
            scriptOptions.WithDependencies = false;
            scriptOptions.IncludeHeaders = true;
            scriptOptions.IncludeIfNotExists = true;
            scriptOptions.Indexes = true;
            scriptOptions.NoIdentities = true;
            scriptOptions.NonClusteredIndexes = true;
            scriptOptions.SchemaQualify = true;
            scriptOptions.AllowSystemObjects = true;

            foreach (string script in dbname.Script(scriptOptions))
            {
                sb.AppendLine(script);
                sb.AppendLine("GO");
            }

            Urn[] DatabaseURNs = new Urn[] { dbname.Urn };
            StringCollection scriptCollection = scripter.Script(DatabaseURNs);
            foreach (string script in scriptCollection)
            {
                if (script.Contains("CREATE DATABASE"))
                {
                    sb.AppendLine(string.Format("CREATE DATABASE [{0}]", item));
                    sb.AppendLine("GO");

                    sb.AppendLine(string.Format("USE [{0}]", item));
                    sb.AppendLine("GO");
                }
                else
                {
                    sb.AppendLine(script);
                    sb.AppendLine("GO");
                }
            }

            foreach (string script in GetTransferScript(dbname))
            {
                if (!script.Contains("CREATE USER") && !script.Contains("sys.sp_addrolemember"))
                {
                    sb.AppendLine(script);
                    sb.AppendLine("GO");
                }
            }

            sb = sb.Replace(item, string.Format("{0}_{1}", item, currentDateShort));
            File.WriteAllText(string.Format("{0}\\{1}_{2}.sql", sqlFilesOutputPath, item, currentDate), sb.ToString());
        }

        private StringCollection GetTransferScript(Database database)
        {

            var transfer = new Transfer(database);

            transfer.CopyAllObjects = true;
            transfer.CopyAllSynonyms = true;
            transfer.CopyData = false;

            transfer.CopyAllRoles = false;

            // additional options
            transfer.Options.WithDependencies = true;
            transfer.Options.DriAll = true;
            transfer.Options.Triggers = true;
            transfer.Options.Indexes = true;
            transfer.Options.SchemaQualifyForeignKeysReferences = true;
            transfer.Options.ExtendedProperties = true;
            transfer.Options.IncludeDatabaseRoleMemberships = true;
            transfer.Options.Permissions = true;
            transfer.PreserveDbo = true;

            // generates script
            return transfer.ScriptTransfer();
        }

        private void btnEnums_Click(object sender, EventArgs e)
        {
            string sqlFilesOutputPath = txtDirectory.Text;

            if (string.IsNullOrEmpty(sqlFilesOutputPath))
            {
                MessageBox.Show("Please select Output directory!");
                txtDirectory.Focus();
                return;
            }

            string currentDate = DateTime.Now.ToString("yyyMMddHHmm");
            string currentDateShort = "20180126_test";//DateTime.Now.ToString("yyyMMdd");

            DisplayProgressBar(2);
            Thread.Sleep(1000);
            pBar1.PerformStep();
            Common.GenerateEnumerationsScript(Common.BinPath, currentDateShort,false, sqlFilesOutputPath, "TOneV2");
            pBar1.PerformStep();
        }
    }
}
