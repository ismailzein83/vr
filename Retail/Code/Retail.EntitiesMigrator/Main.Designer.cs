﻿namespace Retail.EntitiesMigrator
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnImportIntlRates = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.numInternationalChargingPolicy = new System.Windows.Forms.NumericUpDown();
            this.numOnNetChargingPolicy = new System.Windows.Forms.NumericUpDown();
            this.numOffNetChargingPolicy = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numMobileChargingPolicy = new System.Windows.Forms.NumericUpDown();
            this.btnImportOutGoingRates = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cboCurrency = new System.Windows.Forms.ComboBox();
            this.cboSNP = new System.Windows.Forms.ComboBox();
            this.btnImpOffNetRates = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.bsSNP = new System.Windows.Forms.BindingSource(this.components);
            this.bsCurrency = new System.Windows.Forms.BindingSource(this.components);
            this.btnImportOnNet = new System.Windows.Forms.Button();
            this.btnMobileRates = new System.Windows.Forms.Button();
            this.btnImportMonthlyCharges = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.numPricingUnit = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInternationalChargingPolicy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOnNetChargingPolicy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOffNetChargingPolicy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMobileChargingPolicy)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsSNP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCurrency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPricingUnit)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnImportIntlRates, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnImportOutGoingRates, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnImpOffNetRates, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnImportOnNet, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnMobileRates, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnImportMonthlyCharges, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(750, 313);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // btnImportIntlRates
            // 
            this.btnImportIntlRates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImportIntlRates.Location = new System.Drawing.Point(3, 205);
            this.btnImportIntlRates.Name = "btnImportIntlRates";
            this.btnImportIntlRates.Size = new System.Drawing.Size(369, 49);
            this.btnImportIntlRates.TabIndex = 4;
            this.btnImportIntlRates.Text = "Import International Rates";
            this.btnImportIntlRates.UseVisualStyleBackColor = true;
            this.btnImportIntlRates.Click += new System.EventHandler(this.btnImportIntlRates_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.numInternationalChargingPolicy, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.numOnNetChargingPolicy, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.numOffNetChargingPolicy, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.numMobileChargingPolicy, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(369, 141);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // numInternationalChargingPolicy
            // 
            this.numInternationalChargingPolicy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numInternationalChargingPolicy.Location = new System.Drawing.Point(187, 87);
            this.numInternationalChargingPolicy.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.numInternationalChargingPolicy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numInternationalChargingPolicy.Name = "numInternationalChargingPolicy";
            this.numInternationalChargingPolicy.Size = new System.Drawing.Size(179, 20);
            this.numInternationalChargingPolicy.TabIndex = 7;
            this.numInternationalChargingPolicy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numInternationalChargingPolicy.ValueChanged += new System.EventHandler(this.numInternationalChargingPolicy_ValueChanged);
            // 
            // numOnNetChargingPolicy
            // 
            this.numOnNetChargingPolicy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numOnNetChargingPolicy.Location = new System.Drawing.Point(187, 59);
            this.numOnNetChargingPolicy.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.numOnNetChargingPolicy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numOnNetChargingPolicy.Name = "numOnNetChargingPolicy";
            this.numOnNetChargingPolicy.Size = new System.Drawing.Size(179, 20);
            this.numOnNetChargingPolicy.TabIndex = 6;
            this.numOnNetChargingPolicy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numOnNetChargingPolicy.ValueChanged += new System.EventHandler(this.numOnNetChargingPolicy_ValueChanged);
            // 
            // numOffNetChargingPolicy
            // 
            this.numOffNetChargingPolicy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numOffNetChargingPolicy.Location = new System.Drawing.Point(187, 31);
            this.numOffNetChargingPolicy.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.numOffNetChargingPolicy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numOffNetChargingPolicy.Name = "numOffNetChargingPolicy";
            this.numOffNetChargingPolicy.Size = new System.Drawing.Size(179, 20);
            this.numOffNetChargingPolicy.TabIndex = 5;
            this.numOffNetChargingPolicy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numOffNetChargingPolicy.ValueChanged += new System.EventHandler(this.numOffNetChargingPolicy_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mobile Charging Policy";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Off Net Charging Policy";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "On Net Charging Policy";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(141, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "International Charging Policy";
            // 
            // numMobileChargingPolicy
            // 
            this.numMobileChargingPolicy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numMobileChargingPolicy.Location = new System.Drawing.Point(187, 3);
            this.numMobileChargingPolicy.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.numMobileChargingPolicy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMobileChargingPolicy.Name = "numMobileChargingPolicy";
            this.numMobileChargingPolicy.Size = new System.Drawing.Size(179, 20);
            this.numMobileChargingPolicy.TabIndex = 4;
            this.numMobileChargingPolicy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMobileChargingPolicy.ValueChanged += new System.EventHandler(this.numMobileChargingPolicy_ValueChanged);
            // 
            // btnImportOutGoingRates
            // 
            this.btnImportOutGoingRates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImportOutGoingRates.Location = new System.Drawing.Point(3, 150);
            this.btnImportOutGoingRates.Name = "btnImportOutGoingRates";
            this.btnImportOutGoingRates.Size = new System.Drawing.Size(369, 49);
            this.btnImportOutGoingRates.TabIndex = 2;
            this.btnImportOutGoingRates.Text = "Import Incoming Rates";
            this.btnImportOutGoingRates.UseVisualStyleBackColor = true;
            this.btnImportOutGoingRates.Click += new System.EventHandler(this.btnImportOutGoingRates_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.cboCurrency, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.cboSNP, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label7, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.numPricingUnit, 1, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(378, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(369, 141);
            this.tableLayoutPanel3.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Currency";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Selling Number Plan";
            // 
            // cboCurrency
            // 
            this.cboCurrency.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboCurrency.FormattingEnabled = true;
            this.cboCurrency.Location = new System.Drawing.Point(187, 3);
            this.cboCurrency.Name = "cboCurrency";
            this.cboCurrency.Size = new System.Drawing.Size(179, 21);
            this.cboCurrency.TabIndex = 2;
            this.cboCurrency.SelectedIndexChanged += new System.EventHandler(this.cboCurrency_SelectedIndexChanged);
            // 
            // cboSNP
            // 
            this.cboSNP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboSNP.FormattingEnabled = true;
            this.cboSNP.Location = new System.Drawing.Point(187, 31);
            this.cboSNP.Name = "cboSNP";
            this.cboSNP.Size = new System.Drawing.Size(179, 21);
            this.cboSNP.TabIndex = 3;
            this.cboSNP.SelectedIndexChanged += new System.EventHandler(this.cboSNP_SelectedIndexChanged);
            // 
            // btnImpOffNetRates
            // 
            this.btnImpOffNetRates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImpOffNetRates.Location = new System.Drawing.Point(378, 150);
            this.btnImpOffNetRates.Name = "btnImpOffNetRates";
            this.btnImpOffNetRates.Size = new System.Drawing.Size(369, 49);
            this.btnImpOffNetRates.TabIndex = 6;
            this.btnImpOffNetRates.Text = "Import OffNet Rates";
            this.btnImpOffNetRates.UseVisualStyleBackColor = true;
            this.btnImpOffNetRates.Click += new System.EventHandler(this.btnImpOffNetRates_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnImportOnNet
            // 
            this.btnImportOnNet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImportOnNet.Location = new System.Drawing.Point(378, 205);
            this.btnImportOnNet.Name = "btnImportOnNet";
            this.btnImportOnNet.Size = new System.Drawing.Size(369, 49);
            this.btnImportOnNet.TabIndex = 7;
            this.btnImportOnNet.Text = "Import On Net Rates";
            this.btnImportOnNet.UseVisualStyleBackColor = true;
            this.btnImportOnNet.Click += new System.EventHandler(this.btnImportOnNet_Click);
            // 
            // btnMobileRates
            // 
            this.btnMobileRates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMobileRates.Location = new System.Drawing.Point(3, 260);
            this.btnMobileRates.Name = "btnMobileRates";
            this.btnMobileRates.Size = new System.Drawing.Size(369, 50);
            this.btnMobileRates.TabIndex = 8;
            this.btnMobileRates.Text = "Import Mobile Rates";
            this.btnMobileRates.UseVisualStyleBackColor = true;
            this.btnMobileRates.Click += new System.EventHandler(this.btnMobileRates_Click);
            // 
            // btnImportMonthlyCharges
            // 
            this.btnImportMonthlyCharges.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImportMonthlyCharges.Location = new System.Drawing.Point(378, 260);
            this.btnImportMonthlyCharges.Name = "btnImportMonthlyCharges";
            this.btnImportMonthlyCharges.Size = new System.Drawing.Size(369, 50);
            this.btnImportMonthlyCharges.TabIndex = 9;
            this.btnImportMonthlyCharges.Text = "Import Monthly Charges";
            this.btnImportMonthlyCharges.UseVisualStyleBackColor = true;
            this.btnImportMonthlyCharges.Click += new System.EventHandler(this.btnImportMonthlyCharges_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Pricing Unit";
            // 
            // numPricingUnit
            // 
            this.numPricingUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numPricingUnit.Location = new System.Drawing.Point(187, 59);
            this.numPricingUnit.Name = "numPricingUnit";
            this.numPricingUnit.Size = new System.Drawing.Size(179, 20);
            this.numPricingUnit.TabIndex = 5;
            this.numPricingUnit.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numPricingUnit.ValueChanged += new System.EventHandler(this.numPricingUnit_ValueChanged);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 313);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MultiNet-Import Entities";
            this.Load += new System.EventHandler(this.Main_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInternationalChargingPolicy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOnNetChargingPolicy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOffNetChargingPolicy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMobileChargingPolicy)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsSNP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCurrency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPricingUnit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnImportOutGoingRates;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.NumericUpDown numInternationalChargingPolicy;
        private System.Windows.Forms.NumericUpDown numOnNetChargingPolicy;
        private System.Windows.Forms.NumericUpDown numOffNetChargingPolicy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numMobileChargingPolicy;
        private System.Windows.Forms.Button btnImportIntlRates;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboCurrency;
        private System.Windows.Forms.ComboBox cboSNP;
        private System.Windows.Forms.BindingSource bsSNP;
        private System.Windows.Forms.BindingSource bsCurrency;
        private System.Windows.Forms.Button btnImpOffNetRates;
        private System.Windows.Forms.Button btnImportOnNet;
        private System.Windows.Forms.Button btnMobileRates;
        private System.Windows.Forms.Button btnImportMonthlyCharges;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numPricingUnit;
    }
}

