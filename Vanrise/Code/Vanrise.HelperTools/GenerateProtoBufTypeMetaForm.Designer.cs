namespace Vanrise.HelperTools
{
    partial class GenerateProtoBufTypeMetaForm
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
            this.txtCurrentMeta = new System.Windows.Forms.TextBox();
            this.txtNewMeta = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnSelectDLL = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTypes = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkNoCurrentMeta = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblDLL = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtCurrentMeta
            // 
            this.txtCurrentMeta.Location = new System.Drawing.Point(12, 103);
            this.txtCurrentMeta.Multiline = true;
            this.txtCurrentMeta.Name = "txtCurrentMeta";
            this.txtCurrentMeta.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCurrentMeta.Size = new System.Drawing.Size(851, 149);
            this.txtCurrentMeta.TabIndex = 0;
            this.txtCurrentMeta.TextChanged += new System.EventHandler(this.txtCurrentMeta_TextChanged);
            // 
            // txtNewMeta
            // 
            this.txtNewMeta.Location = new System.Drawing.Point(12, 310);
            this.txtNewMeta.Multiline = true;
            this.txtNewMeta.Name = "txtNewMeta";
            this.txtNewMeta.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNewMeta.Size = new System.Drawing.Size(851, 149);
            this.txtNewMeta.TabIndex = 1;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Enabled = false;
            this.btnGenerate.Location = new System.Drawing.Point(788, 258);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnSelectDLL
            // 
            this.btnSelectDLL.Location = new System.Drawing.Point(12, 12);
            this.btnSelectDLL.Name = "btnSelectDLL";
            this.btnSelectDLL.Size = new System.Drawing.Size(75, 23);
            this.btnSelectDLL.TabIndex = 3;
            this.btnSelectDLL.Text = "Select DLL";
            this.btnSelectDLL.UseVisualStyleBackColor = true;
            this.btnSelectDLL.Click += new System.EventHandler(this.btnSelectDLL_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Types";
            // 
            // cmbTypes
            // 
            this.cmbTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTypes.FormattingEnabled = true;
            this.cmbTypes.Location = new System.Drawing.Point(93, 36);
            this.cmbTypes.Name = "cmbTypes";
            this.cmbTypes.Size = new System.Drawing.Size(770, 21);
            this.cmbTypes.TabIndex = 5;
            this.cmbTypes.SelectedIndexChanged += new System.EventHandler(this.cmbTypes_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(284, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Current Serialized Metadata Properties (\"Prop1\", \"Prop2\"..)";
            // 
            // chkNoCurrentMeta
            // 
            this.chkNoCurrentMeta.AutoSize = true;
            this.chkNoCurrentMeta.Location = new System.Drawing.Point(738, 84);
            this.chkNoCurrentMeta.Name = "chkNoCurrentMeta";
            this.chkNoCurrentMeta.Size = new System.Drawing.Size(125, 17);
            this.chkNoCurrentMeta.TabIndex = 7;
            this.chkNoCurrentMeta.Text = "No Current Metadata";
            this.chkNoCurrentMeta.UseVisualStyleBackColor = true;
            this.chkNoCurrentMeta.CheckedChanged += new System.EventHandler(this.chkNoCurrentMeta_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 285);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "New Metadata Properties";
            // 
            // lblDLL
            // 
            this.lblDLL.AutoSize = true;
            this.lblDLL.Location = new System.Drawing.Point(94, 17);
            this.lblDLL.Name = "lblDLL";
            this.lblDLL.Size = new System.Drawing.Size(0, 13);
            this.lblDLL.TabIndex = 9;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Libraries|*.dll;*.exe";
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(766, 10);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(97, 20);
            this.txtFilter.TabIndex = 10;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // GenerateProtoBufTypeMetaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 471);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.lblDLL);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkNoCurrentMeta);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbTypes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSelectDLL);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.txtNewMeta);
            this.Controls.Add(this.txtCurrentMeta);
            this.Name = "GenerateProtoBufTypeMetaForm";
            this.Text = "GenerateProtoBufTypeMetaForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCurrentMeta;
        private System.Windows.Forms.TextBox txtNewMeta;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnSelectDLL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbTypes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkNoCurrentMeta;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblDLL;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtFilter;
    }
}