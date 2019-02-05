namespace Vanrise.HelperTools
{
    partial class DataRecordStorageTransformationToRDB
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
            this.connectionStringLabel = new System.Windows.Forms.Label();
            this.generate = new System.Windows.Forms.Button();
            this.dataRecordLabel = new System.Windows.Forms.Label();
            this.generatedRDB = new System.Windows.Forms.TextBox();
            this.dataRecordStorageComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // connectionString
            // 
            this.connectionString.Location = new System.Drawing.Point(29, 40);
            this.connectionString.Name = "connectionString";
            this.connectionString.Size = new System.Drawing.Size(1022, 20);
            this.connectionString.TabIndex = 0;
            this.connectionString.Text = "Server=192.168.110.185;Database=Retail_Dev_Configuration;User ID=Development;Pass" +
    "word=dev!123";
            this.connectionString.TextChanged += new System.EventHandler(this.onConnectionStringChanged);
            // 
            // connectionStringLabel
            // 
            this.connectionStringLabel.AutoSize = true;
            this.connectionStringLabel.Location = new System.Drawing.Point(29, 21);
            this.connectionStringLabel.Name = "connectionStringLabel";
            this.connectionStringLabel.Size = new System.Drawing.Size(91, 13);
            this.connectionStringLabel.TabIndex = 1;
            this.connectionStringLabel.Text = "Connection String";
            // 
            // generate
            // 
            this.generate.Location = new System.Drawing.Point(568, 87);
            this.generate.Name = "generate";
            this.generate.Size = new System.Drawing.Size(75, 23);
            this.generate.TabIndex = 2;
            this.generate.Text = "Generate";
            this.generate.UseVisualStyleBackColor = true;
            this.generate.Click += new System.EventHandler(this.GenerateRDB);
            // 
            // dataRecordLabel
            // 
            this.dataRecordLabel.AutoSize = true;
            this.dataRecordLabel.Location = new System.Drawing.Point(26, 68);
            this.dataRecordLabel.Name = "dataRecordLabel";
            this.dataRecordLabel.Size = new System.Drawing.Size(71, 13);
            this.dataRecordLabel.TabIndex = 3;
            this.dataRecordLabel.Text = "Data Record ";
            // 
            // generatedRDB
            // 
            this.generatedRDB.Location = new System.Drawing.Point(26, 135);
            this.generatedRDB.Multiline = true;
            this.generatedRDB.Name = "generatedRDB";
            this.generatedRDB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.generatedRDB.Size = new System.Drawing.Size(1022, 648);
            this.generatedRDB.TabIndex = 2;
            // 
            // dataRecordStorageComboBox
            // 
            this.dataRecordStorageComboBox.FormattingEnabled = true;
            this.dataRecordStorageComboBox.Location = new System.Drawing.Point(26, 87);
            this.dataRecordStorageComboBox.Name = "dataRecordStorageComboBox";
            this.dataRecordStorageComboBox.Size = new System.Drawing.Size(500, 21);
            this.dataRecordStorageComboBox.TabIndex = 5;
            // 
            // DataRecordStorageTransformationToRDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1093, 820);
            this.Controls.Add(this.dataRecordStorageComboBox);
            this.Controls.Add(this.generatedRDB);
            this.Controls.Add(this.dataRecordLabel);
            this.Controls.Add(this.generate);
            this.Controls.Add(this.connectionStringLabel);
            this.Controls.Add(this.connectionString);
            this.Name = "DataRecordStorageTransformationToRDB";
            this.Text = "DataRecordStorageTransformationToRDB";
            this.Load += new System.EventHandler(this.LoadDataRecordTables);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox connectionString;
        private System.Windows.Forms.Label connectionStringLabel;
        private System.Windows.Forms.Button generate;
        private System.Windows.Forms.Label dataRecordLabel;
        private System.Windows.Forms.TextBox generatedRDB;
        private System.Windows.Forms.ComboBox dataRecordStorageComboBox;
    }
}