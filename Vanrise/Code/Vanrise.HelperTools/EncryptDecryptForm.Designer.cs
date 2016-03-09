namespace Vanrise.HelperTools
{
    partial class EncryptDecryptForm
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
            this.btnDecryptCode = new System.Windows.Forms.Button();
            this.btnEncryptCode = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtEncyptedCode = new System.Windows.Forms.TextBox();
            this.txtDecryptedCode = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDecryptCode
            // 
            this.btnDecryptCode.Location = new System.Drawing.Point(387, 12);
            this.btnDecryptCode.Name = "btnDecryptCode";
            this.btnDecryptCode.Size = new System.Drawing.Size(94, 26);
            this.btnDecryptCode.TabIndex = 13;
            this.btnDecryptCode.Text = "<- Decrypt";
            this.btnDecryptCode.UseVisualStyleBackColor = true;
            this.btnDecryptCode.Click += new System.EventHandler(this.btnDecryptCode_Click);
            // 
            // btnEncryptCode
            // 
            this.btnEncryptCode.Location = new System.Drawing.Point(287, 12);
            this.btnEncryptCode.Name = "btnEncryptCode";
            this.btnEncryptCode.Size = new System.Drawing.Size(94, 26);
            this.btnEncryptCode.TabIndex = 11;
            this.btnEncryptCode.Text = "Encrypt ->";
            this.btnEncryptCode.UseVisualStyleBackColor = true;
            this.btnEncryptCode.Click += new System.EventHandler(this.btnEncryptCode_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(666, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Ecnrypted Code";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Decrypted Code";
            // 
            // txtEncyptedCode
            // 
            this.txtEncyptedCode.AcceptsReturn = true;
            this.txtEncyptedCode.AcceptsTab = true;
            this.txtEncyptedCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEncyptedCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEncyptedCode.Location = new System.Drawing.Point(370, 44);
            this.txtEncyptedCode.MaxLength = 0;
            this.txtEncyptedCode.Multiline = true;
            this.txtEncyptedCode.Name = "txtEncyptedCode";
            this.txtEncyptedCode.Size = new System.Drawing.Size(382, 285);
            this.txtEncyptedCode.TabIndex = 10;
            this.txtEncyptedCode.WordWrap = false;
            // 
            // txtDecryptedCode
            // 
            this.txtDecryptedCode.AcceptsReturn = true;
            this.txtDecryptedCode.AcceptsTab = true;
            this.txtDecryptedCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDecryptedCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDecryptedCode.Location = new System.Drawing.Point(5, 44);
            this.txtDecryptedCode.MaxLength = 0;
            this.txtDecryptedCode.Multiline = true;
            this.txtDecryptedCode.Name = "txtDecryptedCode";
            this.txtDecryptedCode.Size = new System.Drawing.Size(359, 285);
            this.txtDecryptedCode.TabIndex = 9;
            this.txtDecryptedCode.WordWrap = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox1.Controls.Add(this.btnDecryptCode);
            this.groupBox1.Controls.Add(this.btnEncryptCode);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtEncyptedCode);
            this.groupBox1.Controls.Add(this.txtDecryptedCode);
            this.groupBox1.Location = new System.Drawing.Point(185, 165);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(758, 335);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Encryption Tool";
            // 
            // EncryptDecryptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1129, 664);
            this.Controls.Add(this.groupBox1);
            this.Name = "EncryptDecryptForm";
            this.Text = "EncryptDecryptForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDecryptCode;
        private System.Windows.Forms.Button btnEncryptCode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtEncyptedCode;
        private System.Windows.Forms.TextBox txtDecryptedCode;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}