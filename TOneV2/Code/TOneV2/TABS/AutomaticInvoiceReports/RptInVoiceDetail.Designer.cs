namespace TABS.AutomaticInvoiceReports
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    partial class RptInVoiceDetail
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.Drawing.FormattingRule formattingRule1 = new Telerik.Reporting.Drawing.FormattingRule();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.txtRateMin = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.piccompanylogo1 = new Telerik.Reporting.PictureBox();
            this.txtCustomerGroup = new Telerik.Reporting.TextBox();
            this.txtInvoiceDate = new Telerik.Reporting.TextBox();
            this.txtSerialNumber = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.group1 = new Telerik.Reporting.Group();
            this.groupFooterSection1 = new Telerik.Reporting.GroupFooterSection();
            this.shape3 = new Telerik.Reporting.Shape();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.txtAmounttotal = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.groupHeaderSection1 = new Telerik.Reporting.GroupHeaderSection();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.txtRate = new Telerik.Reporting.TextBox();
            this.txtAmount = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.shape1 = new Telerik.Reporting.Shape();
            this.textBox12 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            formattingRule1.Filters.AddRange(new Telerik.Reporting.Data.Filter[] {
            new Telerik.Reporting.Data.Filter("=RowNumber() % 2", Telerik.Reporting.Data.FilterOperator.Equal, "0")});
            formattingRule1.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.detail.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule1});
            this.detail.Height = new Telerik.Reporting.Drawing.Unit(0.2083333283662796, Telerik.Reporting.Drawing.UnitType.Inch);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1,
            this.textBox2,
            this.textBox3,
            this.txtRateMin,
            this.textBox5,
            this.textBox6,
            this.textBox11,
            this.textBox4,
            this.textBox15});
            this.detail.Name = "detail";
            // 
            // textBox1
            // 
            this.textBox1.Format = "{0:dd/MM/yyyy}";
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(2.25, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.75, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.20000000298023224, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox1.Style.Color = System.Drawing.Color.Black;
            this.textBox1.Style.Font.Bold = false;
            this.textBox1.Style.Font.Italic = false;
            this.textBox1.Style.Font.Name = "Verdana";
            this.textBox1.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox1.Style.Font.Strikeout = false;
            this.textBox1.Style.Font.Underline = false;
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox1.Value = "=Fields.FromDate";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.039999999105930328, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.1700000762939453, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.20000000298023224, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox2.Style.Font.Bold = false;
            this.textBox2.Style.Font.Italic = false;
            this.textBox2.Style.Font.Name = "Verdana";
            this.textBox2.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox2.Style.Font.Strikeout = false;
            this.textBox2.Style.Font.Underline = false;
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox2.Value = "=Fields.Destination";
            // 
            // textBox3
            // 
            this.textBox3.Format = "{0:#,##0.00}";
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(7.0416665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.75, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.20000000298023224, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox3.Style.Color = System.Drawing.Color.Black;
            this.textBox3.Style.Font.Bold = false;
            this.textBox3.Style.Font.Italic = false;
            this.textBox3.Style.Font.Name = "Verdana";
            this.textBox3.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox3.Style.Font.Strikeout = false;
            this.textBox3.Style.Font.Underline = false;
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.Value = "=Fields.Amount";
            // 
            // txtRateMin
            // 
            this.txtRateMin.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.910494327545166, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtRateMin.Name = "txtRateMin";
            this.txtRateMin.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.7700003981590271, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.20000000298023224, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtRateMin.Style.Font.Bold = false;
            this.txtRateMin.Style.Font.Italic = false;
            this.txtRateMin.Style.Font.Name = "Verdana";
            this.txtRateMin.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtRateMin.Style.Font.Strikeout = false;
            this.txtRateMin.Style.Font.Underline = false;
            this.txtRateMin.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtRateMin.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtRateMin.Value = "=Fields.Rate";
            // 
            // textBox5
            // 
            this.textBox5.Format = "{0:#,##0.00}";
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(4.03125, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.0312498807907105, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.20000000298023224, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox5.Style.Color = System.Drawing.Color.Black;
            this.textBox5.Style.Font.Bold = false;
            this.textBox5.Style.Font.Italic = false;
            this.textBox5.Style.Font.Name = "Verdana";
            this.textBox5.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox5.Style.Font.Strikeout = false;
            this.textBox5.Style.Font.Underline = false;
            this.textBox5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox5.Value = "=Fields.Duration";
            // 
            // textBox6
            // 
            this.textBox6.Format = "{0:dd/MM/yyyy}";
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(3.2083334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.75, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.20000000298023224, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox6.Style.Color = System.Drawing.Color.Black;
            this.textBox6.Style.Font.Bold = false;
            this.textBox6.Style.Font.Italic = false;
            this.textBox6.Style.Font.Name = "Verdana";
            this.textBox6.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox6.Style.Font.Strikeout = false;
            this.textBox6.Style.Font.Underline = false;
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox6.Value = "=Fields.TillDate";
            // 
            // textBox11
            // 
            this.textBox11.Format = "{0:dd/MM/yyyy}";
            this.textBox11.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(3.0416667461395264, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.125, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.2083333283662796, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox11.Style.Color = System.Drawing.Color.Black;
            this.textBox11.Style.Font.Bold = false;
            this.textBox11.Style.Font.Italic = false;
            this.textBox11.Style.Font.Name = "Verdana";
            this.textBox11.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox11.Style.Font.Strikeout = false;
            this.textBox11.Style.Font.Underline = false;
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox11.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox11.Value = "-";
            // 
            // textBox4
            // 
            this.textBox4.Format = "{0:#,##0}";
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.0866665840148926, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.791666567325592, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.20000000298023224, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox4.Style.Font.Bold = false;
            this.textBox4.Style.Font.Italic = false;
            this.textBox4.Style.Font.Name = "Verdana";
            this.textBox4.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox4.Style.Font.Strikeout = false;
            this.textBox4.Style.Font.Underline = false;
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox4.Value = "=Fields.NumberOfCalls";
            // 
            // textBox15
            // 
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(6.7083334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.33325442671775818, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.20000000298023224, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox15.Style.Font.Bold = false;
            this.textBox15.Style.Font.Italic = false;
            this.textBox15.Style.Font.Name = "Verdana";
            this.textBox15.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox15.Style.Font.Strikeout = false;
            this.textBox15.Style.Font.Underline = false;
            this.textBox15.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox15.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox15.Value = "=Fields.Currency";
            // 
            // piccompanylogo1
            // 
            this.piccompanylogo1.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.0833333358168602, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.125, Telerik.Reporting.Drawing.UnitType.Inch));
            this.piccompanylogo1.Name = "piccompanylogo1";
            this.piccompanylogo1.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.2083332538604736, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.70833331346511841, Telerik.Reporting.Drawing.UnitType.Inch));
            this.piccompanylogo1.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.piccompanylogo1.Style.Font.Bold = false;
            this.piccompanylogo1.Style.Font.Italic = false;
            this.piccompanylogo1.Style.Font.Strikeout = false;
            this.piccompanylogo1.Style.Font.Underline = false;
            // 
            // txtCustomerGroup
            // 
            this.txtCustomerGroup.Format = "{0}";
            this.txtCustomerGroup.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(3.9166667461395264, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.125, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomerGroup.Name = "txtCustomerGroup";
            this.txtCustomerGroup.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(3.8229167461395264, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.25, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomerGroup.Style.Color = System.Drawing.Color.Black;
            this.txtCustomerGroup.Style.Font.Bold = true;
            this.txtCustomerGroup.Style.Font.Italic = false;
            this.txtCustomerGroup.Style.Font.Name = "Verdana";
            this.txtCustomerGroup.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtCustomerGroup.Style.Font.Strikeout = false;
            this.txtCustomerGroup.Style.Font.Underline = false;
            this.txtCustomerGroup.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtCustomerGroup.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtCustomerGroup.Value = "CUSTOMER NAME";
            // 
            // txtInvoiceDate
            // 
            this.txtInvoiceDate.Format = "{0:yyyy.MM.dd}";
            this.txtInvoiceDate.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(6.65625, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.125, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtInvoiceDate.Name = "txtInvoiceDate";
            this.txtInvoiceDate.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.0625, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtInvoiceDate.Style.Font.Bold = false;
            this.txtInvoiceDate.Style.Font.Italic = true;
            this.txtInvoiceDate.Style.Font.Name = "Verdana";
            this.txtInvoiceDate.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtInvoiceDate.Style.Font.Strikeout = false;
            this.txtInvoiceDate.Style.Font.Underline = false;
            // 
            // txtSerialNumber
            // 
            this.txtSerialNumber.Format = "{0:#.}";
            this.txtSerialNumber.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(1, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.125, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtSerialNumber.Name = "txtSerialNumber";
            this.txtSerialNumber.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.75, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtSerialNumber.Style.Font.Bold = false;
            this.txtSerialNumber.Style.Font.Italic = true;
            this.txtSerialNumber.Style.Font.Name = "Verdana";
            this.txtSerialNumber.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtSerialNumber.Style.Font.Strikeout = false;
            this.txtSerialNumber.Style.Font.Underline = false;
            // 
            // textBox21
            // 
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.0833333358168602, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.125, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.83333331346511841, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox21.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.textBox21.Style.Color = System.Drawing.Color.Black;
            this.textBox21.Style.Font.Bold = true;
            this.textBox21.Style.Font.Italic = false;
            this.textBox21.Style.Font.Name = "Verdana";
            this.textBox21.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox21.Style.Font.Strikeout = false;
            this.textBox21.Style.Font.Underline = false;
            this.textBox21.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox21.Value = "Invoice No";
            // 
            // textBox20
            // 
            this.textBox20.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.75, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.125, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.875, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.15625, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox20.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.textBox20.Style.Color = System.Drawing.Color.Black;
            this.textBox20.Style.Font.Bold = true;
            this.textBox20.Style.Font.Italic = false;
            this.textBox20.Style.Font.Name = "Verdana";
            this.textBox20.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox20.Style.Font.Strikeout = false;
            this.textBox20.Style.Font.Underline = false;
            this.textBox20.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox20.Value = "Invoice Date";
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(3.25, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.4479166567325592, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.375, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.3333333432674408, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox16.Style.Color = System.Drawing.Color.Black;
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Italic = true;
            this.textBox16.Style.Font.Name = "Verdana";
            this.textBox16.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(18, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox16.Style.Font.Strikeout = false;
            this.textBox16.Style.Font.Underline = false;
            this.textBox16.Value = "Invoice";
            // 
            // group1
            // 
            this.group1.GroupFooter = this.groupFooterSection1;
            this.group1.GroupHeader = this.groupHeaderSection1;
            // 
            // groupFooterSection1
            // 
            this.groupFooterSection1.Height = new Telerik.Reporting.Drawing.Unit(0.25007882714271545, Telerik.Reporting.Drawing.UnitType.Inch);
            this.groupFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.shape3,
            this.textBox7,
            this.textBox8,
            this.txtAmounttotal,
            this.textBox14});
            this.groupFooterSection1.Name = "groupFooterSection1";
            // 
            // shape3
            // 
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(2.96875, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.EW);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(4.9166665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.0833333358168602, Telerik.Reporting.Drawing.UnitType.Inch));
            // 
            // textBox7
            // 
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(3, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.08341217041015625, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.75, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.Font.Italic = false;
            this.textBox7.Style.Font.Name = "Verdana";
            this.textBox7.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox7.Style.Font.Strikeout = false;
            this.textBox7.Style.Font.Underline = false;
            this.textBox7.Value = "Net Total";
            // 
            // textBox8
            // 
            this.textBox8.Format = "{0:#,##0.00}";
            this.textBox8.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(4.03125, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.08341217041015625, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.03125, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox8.Style.Font.Bold = false;
            this.textBox8.Style.Font.Italic = false;
            this.textBox8.Style.Font.Name = "Verdana";
            this.textBox8.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox8.Style.Font.Strikeout = false;
            this.textBox8.Style.Font.Underline = false;
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox8.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox8.Value = "=Sum(Fields.Duration)";
            // 
            // txtAmounttotal
            // 
            this.txtAmounttotal.Format = "{0:#,##0.00}";
            this.txtAmounttotal.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(7.03125, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.08341217041015625, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtAmounttotal.Name = "txtAmounttotal";
            this.txtAmounttotal.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.75, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtAmounttotal.Style.Font.Bold = false;
            this.txtAmounttotal.Style.Font.Italic = false;
            this.txtAmounttotal.Style.Font.Name = "Verdana";
            this.txtAmounttotal.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtAmounttotal.Style.Font.Strikeout = false;
            this.txtAmounttotal.Style.Font.Underline = false;
            this.txtAmounttotal.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtAmounttotal.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            // 
            // textBox14
            // 
            this.textBox14.Format = "{0:#,##0}";
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.0866665840148926, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.08341217041015625, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.79166668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox14.Style.Font.Bold = false;
            this.textBox14.Style.Font.Italic = false;
            this.textBox14.Style.Font.Name = "Verdana";
            this.textBox14.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox14.Style.Font.Strikeout = false;
            this.textBox14.Style.Font.Underline = false;
            this.textBox14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox14.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox14.Value = "=Sum(Fields.NumberOfCalls)";
            // 
            // groupHeaderSection1
            // 
            this.groupHeaderSection1.Height = new Telerik.Reporting.Drawing.Unit(2.0416667461395264, Telerik.Reporting.Drawing.UnitType.Inch);
            this.groupHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox18,
            this.txtRate,
            this.txtAmount,
            this.textBox9,
            this.textBox13,
            this.shape1,
            this.piccompanylogo1,
            this.txtCustomerGroup,
            this.txtInvoiceDate,
            this.txtSerialNumber,
            this.textBox21,
            this.textBox20,
            this.textBox16,
            this.textBox12});
            this.groupHeaderSection1.Name = "groupHeaderSection1";
            this.groupHeaderSection1.PrintOnEveryPage = true;
            this.groupHeaderSection1.Style.Font.Bold = true;
            this.groupHeaderSection1.Style.Font.Italic = false;
            this.groupHeaderSection1.Style.Font.Strikeout = false;
            this.groupHeaderSection1.Style.Font.Underline = false;
            // 
            // textBox18
            // 
            this.textBox18.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(4.03125, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.7916666269302368, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.03125, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox18.Style.Color = System.Drawing.Color.Black;
            this.textBox18.Style.Font.Bold = true;
            this.textBox18.Style.Font.Italic = false;
            this.textBox18.Style.Font.Name = "Verdana";
            this.textBox18.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox18.Style.Font.Strikeout = false;
            this.textBox18.Style.Font.Underline = false;
            this.textBox18.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox18.Value = "Duration (Min)";
            // 
            // txtRate
            // 
            this.txtRate.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.9096617698669434, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.7916666269302368, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtRate.Name = "txtRate";
            this.txtRate.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.77083301544189453, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtRate.Style.Font.Bold = true;
            this.txtRate.Style.Font.Italic = false;
            this.txtRate.Style.Font.Name = "Verdana";
            this.txtRate.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtRate.Style.Font.Strikeout = false;
            this.txtRate.Style.Font.Underline = false;
            this.txtRate.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtRate.Value = "Rate/Min";
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(7.0479168891906738, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.7916666269302368, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.73958331346511841, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtAmount.Style.Font.Bold = true;
            this.txtAmount.Style.Font.Italic = false;
            this.txtAmount.Style.Font.Name = "Verdana";
            this.txtAmount.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtAmount.Style.Font.Strikeout = false;
            this.txtAmount.Style.Font.Underline = false;
            this.txtAmount.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtAmount.Value = "Amount";
            // 
            // textBox9
            // 
            this.textBox9.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.0416666679084301, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.7916666269302368, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.875, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.Style.Font.Italic = false;
            this.textBox9.Style.Font.Name = "Verdana";
            this.textBox9.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox9.Style.Font.Strikeout = false;
            this.textBox9.Style.Font.Underline = false;
            this.textBox9.Value = "Zone Name";
            // 
            // textBox13
            // 
            this.textBox13.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(2.625, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.7916667461395264, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.91666668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox13.Style.Font.Bold = true;
            this.textBox13.Style.Font.Italic = false;
            this.textBox13.Style.Font.Name = "Verdana";
            this.textBox13.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox13.Style.Font.Strikeout = false;
            this.textBox13.Style.Font.Underline = false;
            this.textBox13.Value = "Billing Period";
            // 
            // shape1
            // 
            this.shape1.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.0416666679084301, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.9583333730697632, Telerik.Reporting.Drawing.UnitType.Inch));
            this.shape1.Name = "shape1";
            this.shape1.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.EW);
            this.shape1.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(7.8333334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.0833333358168602, Telerik.Reporting.Drawing.UnitType.Inch));
            // 
            // textBox12
            // 
            this.textBox12.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.0866665840148926, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.7915878295898438, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.79166632890701294, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox12.Style.Font.Bold = true;
            this.textBox12.Style.Font.Italic = false;
            this.textBox12.Style.Font.Name = "Verdana";
            this.textBox12.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox12.Style.Font.Strikeout = false;
            this.textBox12.Style.Font.Underline = false;
            this.textBox12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox12.Value = "Calls";
            // 
            // RptInVoiceDetail
            // 
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            this.group1});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.groupHeaderSection1,
            this.groupFooterSection1,
            this.detail});
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins.Bottom = new Telerik.Reporting.Drawing.Unit(0.17000000178813934, Telerik.Reporting.Drawing.UnitType.Inch);
            this.PageSettings.Margins.Left = new Telerik.Reporting.Drawing.Unit(0.17000000178813934, Telerik.Reporting.Drawing.UnitType.Inch);
            this.PageSettings.Margins.Right = new Telerik.Reporting.Drawing.Unit(0.17000000178813934, Telerik.Reporting.Drawing.UnitType.Inch);
            this.PageSettings.Margins.Top = new Telerik.Reporting.Drawing.Unit(0.17000000178813934, Telerik.Reporting.Drawing.UnitType.Inch);
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Style.BackgroundColor = System.Drawing.Color.White;
            this.Width = new Telerik.Reporting.Drawing.Unit(7.9166698455810547, Telerik.Reporting.Drawing.UnitType.Inch);
            this.ItemDataBinding += new System.EventHandler(this.RptInVoiceDetail_ItemDataBinding);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DetailSection detail;
        private Group group1;
        private GroupFooterSection groupFooterSection1;
        private GroupHeaderSection groupHeaderSection1;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox txtRate;
        private Telerik.Reporting.TextBox txtAmount;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox txtRateMin;
        private Telerik.Reporting.TextBox textBox5;
        private Shape shape1;
        private Shape shape3;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox txtAmounttotal;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.PictureBox piccompanylogo1;
        private Telerik.Reporting.TextBox txtCustomerGroup;
        private Telerik.Reporting.TextBox txtInvoiceDate;
        private Telerik.Reporting.TextBox txtSerialNumber;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox15;
    }
}