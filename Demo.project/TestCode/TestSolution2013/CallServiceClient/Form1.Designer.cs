﻿namespace CallServiceClient
{
    partial class Form1
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
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btnCallNoverca = new System.Windows.Forms.Button();
            this.btnCallAudiService = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(70, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(143, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Call using RestSharp";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(70, 138);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(143, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Call Directly";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(70, 205);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(143, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Call using HttpClient";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(70, 235);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(143, 23);
            this.button4.TabIndex = 3;
            this.button4.Text = "WSDL to Code";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // btnCallNoverca
            // 
            this.btnCallNoverca.Location = new System.Drawing.Point(338, 54);
            this.btnCallNoverca.Name = "btnCallNoverca";
            this.btnCallNoverca.Size = new System.Drawing.Size(108, 23);
            this.btnCallNoverca.TabIndex = 4;
            this.btnCallNoverca.Text = "Call Noverca";
            this.btnCallNoverca.UseVisualStyleBackColor = true;
            this.btnCallNoverca.Click += new System.EventHandler(this.btnCallNoverca_Click);
            // 
            // btnCallAudiService
            // 
            this.btnCallAudiService.Location = new System.Drawing.Point(338, 128);
            this.btnCallAudiService.Name = "btnCallAudiService";
            this.btnCallAudiService.Size = new System.Drawing.Size(108, 23);
            this.btnCallAudiService.TabIndex = 5;
            this.btnCallAudiService.Text = "Call Audi Service";
            this.btnCallAudiService.UseVisualStyleBackColor = true;
            this.btnCallAudiService.Click += new System.EventHandler(this.btnCallAudiService_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 261);
            this.Controls.Add(this.btnCallAudiService);
            this.Controls.Add(this.btnCallNoverca);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btnCallNoverca;
        private System.Windows.Forms.Button btnCallAudiService;
    }
}

