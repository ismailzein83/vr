namespace Vanrise.HelperTools
{
    partial class TraceBCPCommand
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TraceBCPCommand));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btn_browse = new System.Windows.Forms.Button();
            this.txt_connectionString = new System.Windows.Forms.TextBox();
            this.btn_trace = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_tableName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_columns = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btn_browse
            // 
            this.btn_browse.Location = new System.Drawing.Point(13, 13);
            this.btn_browse.Name = "btn_browse";
            this.btn_browse.Size = new System.Drawing.Size(75, 23);
            this.btn_browse.TabIndex = 0;
            this.btn_browse.Text = "Browse";
            this.btn_browse.UseVisualStyleBackColor = true;
            this.btn_browse.Click += new System.EventHandler(this.btn_browse_Click);
            // 
            // txt_connectionString
            // 
            this.txt_connectionString.Location = new System.Drawing.Point(13, 80);
            this.txt_connectionString.Name = "txt_connectionString";
            this.txt_connectionString.Size = new System.Drawing.Size(873, 20);
            this.txt_connectionString.TabIndex = 1;
            this.txt_connectionString.Text = "Server=192.168.110.185;Database=TOneV2_Dev_CDR;User ID=Development;Password=dev!1" +
    "23";
            // 
            // btn_trace
            // 
            this.btn_trace.Location = new System.Drawing.Point(13, 262);
            this.btn_trace.Name = "btn_trace";
            this.btn_trace.Size = new System.Drawing.Size(75, 23);
            this.btn_trace.TabIndex = 2;
            this.btn_trace.Text = "Trace";
            this.btn_trace.UseVisualStyleBackColor = true;
            this.btn_trace.Click += new System.EventHandler(this.btn_trace_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Connection String";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Table Name";
            // 
            // txt_tableName
            // 
            this.txt_tableName.Location = new System.Drawing.Point(13, 145);
            this.txt_tableName.Name = "txt_tableName";
            this.txt_tableName.Size = new System.Drawing.Size(873, 20);
            this.txt_tableName.TabIndex = 6;
            this.txt_tableName.Text = "TOneWhS_CDR.BillingCDR_Failed";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 199);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Columns";
            // 
            // txt_columns
            // 
            this.txt_columns.Location = new System.Drawing.Point(13, 218);
            this.txt_columns.Name = "txt_columns";
            this.txt_columns.Size = new System.Drawing.Size(873, 20);
            this.txt_columns.TabIndex = 8;
            this.txt_columns.Text = resources.GetString("txt_columns.Text");
            // 
            // TraceBCPCommand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 447);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_columns);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt_tableName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_trace);
            this.Controls.Add(this.txt_connectionString);
            this.Controls.Add(this.btn_browse);
            this.Name = "TraceBCPCommand";
            this.Text = "TraceBCPCommand";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected internal System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btn_browse;
        private System.Windows.Forms.TextBox txt_connectionString;
        private System.Windows.Forms.Button btn_trace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_tableName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_columns;

    }
}