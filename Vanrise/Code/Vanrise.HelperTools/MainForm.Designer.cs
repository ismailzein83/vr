﻿namespace Vanrise.HelperTools
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.button1 = new System.Windows.Forms.Button();
            this.EncryptDecrypt = new System.Windows.Forms.Button();
            this.bcpCommand = new System.Windows.Forms.Button();
            this.CompressJS = new System.Windows.Forms.Button();
            this.rdb_Generator = new System.Windows.Forms.Button();
            this.TCPCLient = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(140, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "ProtoBuf Type Metadata";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // EncryptDecrypt
            // 
            this.EncryptDecrypt.Location = new System.Drawing.Point(13, 62);
            this.EncryptDecrypt.Name = "EncryptDecrypt";
            this.EncryptDecrypt.Size = new System.Drawing.Size(140, 23);
            this.EncryptDecrypt.TabIndex = 1;
            this.EncryptDecrypt.Text = "Encrypt Decrypt Tool";
            this.EncryptDecrypt.UseVisualStyleBackColor = true;
            this.EncryptDecrypt.Click += new System.EventHandler(this.EncryptDecrypt_Click);
            // 
            // bcpCommand
            // 
            this.bcpCommand.Location = new System.Drawing.Point(13, 113);
            this.bcpCommand.Name = "bcpCommand";
            this.bcpCommand.Size = new System.Drawing.Size(140, 23);
            this.bcpCommand.TabIndex = 2;
            this.bcpCommand.Text = "Trace BCP Command";
            this.bcpCommand.UseVisualStyleBackColor = true;
            this.bcpCommand.Click += new System.EventHandler(this.bcpCommand_Click);
            // 
            // CompressJS
            // 
            this.CompressJS.Location = new System.Drawing.Point(13, 169);
            this.CompressJS.Name = "CompressJS";
            this.CompressJS.Size = new System.Drawing.Size(140, 23);
            this.CompressJS.TabIndex = 3;
            this.CompressJS.Text = "Compress / Generate";
            this.CompressJS.UseVisualStyleBackColor = true;
            this.CompressJS.Click += new System.EventHandler(this.CompressJS_Click);
            // 
            // rdb_Generator
            // 
            this.rdb_Generator.Location = new System.Drawing.Point(13, 220);
            this.rdb_Generator.Name = "rdb_Generator";
            this.rdb_Generator.Size = new System.Drawing.Size(140, 23);
            this.rdb_Generator.TabIndex = 4;
            this.rdb_Generator.Text = "Generate RDB ";
            this.rdb_Generator.UseVisualStyleBackColor = true;
            this.rdb_Generator.Click += new System.EventHandler(this.RDBGenerator_Click);
            // 
            // TCPCLient
            // 
            this.TCPCLient.Location = new System.Drawing.Point(181, 12);
            this.TCPCLient.Name = "TCPCLient";
            this.TCPCLient.Size = new System.Drawing.Size(140, 23);
            this.TCPCLient.TabIndex = 5;
            this.TCPCLient.Text = "TCP Client";
            this.TCPCLient.UseVisualStyleBackColor = true;
            this.TCPCLient.Click += new System.EventHandler(this.tcpClient_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(181, 62);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(140, 23);
            this.button3.TabIndex = 7;
            this.button3.Text = "Data Record -> RDB";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.DataRecordRDB_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 262);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.TCPCLient);
            this.Controls.Add(this.rdb_Generator);
            this.Controls.Add(this.CompressJS);
            this.Controls.Add(this.bcpCommand);
            this.Controls.Add(this.EncryptDecrypt);
            this.Controls.Add(this.button1);
            this.Name = "MainForm";
            this.Text = "Helper Tools";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button EncryptDecrypt;
        private System.Windows.Forms.Button bcpCommand;
        private System.Windows.Forms.Button CompressJS;
        private System.Windows.Forms.Button rdb_Generator;
        private System.Windows.Forms.Button TCPCLient;
        private System.Windows.Forms.Button button3;
    }
}

