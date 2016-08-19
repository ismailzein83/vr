namespace TOne.WhS.DBSync.App
{
    partial class MigrationForm
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
            this.CBCurrentSwitches = new System.Windows.Forms.ComboBox();
            this.CBParser = new System.Windows.Forms.ComboBox();
            this.btnMigrate = new System.Windows.Forms.Button();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.LblInfo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CBCurrentSwitches
            // 
            this.CBCurrentSwitches.FormattingEnabled = true;
            this.CBCurrentSwitches.Location = new System.Drawing.Point(12, 36);
            this.CBCurrentSwitches.Name = "CBCurrentSwitches";
            this.CBCurrentSwitches.Size = new System.Drawing.Size(200, 21);
            this.CBCurrentSwitches.TabIndex = 0;
            this.CBCurrentSwitches.Text = "Select Switch";
            // 
            // CBParser
            // 
            this.CBParser.FormattingEnabled = true;
            this.CBParser.Location = new System.Drawing.Point(235, 36);
            this.CBParser.Name = "CBParser";
            this.CBParser.Size = new System.Drawing.Size(177, 21);
            this.CBParser.TabIndex = 1;
            this.CBParser.Text = "Select Parser";
            // 
            // btnMigrate
            // 
            this.btnMigrate.Location = new System.Drawing.Point(235, 100);
            this.btnMigrate.Name = "btnMigrate";
            this.btnMigrate.Size = new System.Drawing.Size(177, 23);
            this.btnMigrate.TabIndex = 3;
            this.btnMigrate.Text = "Migrate";
            this.btnMigrate.UseVisualStyleBackColor = true;
            this.btnMigrate.Click += new System.EventHandler(this.btnMigrate_Click);
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.Location = new System.Drawing.Point(12, 99);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker.TabIndex = 4;
            // 
            // LblInfo
            // 
            this.LblInfo.AutoSize = true;
            this.LblInfo.Location = new System.Drawing.Point(17, 128);
            this.LblInfo.Name = "LblInfo";
            this.LblInfo.Size = new System.Drawing.Size(0, 13);
            this.LblInfo.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Select Rule Effective Date";
            // 
            // MigrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 210);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LblInfo);
            this.Controls.Add(this.dateTimePicker);
            this.Controls.Add(this.btnMigrate);
            this.Controls.Add(this.CBParser);
            this.Controls.Add(this.CBCurrentSwitches);
            this.Name = "MigrationForm";
            this.Text = "TOne Sync App";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CBCurrentSwitches;
        private System.Windows.Forms.ComboBox CBParser;
        private System.Windows.Forms.Button btnMigrate;
        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private System.Windows.Forms.Label LblInfo;
        private System.Windows.Forms.Label label1;
    }
}

