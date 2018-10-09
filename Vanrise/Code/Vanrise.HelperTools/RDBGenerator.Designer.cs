namespace Vanrise.HelperTools
{
    partial class RDBGenerator
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
            this.connectionString = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.generatedCode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.GenerateRDBCode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connectionString
            // 
            this.connectionString.Location = new System.Drawing.Point(29, 40);
            this.connectionString.Name = "connectionString";
            this.connectionString.Text = "Server=192.168.110.185;Database=TOneConfiguration;User ID=Development;Password=dev!123";
            this.connectionString.Size = new System.Drawing.Size(1022, 20);
            this.connectionString.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Connection String";
            // 
            // generatedCode
            // 
            this.generatedCode.Location = new System.Drawing.Point(26, 113);
            this.generatedCode.Multiline = true;
            this.generatedCode.Name = "generatedCode";
            this.generatedCode.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.generatedCode.Size = new System.Drawing.Size(1022, 698);
            this.generatedCode.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Generated RDB Code";
            // 
            // GenerateRDBCode
            // 
            this.GenerateRDBCode.Location = new System.Drawing.Point(416, 67);
            this.GenerateRDBCode.Name = "GenerateRDBCode";
            this.GenerateRDBCode.Size = new System.Drawing.Size(75, 23);
            this.GenerateRDBCode.TabIndex = 4;
            this.GenerateRDBCode.Text = "Generate";
            this.GenerateRDBCode.UseVisualStyleBackColor = true;
            this.GenerateRDBCode.Click += new System.EventHandler(this.GenerateRDBCode_Click);
            // 
            // RDBGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1093, 820);
            this.Controls.Add(this.GenerateRDBCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.generatedCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.connectionString);
            this.Name = "RDBGenerator";
            this.Text = "RDBGenerator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox connectionString;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox generatedCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button GenerateRDBCode;
    }
}