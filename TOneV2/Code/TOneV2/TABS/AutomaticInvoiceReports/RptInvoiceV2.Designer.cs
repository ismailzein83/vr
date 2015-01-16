namespace TABS.AutomaticInvoiceReports
{
    using System.ComponentModel;
    using System.Drawing;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    partial class RptInvoiceV2
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            this.detail = new Telerik.Reporting.DetailSection();
            this.group1 = new Telerik.Reporting.Group();
            this.groupFooterSection1 = new Telerik.Reporting.GroupFooterSection();
            this.groupHeaderSection1 = new Telerik.Reporting.GroupHeaderSection();
            this.txtOwnName = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.txtOwnAdress = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.txtOwnRegNmber = new Telerik.Reporting.TextBox();
            this.txtCustomerFax = new Telerik.Reporting.TextBox();
            this.txtCustomerPhone = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.txtCustomer = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.txtCustomerAddress = new Telerik.Reporting.TextBox();
            this.txtDueDate = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.txtInvoiceDate = new Telerik.Reporting.TextBox();
            this.txtSerialNumber = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox23 = new Telerik.Reporting.TextBox();
            this.panel2 = new Telerik.Reporting.Panel();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.txtTotalAmount = new Telerik.Reporting.TextBox();
            this.txtTotalDuration = new Telerik.Reporting.TextBox();
            this.txtTotalCalls = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.shape1 = new Telerik.Reporting.Shape();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.picBoxOwnLogo = new Telerik.Reporting.PictureBox();
            this.txtBankingDetails = new Telerik.Reporting.TextBox();
            this.VatPanel = new Telerik.Reporting.Panel();
            this.txtVatAmount = new Telerik.Reporting.TextBox();
            this.lblVat = new Telerik.Reporting.TextBox();
            this.txtInvoiceNote = new Telerik.Reporting.HtmlTextBox();
            this.txtCustomerRegNumber = new Telerik.Reporting.TextBox();
            this.txtCustomerRegLabel = new Telerik.Reporting.TextBox();
            this.txtVatID = new Telerik.Reporting.TextBox();
            this.txtVatIDValue = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.txtOwnVATID = new Telerik.Reporting.TextBox();
            this.SRinvoiceDetail = new Telerik.Reporting.SubReport();
            this.rptInVoiceDetail1 = new TABS.AutomaticInvoiceReports.RptInVoiceDetail();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox35 = new Telerik.Reporting.TextBox();
            this.shape2 = new Telerik.Reporting.Shape();
            this.txtFooter = new Telerik.Reporting.TextBox();
            this.TableData = new Telerik.Reporting.Table();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox22 = new Telerik.Reporting.TextBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.rptInVoiceDetail1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            this.detail.Height = new Telerik.Reporting.Drawing.Unit(0.0833333358168602, Telerik.Reporting.Drawing.UnitType.Inch);
            this.detail.Name = "detail";
            this.detail.Style.Font.Bold = false;
            this.detail.Style.Font.Italic = false;
            this.detail.Style.Font.Strikeout = false;
            this.detail.Style.Font.Underline = false;
            this.detail.Style.Visible = false;
            // 
            // group1
            // 
            this.group1.GroupFooter = this.groupFooterSection1;
            this.group1.GroupHeader = this.groupHeaderSection1;
            // 
            // groupFooterSection1
            // 
            this.groupFooterSection1.Height = new Telerik.Reporting.Drawing.Unit(0.0520833320915699, Telerik.Reporting.Drawing.UnitType.Inch);
            this.groupFooterSection1.Name = "groupFooterSection1";
            this.groupFooterSection1.Style.Font.Bold = false;
            this.groupFooterSection1.Style.Font.Italic = false;
            this.groupFooterSection1.Style.Font.Strikeout = false;
            this.groupFooterSection1.Style.Font.Underline = false;
            this.groupFooterSection1.Style.Visible = false;
            // 
            // groupHeaderSection1
            // 
            this.groupHeaderSection1.Height = new Telerik.Reporting.Drawing.Unit(0.0520833320915699, Telerik.Reporting.Drawing.UnitType.Inch);
            this.groupHeaderSection1.Name = "groupHeaderSection1";
            this.groupHeaderSection1.Style.Font.Bold = true;
            this.groupHeaderSection1.Style.Font.Italic = false;
            this.groupHeaderSection1.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.groupHeaderSection1.Style.Font.Strikeout = false;
            this.groupHeaderSection1.Style.Font.Underline = false;
            // 
            // txtOwnName
            // 
            this.txtOwnName.Format = "{0:yyyy.MM.dd}";
            this.txtOwnName.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.2291665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.0833333358168602, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtOwnName.Name = "txtOwnName";
            this.txtOwnName.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.6458332538604736, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.25, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtOwnName.Style.Color = System.Drawing.Color.Black;
            this.txtOwnName.Style.Font.Bold = true;
            this.txtOwnName.Style.Font.Italic = true;
            this.txtOwnName.Style.Font.Name = "Verdana";
            this.txtOwnName.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(10, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtOwnName.Style.Font.Strikeout = false;
            this.txtOwnName.Style.Font.Underline = false;
            this.txtOwnName.Value = "SYSTEM NAME";
            // 
            // textBox5
            // 
            this.textBox5.Format = "{0:yyyy.MM.dd}";
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.2291665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.3333333432674408, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.6458332538604736, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox5.Style.Color = System.Drawing.Color.Black;
            this.textBox5.Style.Font.Bold = false;
            this.textBox5.Style.Font.Italic = true;
            this.textBox5.Style.Font.Name = "Times New Roman";
            this.textBox5.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(9, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox5.Style.Font.Strikeout = false;
            this.textBox5.Style.Font.Underline = false;
            this.textBox5.Value = "Registration Address ";
            // 
            // txtOwnAdress
            // 
            this.txtOwnAdress.Format = "{0:yyyy.MM.dd}";
            this.txtOwnAdress.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.2291665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.5, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtOwnAdress.Name = "txtOwnAdress";
            this.txtOwnAdress.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.6458332538604736, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.2916666567325592, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtOwnAdress.Style.Font.Bold = false;
            this.txtOwnAdress.Style.Font.Italic = true;
            this.txtOwnAdress.Style.Font.Name = "Times New Roman";
            this.txtOwnAdress.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(9, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtOwnAdress.Style.Font.Strikeout = false;
            this.txtOwnAdress.Style.Font.Underline = false;
            this.txtOwnAdress.Value = "SYSTEM ADDRESS";
            // 
            // textBox7
            // 
            this.textBox7.Format = "{0:yyyy.MM.dd}";
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.2291665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.79166668653488159, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.91666668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox7.Style.Color = System.Drawing.Color.Black;
            this.textBox7.Style.Font.Bold = false;
            this.textBox7.Style.Font.Italic = true;
            this.textBox7.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox7.Style.Font.Strikeout = false;
            this.textBox7.Style.Font.Underline = false;
            this.textBox7.Value = "Registration No. ";
            // 
            // txtOwnRegNmber
            // 
            this.txtOwnRegNmber.Format = "{0:yyyy.MM.dd}";
            this.txtOwnRegNmber.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(6.1458334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.79166668653488159, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtOwnRegNmber.Name = "txtOwnRegNmber";
            this.txtOwnRegNmber.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.7291666269302368, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtOwnRegNmber.Style.Font.Bold = false;
            this.txtOwnRegNmber.Style.Font.Italic = true;
            this.txtOwnRegNmber.Style.Font.Name = "Times New Roman";
            this.txtOwnRegNmber.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(9, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtOwnRegNmber.Style.Font.Strikeout = false;
            this.txtOwnRegNmber.Style.Font.Underline = false;
            this.txtOwnRegNmber.Value = "SYSTEM REG No";
            // 
            // txtCustomerFax
            // 
            this.txtCustomerFax.Format = "{0:yyyy.MM.dd}";
            this.txtCustomerFax.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.91666668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(2.4583332538604736, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomerFax.Name = "txtCustomerFax";
            this.txtCustomerFax.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(4.0729165077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.2083333283662796, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomerFax.Style.Font.Bold = false;
            this.txtCustomerFax.Style.Font.Italic = true;
            this.txtCustomerFax.Style.Font.Name = "Verdana";
            this.txtCustomerFax.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtCustomerFax.Style.Font.Strikeout = false;
            this.txtCustomerFax.Style.Font.Underline = false;
            this.txtCustomerFax.Value = "CUSTOMER FAX";
            // 
            // txtCustomerPhone
            // 
            this.txtCustomerPhone.Format = "{0:yyyy.MM.dd}";
            this.txtCustomerPhone.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.91666668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(2.2708332538604736, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomerPhone.Name = "txtCustomerPhone";
            this.txtCustomerPhone.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(4.0729165077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomerPhone.Style.Font.Bold = false;
            this.txtCustomerPhone.Style.Font.Italic = true;
            this.txtCustomerPhone.Style.Font.Name = "Verdana";
            this.txtCustomerPhone.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtCustomerPhone.Style.Font.Strikeout = false;
            this.txtCustomerPhone.Style.Font.Underline = false;
            this.txtCustomerPhone.Value = "CUSTOMER PHONE";
            // 
            // textBox19
            // 
            this.textBox19.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(2.2604167461395264, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.625, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox19.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.textBox19.Style.Color = System.Drawing.Color.Black;
            this.textBox19.Style.Font.Bold = true;
            this.textBox19.Style.Font.Italic = false;
            this.textBox19.Style.Font.Name = "Verdana";
            this.textBox19.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox19.Style.Font.Strikeout = false;
            this.textBox19.Style.Font.Underline = false;
            this.textBox19.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox19.Value = "Phone";
            // 
            // textBox17
            // 
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.5833333730697632, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.91666668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox17.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.textBox17.Style.Color = System.Drawing.Color.Black;
            this.textBox17.Style.Font.Bold = true;
            this.textBox17.Style.Font.Italic = false;
            this.textBox17.Style.Font.Name = "Verdana";
            this.textBox17.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox17.Style.Font.Strikeout = false;
            this.textBox17.Style.Font.Underline = false;
            this.textBox17.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox17.Value = "Customer";
            // 
            // txtCustomer
            // 
            this.txtCustomer.Format = "{0:yyyy.MM.dd}";
            this.txtCustomer.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.91666668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.5833333730697632, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(4.0729165077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomer.Style.Font.Bold = false;
            this.txtCustomer.Style.Font.Italic = true;
            this.txtCustomer.Style.Font.Name = "Verdana";
            this.txtCustomer.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtCustomer.Style.Font.Strikeout = false;
            this.txtCustomer.Style.Font.Underline = false;
            this.txtCustomer.Value = "CUSTOMER NAME";
            // 
            // textBox14
            // 
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.9270833730697632, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.91666668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox14.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.textBox14.Style.Color = System.Drawing.Color.Black;
            this.textBox14.Style.Font.Bold = true;
            this.textBox14.Style.Font.Italic = false;
            this.textBox14.Style.Font.Name = "Verdana";
            this.textBox14.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox14.Style.Font.Strikeout = false;
            this.textBox14.Style.Font.Underline = false;
            this.textBox14.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox14.Value = "Address";
            // 
            // txtCustomerAddress
            // 
            this.txtCustomerAddress.Format = "{0:yyyy.MM.dd}";
            this.txtCustomerAddress.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.91666668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.9270833730697632, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomerAddress.Name = "txtCustomerAddress";
            this.txtCustomerAddress.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(4.0729165077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.3333333432674408, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomerAddress.Style.Font.Bold = false;
            this.txtCustomerAddress.Style.Font.Italic = true;
            this.txtCustomerAddress.Style.Font.Name = "Verdana";
            this.txtCustomerAddress.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtCustomerAddress.Style.Font.Strikeout = false;
            this.txtCustomerAddress.Style.Font.Underline = false;
            this.txtCustomerAddress.Value = "CUSTOMER ADDRESS";
            // 
            // txtDueDate
            // 
            this.txtDueDate.Format = "{0:yyyy.MM.dd}";
            this.txtDueDate.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(6.1458334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(2.1666667461395264, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtDueDate.Name = "txtDueDate";
            this.txtDueDate.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.7291666269302368, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtDueDate.Style.Font.Bold = false;
            this.txtDueDate.Style.Font.Italic = true;
            this.txtDueDate.Style.Font.Name = "Verdana";
            this.txtDueDate.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtDueDate.Style.Font.Strikeout = false;
            this.txtDueDate.Style.Font.Underline = false;
            // 
            // textBox11
            // 
            this.textBox11.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.2291665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(2.1666667461395264, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.70833331346511841, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox11.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.textBox11.Style.Color = System.Drawing.Color.Black;
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.Font.Italic = false;
            this.textBox11.Style.Font.Name = "Verdana";
            this.textBox11.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox11.Style.Font.Strikeout = false;
            this.textBox11.Style.Font.Underline = false;
            this.textBox11.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox11.Value = "Due Date";
            // 
            // txtInvoiceDate
            // 
            this.txtInvoiceDate.Format = "{0:yyyy.MM.dd}";
            this.txtInvoiceDate.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(6.1458334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.9583333730697632, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtInvoiceDate.Name = "txtInvoiceDate";
            this.txtInvoiceDate.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.7291666269302368, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
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
            this.txtSerialNumber.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(6.1458334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.75, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtSerialNumber.Name = "txtSerialNumber";
            this.txtSerialNumber.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.7291666269302368, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtSerialNumber.Style.Font.Bold = false;
            this.txtSerialNumber.Style.Font.Italic = true;
            this.txtSerialNumber.Style.Font.Name = "Verdana";
            this.txtSerialNumber.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtSerialNumber.Style.Font.Strikeout = false;
            this.txtSerialNumber.Style.Font.Underline = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.2291665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.75, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.83333331346511841, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox2.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.textBox2.Style.Color = System.Drawing.Color.Black;
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Style.Font.Italic = false;
            this.textBox2.Style.Font.Name = "Verdana";
            this.textBox2.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox2.Style.Font.Strikeout = false;
            this.textBox2.Style.Font.Underline = false;
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox2.Value = "Invoice No";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.2291665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.9583333730697632, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.875, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.15625, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox1.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.textBox1.Style.Color = System.Drawing.Color.Black;
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Italic = false;
            this.textBox1.Style.Font.Name = "Verdana";
            this.textBox1.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox1.Style.Font.Strikeout = false;
            this.textBox1.Style.Font.Underline = false;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox1.Value = "Invoice Date";
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(3.25, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.375, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.3333333432674408, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox4.Style.Color = System.Drawing.Color.Black;
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.Style.Font.Italic = true;
            this.textBox4.Style.Font.Name = "Verdana";
            this.textBox4.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(18, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox4.Style.Font.Strikeout = false;
            this.textBox4.Style.Font.Underline = false;
            this.textBox4.Value = "Invoice";
            // 
            // textBox23
            // 
            this.textBox23.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(2.4583332538604736, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox23.Name = "textBox23";
            this.textBox23.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.54166668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox23.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.textBox23.Style.Color = System.Drawing.Color.Black;
            this.textBox23.Style.Font.Bold = true;
            this.textBox23.Style.Font.Italic = false;
            this.textBox23.Style.Font.Name = "Verdana";
            this.textBox23.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox23.Style.Font.Strikeout = false;
            this.textBox23.Style.Font.Underline = false;
            this.textBox23.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox23.Value = "Fax";
            // 
            // panel2
            // 
            this.panel2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox10,
            this.txtTotalAmount,
            this.txtTotalDuration,
            this.txtTotalCalls});
            this.panel2.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(3.0833334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(4.3000001907348633, Telerik.Reporting.Drawing.UnitType.Inch));
            this.panel2.Name = "panel2";
            this.panel2.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(4.8312497138977051, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.25, Telerik.Reporting.Drawing.UnitType.Inch));
            this.panel2.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.panel2.Style.Font.Bold = false;
            this.panel2.Style.Font.Italic = false;
            this.panel2.Style.Font.Strikeout = false;
            this.panel2.Style.Font.Underline = false;
            // 
            // textBox10
            // 
            this.textBox10.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.125, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.0416666679084301, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox10.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.textBox10.Style.Color = System.Drawing.Color.Black;
            this.textBox10.Style.Font.Bold = true;
            this.textBox10.Style.Font.Italic = false;
            this.textBox10.Style.Font.Name = "Verdana";
            this.textBox10.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox10.Style.Font.Strikeout = false;
            this.textBox10.Style.Font.Underline = false;
            this.textBox10.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.textBox10.Value = "Total Invoice";
            // 
            // txtTotalAmount
            // 
            this.txtTotalAmount.Format = "{0:#,##0.00}";
            this.txtTotalAmount.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(3.5166671276092529, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtTotalAmount.Name = "txtTotalAmount";
            this.txtTotalAmount.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.3145828247070313, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1979166716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtTotalAmount.Style.Font.Bold = true;
            this.txtTotalAmount.Style.Font.Italic = false;
            this.txtTotalAmount.Style.Font.Name = "Verdana";
            this.txtTotalAmount.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtTotalAmount.Style.Font.Strikeout = false;
            this.txtTotalAmount.Style.Font.Underline = false;
            this.txtTotalAmount.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtTotalAmount.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            // 
            // txtTotalDuration
            // 
            this.txtTotalDuration.Format = "{0:#,##0.00}";
            this.txtTotalDuration.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(2.5166671276092529, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtTotalDuration.Name = "txtTotalDuration";
            this.txtTotalDuration.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.2083333283662796, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtTotalDuration.Style.Font.Bold = true;
            this.txtTotalDuration.Style.Font.Italic = false;
            this.txtTotalDuration.Style.Font.Name = "Verdana";
            this.txtTotalDuration.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtTotalDuration.Style.Font.Strikeout = false;
            this.txtTotalDuration.Style.Font.Underline = false;
            this.txtTotalDuration.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtTotalDuration.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtTotalDuration.Value = "Duration";
            // 
            // txtTotalCalls
            // 
            this.txtTotalCalls.Format = "{0:#,##0}";
            this.txtTotalCalls.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(1.5, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.016271591186523438, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtTotalCalls.Name = "txtTotalCalls";
            this.txtTotalCalls.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.2083333283662796, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtTotalCalls.Style.Font.Bold = true;
            this.txtTotalCalls.Style.Font.Italic = false;
            this.txtTotalCalls.Style.Font.Name = "Verdana";
            this.txtTotalCalls.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtTotalCalls.Style.Font.Strikeout = false;
            this.txtTotalCalls.Style.Font.Underline = false;
            this.txtTotalCalls.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtTotalCalls.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.txtTotalCalls.Value = "NumberOfCalls";
            // 
            // textBox12
            // 
            this.textBox12.Format = "{0:yyyy.MM.dd}";
            this.textBox12.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.91666668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(4.7166666984558105, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.2916666269302368, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.2083333283662796, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox12.Style.Font.Bold = true;
            this.textBox12.Style.Font.Italic = false;
            this.textBox12.Style.Font.Name = "Verdana";
            this.textBox12.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox12.Style.Font.Strikeout = false;
            this.textBox12.Style.Font.Underline = false;
            this.textBox12.Value = "Wire Information";
            // 
            // shape1
            // 
            this.shape1.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.2083333283662796, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(4.966667652130127, Telerik.Reporting.Drawing.UnitType.Inch));
            this.shape1.Name = "shape1";
            this.shape1.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.EW);
            this.shape1.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.7083332538604736, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.0833333358168602, Telerik.Reporting.Drawing.UnitType.Inch));
            this.shape1.Style.Font.Bold = true;
            this.shape1.Style.Font.Italic = false;
            this.shape1.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.shape1.Style.Font.Strikeout = false;
            this.shape1.Style.Font.Underline = false;
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = new Telerik.Reporting.Drawing.Unit(9.3707551956176758, Telerik.Reporting.Drawing.UnitType.Inch);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.picBoxOwnLogo,
            this.txtOwnName,
            this.textBox5,
            this.txtOwnAdress,
            this.textBox7,
            this.txtOwnRegNmber,
            this.txtCustomerFax,
            this.txtCustomerPhone,
            this.textBox19,
            this.textBox17,
            this.txtCustomer,
            this.textBox14,
            this.txtCustomerAddress,
            this.txtDueDate,
            this.textBox11,
            this.txtInvoiceDate,
            this.txtSerialNumber,
            this.textBox2,
            this.textBox1,
            this.textBox4,
            this.textBox23,
            this.panel2,
            this.textBox12,
            this.shape1,
            this.txtBankingDetails,
            this.VatPanel,
            this.txtInvoiceNote,
            this.txtCustomerRegNumber,
            this.txtCustomerRegLabel,
            this.txtVatID,
            this.txtVatIDValue,
            this.textBox16,
            this.txtOwnVATID,
            this.SRinvoiceDetail,
            this.TableData});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            this.reportHeaderSection1.Style.Font.Bold = false;
            this.reportHeaderSection1.Style.Font.Italic = false;
            this.reportHeaderSection1.Style.Font.Strikeout = false;
            this.reportHeaderSection1.Style.Font.Underline = false;
            this.reportHeaderSection1.ItemDataBinding += new System.EventHandler(this.reportHeaderSection1_ItemDataBinding_1);
            // 
            // picBoxOwnLogo
            // 
            this.picBoxOwnLogo.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.0833333358168602, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.0833333358168602, Telerik.Reporting.Drawing.UnitType.Inch));
            this.picBoxOwnLogo.Name = "picBoxOwnLogo";
            this.picBoxOwnLogo.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.2083332538604736, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.70833331346511841, Telerik.Reporting.Drawing.UnitType.Inch));
            this.picBoxOwnLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.picBoxOwnLogo.Style.Font.Bold = false;
            this.picBoxOwnLogo.Style.Font.Italic = false;
            this.picBoxOwnLogo.Style.Font.Strikeout = false;
            this.picBoxOwnLogo.Style.Font.Underline = false;
            // 
            // txtBankingDetails
            // 
            this.txtBankingDetails.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.2083333283662796, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(5.0709133148193359, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtBankingDetails.Name = "txtBankingDetails";
            this.txtBankingDetails.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(7.4166665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.21033795177936554, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtBankingDetails.Style.Font.Bold = true;
            this.txtBankingDetails.Style.Font.Italic = false;
            this.txtBankingDetails.Style.Font.Name = "Verdana";
            this.txtBankingDetails.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtBankingDetails.Style.Font.Strikeout = false;
            this.txtBankingDetails.Style.Font.Underline = false;
            // 
            // VatPanel
            // 
            this.VatPanel.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtVatAmount,
            this.lblVat});
            this.VatPanel.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.0416666679084301, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(3.7000000476837158, Telerik.Reporting.Drawing.UnitType.Inch));
            this.VatPanel.Name = "VatPanel";
            this.VatPanel.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(7.8333334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.25, Telerik.Reporting.Drawing.UnitType.Inch));
            this.VatPanel.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.VatPanel.Style.Color = System.Drawing.Color.Black;
            this.VatPanel.Style.Font.Bold = false;
            this.VatPanel.Style.Font.Italic = false;
            this.VatPanel.Style.Font.Strikeout = false;
            this.VatPanel.Style.Font.Underline = false;
            // 
            // txtVatAmount
            // 
            this.txtVatAmount.Format = "{0:#,##0.00}";
            this.txtVatAmount.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(6.5583338737487793, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.031210580840706825, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtVatAmount.Name = "txtVatAmount";
            this.txtVatAmount.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.2729161977767944, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1979166716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtVatAmount.Style.Font.Bold = true;
            this.txtVatAmount.Style.Font.Italic = false;
            this.txtVatAmount.Style.Font.Name = "Verdana";
            this.txtVatAmount.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtVatAmount.Style.Font.Strikeout = false;
            this.txtVatAmount.Style.Font.Underline = false;
            this.txtVatAmount.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtVatAmount.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            // 
            // lblVat
            // 
            this.lblVat.Format = "{0:yyyy.MM.dd}";
            this.lblVat.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.2916666567325592, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.020793914794921875, Telerik.Reporting.Drawing.UnitType.Inch));
            this.lblVat.Name = "lblVat";
            this.lblVat.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.3333332538604736, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.2083333283662796, Telerik.Reporting.Drawing.UnitType.Inch));
            this.lblVat.Style.Color = System.Drawing.Color.Black;
            this.lblVat.Style.Font.Bold = true;
            this.lblVat.Style.Font.Italic = true;
            this.lblVat.Style.Font.Name = "Verdana";
            this.lblVat.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.lblVat.Style.Font.Strikeout = false;
            this.lblVat.Style.Font.Underline = false;
            this.lblVat.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.lblVat.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.lblVat.Value = "VAT";
            // 
            // txtInvoiceNote
            // 
            this.txtInvoiceNote.CanGrow = false;
            this.txtInvoiceNote.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.2083333283662796, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(6.2000002861022949, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtInvoiceNote.Name = "txtInvoiceNote";
            this.txtInvoiceNote.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(7.4583334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1.0000005960464478, Telerik.Reporting.Drawing.UnitType.Inch));
            // 
            // txtCustomerRegNumber
            // 
            this.txtCustomerRegNumber.Format = "{0:yyyy.MM.dd}";
            this.txtCustomerRegNumber.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.91666668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(2.66674542427063, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomerRegNumber.Name = "txtCustomerRegNumber";
            this.txtCustomerRegNumber.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.7291666269302368, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomerRegNumber.Style.Font.Bold = false;
            this.txtCustomerRegNumber.Style.Font.Italic = true;
            this.txtCustomerRegNumber.Style.Font.Name = "Times New Roman";
            this.txtCustomerRegNumber.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(9, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtCustomerRegNumber.Style.Font.Strikeout = false;
            this.txtCustomerRegNumber.Style.Font.Underline = false;
            this.txtCustomerRegNumber.Value = "Customer REG No";
            // 
            // txtCustomerRegLabel
            // 
            this.txtCustomerRegLabel.Format = "{0:yyyy.MM.dd}";
            this.txtCustomerRegLabel.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.010416666977107525, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(2.66674542427063, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomerRegLabel.Name = "txtCustomerRegLabel";
            this.txtCustomerRegLabel.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.78958338499069214, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtCustomerRegLabel.Style.Color = System.Drawing.Color.Black;
            this.txtCustomerRegLabel.Style.Font.Bold = true;
            this.txtCustomerRegLabel.Style.Font.Italic = false;
            this.txtCustomerRegLabel.Style.Font.Name = "Verdana";
            this.txtCustomerRegLabel.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtCustomerRegLabel.Style.Font.Strikeout = false;
            this.txtCustomerRegLabel.Style.Font.Underline = false;
            this.txtCustomerRegLabel.Value = "Reg No. ";
            // 
            // txtVatID
            // 
            this.txtVatID.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.010416666977107525, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(2.8999998569488525, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtVatID.Name = "txtVatID";
            this.txtVatID.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.800000011920929, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtVatID.Style.Color = System.Drawing.Color.Black;
            this.txtVatID.Style.Font.Bold = true;
            this.txtVatID.Style.Font.Italic = false;
            this.txtVatID.Style.Font.Name = "Verdana";
            this.txtVatID.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtVatID.Style.Font.Strikeout = false;
            this.txtVatID.Style.Font.Underline = false;
            this.txtVatID.Value = "Vat ID.";
            // 
            // txtVatIDValue
            // 
            this.txtVatIDValue.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.91666668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(2.8999998569488525, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtVatIDValue.Name = "txtVatIDValue";
            this.txtVatIDValue.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.7291666269302368, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtVatIDValue.Style.Font.Bold = false;
            this.txtVatIDValue.Style.Font.Italic = true;
            this.txtVatIDValue.Style.Font.Name = "Times New Roman";
            this.txtVatIDValue.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(9, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtVatIDValue.Style.Font.Strikeout = false;
            this.txtVatIDValue.Style.Font.Underline = false;
            // 
            // textBox16
            // 
            this.textBox16.Format = "{0:yyyy.MM.dd}";
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.2291665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(0.91666668653488159, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox16.Style.Color = System.Drawing.Color.Black;
            this.textBox16.Style.Font.Bold = false;
            this.textBox16.Style.Font.Italic = true;
            this.textBox16.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox16.Style.Font.Strikeout = false;
            this.textBox16.Style.Font.Underline = false;
            this.textBox16.Value = "VAT ID.";
            // 
            // txtOwnVATID
            // 
            this.txtOwnVATID.Format = "{0:yyyy.MM.dd}";
            this.txtOwnVATID.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(6.1458334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(1, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtOwnVATID.Name = "txtOwnVATID";
            this.txtOwnVATID.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.7270832061767578, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.1666666716337204, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtOwnVATID.Style.Font.Bold = false;
            this.txtOwnVATID.Style.Font.Italic = true;
            this.txtOwnVATID.Style.Font.Name = "Times New Roman";
            this.txtOwnVATID.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(9, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtOwnVATID.Style.Font.Strikeout = false;
            this.txtOwnVATID.Style.Font.Underline = false;
            // 
            // SRinvoiceDetail
            // 
            this.SRinvoiceDetail.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(8.40000057220459, Telerik.Reporting.Drawing.UnitType.Inch));
            this.SRinvoiceDetail.Name = "SRinvoiceDetail";
            this.SRinvoiceDetail.ReportSource = this.rptInVoiceDetail1;
            this.SRinvoiceDetail.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(7.9166665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.95833331346511841, Telerik.Reporting.Drawing.UnitType.Inch));
            this.SRinvoiceDetail.Style.Font.Bold = false;
            this.SRinvoiceDetail.Style.Font.Italic = false;
            this.SRinvoiceDetail.Style.Font.Strikeout = false;
            this.SRinvoiceDetail.Style.Font.Underline = false;
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = new Telerik.Reporting.Drawing.Unit(0.49791717529296875, Telerik.Reporting.Drawing.UnitType.Inch);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox35,
            this.shape2,
            this.txtFooter});
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.Font.Bold = false;
            this.pageFooterSection1.Style.Font.Italic = false;
            this.pageFooterSection1.Style.Font.Strikeout = false;
            this.pageFooterSection1.Style.Font.Underline = false;
            this.pageFooterSection1.ItemDataBinding += new System.EventHandler(this.pageFooterSection1_ItemDataBinding);
            // 
            // textBox35
            // 
            this.textBox35.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(6.8000006675720215, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.11249987035989761, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox35.Multiline = false;
            this.textBox35.Name = "textBox35";
            this.textBox35.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.0833333730697632, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.25, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox35.Style.Color = System.Drawing.Color.Black;
            this.textBox35.Style.Font.Bold = true;
            this.textBox35.Style.Font.Italic = false;
            this.textBox35.Style.Font.Name = "Verdana";
            this.textBox35.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox35.Style.Font.Strikeout = false;
            this.textBox35.Style.Font.Underline = false;
            this.textBox35.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox35.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.textBox35.StyleName = "PageInfo";
            this.textBox35.Value = "=PageNumber + \' of \' + PageCount";
            // 
            // shape2
            // 
            this.shape2.Dock = System.Windows.Forms.DockStyle.Top;
            this.shape2.Name = "shape2";
            this.shape2.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.EW);
            this.shape2.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(7.9277167320251465, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.0833333358168602, Telerik.Reporting.Drawing.UnitType.Inch));
            this.shape2.Style.Font.Bold = false;
            this.shape2.Style.Font.Italic = false;
            this.shape2.Style.Font.Strikeout = false;
            this.shape2.Style.Font.Underline = false;
            // 
            // txtFooter
            // 
            this.txtFooter.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.08341217041015625, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtFooter.Name = "txtFooter";
            this.txtFooter.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(7.9166665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.3604176938533783, Telerik.Reporting.Drawing.UnitType.Inch));
            this.txtFooter.Style.Color = System.Drawing.Color.Black;
            this.txtFooter.Style.Font.Bold = false;
            this.txtFooter.Style.Font.Italic = false;
            this.txtFooter.Style.Font.Name = "Verdana";
            this.txtFooter.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.txtFooter.Style.Font.Strikeout = false;
            this.txtFooter.Style.Font.Underline = false;
            this.txtFooter.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtFooter.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Top;
            this.txtFooter.StyleName = "PageInfo";
            // 
            // TableData
            // 
            this.TableData.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(new Telerik.Reporting.Drawing.Unit(2.144841194152832, Telerik.Reporting.Drawing.UnitType.Inch)));
            this.TableData.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(new Telerik.Reporting.Drawing.Unit(2.144841194152832, Telerik.Reporting.Drawing.UnitType.Inch)));
            this.TableData.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(new Telerik.Reporting.Drawing.Unit(2.144841194152832, Telerik.Reporting.Drawing.UnitType.Inch)));
            this.TableData.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(new Telerik.Reporting.Drawing.Unit(1.398809552192688, Telerik.Reporting.Drawing.UnitType.Inch)));
            this.TableData.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(new Telerik.Reporting.Drawing.Unit(0.26662731170654297, Telerik.Reporting.Drawing.UnitType.Inch)));
            this.TableData.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(new Telerik.Reporting.Drawing.Unit(0.26662731170654297, Telerik.Reporting.Drawing.UnitType.Inch)));
            this.TableData.Body.SetCellContent(1, 0, this.textBox20);
            this.TableData.Body.SetCellContent(1, 1, this.textBox21);
            this.TableData.Body.SetCellContent(1, 2, this.textBox22);
            this.TableData.Body.SetCellContent(1, 3, this.textBox25);
            this.TableData.Body.SetCellContent(0, 0, this.textBox3);
            this.TableData.Body.SetCellContent(0, 1, this.textBox15);
            this.TableData.Body.SetCellContent(0, 2, this.textBox8);
            this.TableData.Body.SetCellContent(0, 3, this.textBox9);
            tableGroup4.Name = "Group1";
            this.TableData.ColumnGroups.Add(tableGroup1);
            this.TableData.ColumnGroups.Add(tableGroup2);
            this.TableData.ColumnGroups.Add(tableGroup3);
            this.TableData.ColumnGroups.Add(tableGroup4);
            this.TableData.DataMember = "";
            this.TableData.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox20,
            this.textBox21,
            this.textBox22,
            this.textBox25,
            this.textBox3,
            this.textBox15,
            this.textBox8,
            this.textBox9});
            this.TableData.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.0634918212890625, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(3.2000000476837158, Telerik.Reporting.Drawing.UnitType.Inch));
            this.TableData.Name = "TableData";
            tableGroup6.Grouping.AddRange(new Telerik.Reporting.Data.Grouping[] {
            new Telerik.Reporting.Data.Grouping("")});
            tableGroup6.Name = "detailGroup";
            this.TableData.RowGroups.Add(tableGroup5);
            this.TableData.RowGroups.Add(tableGroup6);
            this.TableData.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(7.8333334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.53325462341308594, Telerik.Reporting.Drawing.UnitType.Inch));
            this.TableData.ItemDataBound += new System.EventHandler(this.TableData_ItemDataBound);
            // 
            // textBox20
            // 
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.144841194152832, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.26662731170654297, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox20.Style.Font.Bold = true;
            this.textBox20.Style.Font.Italic = true;
            this.textBox20.Style.Font.Name = "Verdana";
            this.textBox20.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox20.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox20.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox20.Value = "=Fields.FromDate + \" / \" + Fields.ToDate";
            // 
            // textBox21
            // 
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.144841194152832, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.26662731170654297, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox21.Style.Font.Bold = true;
            this.textBox21.Style.Font.Name = "Verdana";
            this.textBox21.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox21.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox21.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox21.Value = "=Fields.Calls";
            // 
            // textBox22
            // 
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.144841194152832, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.26662731170654297, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox22.Style.Font.Bold = true;
            this.textBox22.Style.Font.Name = "Verdana";
            this.textBox22.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox22.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox22.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox22.Value = "=Fields.Duration";
            // 
            // textBox25
            // 
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.398809552192688, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.26662731170654297, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox25.Style.Font.Bold = true;
            this.textBox25.Style.Font.Name = "Verdana";
            this.textBox25.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox25.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox25.Value = "=Fields.Amount";
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.85833340883255, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.0416666679084301, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.144841194152832, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.26662731170654297, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox3.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.textBox3.Style.Color = System.Drawing.Color.Black;
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Style.Font.Italic = false;
            this.textBox3.Style.Font.Name = "Verdana";
            this.textBox3.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox3.Style.Font.Strikeout = false;
            this.textBox3.Style.Font.Underline = false;
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.Value = "Billing Period";
            this.textBox3.ItemDataBound += new System.EventHandler(this.textBox3_ItemDataBound);
            // 
            // textBox15
            // 
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(4.3583335876464844, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.0416666679084301, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.144841194152832, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.26662731170654297, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox15.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.textBox15.Style.Color = System.Drawing.Color.Black;
            this.textBox15.Style.Font.Bold = true;
            this.textBox15.Style.Font.Italic = false;
            this.textBox15.Style.Font.Name = "Verdana";
            this.textBox15.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox15.Style.Font.Strikeout = false;
            this.textBox15.Style.Font.Underline = false;
            this.textBox15.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox15.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox15.Value = "Calls (successful)";
            // 
            // textBox8
            // 
            this.textBox8.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.5416665077209473, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.0416666679084301, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2.144841194152832, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.26662731170654297, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox8.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.textBox8.Style.Color = System.Drawing.Color.Black;
            this.textBox8.Style.Font.Bold = true;
            this.textBox8.Style.Font.Italic = false;
            this.textBox8.Style.Font.Name = "Verdana";
            this.textBox8.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox8.Style.Font.Strikeout = false;
            this.textBox8.Style.Font.Underline = false;
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox8.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox8.Value = "Duration (Min)";
            // 
            // textBox9
            // 
            this.textBox9.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(6.5833334922790527, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.0416666679084301, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(1.398809552192688, Telerik.Reporting.Drawing.UnitType.Inch), new Telerik.Reporting.Drawing.Unit(0.26662731170654297, Telerik.Reporting.Drawing.UnitType.Inch));
            this.textBox9.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.textBox9.Style.Color = System.Drawing.Color.Black;
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.Style.Font.Italic = false;
            this.textBox9.Style.Font.Name = "Verdana";
            this.textBox9.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8, Telerik.Reporting.Drawing.UnitType.Point);
            this.textBox9.Style.Font.Strikeout = false;
            this.textBox9.Style.Font.Underline = false;
            this.textBox9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox9.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox9.Value = "Amount ";
            // 
            // RptInvoice
            // 
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            this.group1});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.groupHeaderSection1,
            this.groupFooterSection1,
            this.detail,
            this.reportHeaderSection1,
            this.pageFooterSection1});
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins.Bottom = new Telerik.Reporting.Drawing.Unit(0.17000000178813934, Telerik.Reporting.Drawing.UnitType.Inch);
            this.PageSettings.Margins.Left = new Telerik.Reporting.Drawing.Unit(0.17000000178813934, Telerik.Reporting.Drawing.UnitType.Inch);
            this.PageSettings.Margins.Right = new Telerik.Reporting.Drawing.Unit(0.17000000178813934, Telerik.Reporting.Drawing.UnitType.Inch);
            this.PageSettings.Margins.Top = new Telerik.Reporting.Drawing.Unit(0.17000000178813934, Telerik.Reporting.Drawing.UnitType.Inch);
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Style.BackgroundColor = System.Drawing.Color.White;
            this.Style.Font.Bold = false;
            this.Style.Font.Italic = false;
            this.Style.Font.Strikeout = false;
            this.Style.Font.Underline = false;
            this.Width = new Telerik.Reporting.Drawing.Unit(7.9277167320251465, Telerik.Reporting.Drawing.UnitType.Inch);
            ((System.ComponentModel.ISupportInitialize)(this.rptInVoiceDetail1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DetailSection detail;
        private Group group1;
        private GroupFooterSection groupFooterSection1;
        private GroupHeaderSection groupHeaderSection1;
        private TextBox txtOwnName;
        private TextBox textBox5;
        private TextBox txtOwnAdress;
        private TextBox textBox7;
        private TextBox txtOwnRegNmber;
        private TextBox txtCustomerFax;
        private TextBox txtCustomerPhone;
        private TextBox textBox19;
        private TextBox textBox17;
        private TextBox txtCustomer;
        private TextBox textBox14;
        private TextBox txtCustomerAddress;
        private TextBox txtDueDate;
        private TextBox textBox11;
        private TextBox txtInvoiceDate;
        private TextBox txtSerialNumber;
        private TextBox textBox2;
        private TextBox textBox1;
        private TextBox textBox4;
        private TextBox textBox23;
        private Panel panel2;
        private TextBox textBox10;
        private TextBox textBox12;
        private Shape shape1;
        private ReportHeaderSection reportHeaderSection1;
        private TextBox txtTotalAmount;
        private TextBox txtTotalDuration;
        private PageFooterSection pageFooterSection1;
        private TextBox textBox35;
        private Shape shape2;
        private TextBox txtFooter;
        private TextBox txtBankingDetails;
        private Panel VatPanel;
        private TextBox txtVatAmount;
        private TextBox lblVat;
        private SubReport SRinvoiceDetail;
        private RptInVoiceDetail rptInVoiceDetail1;
        private PictureBox picBoxOwnLogo;
        private HtmlTextBox txtInvoiceNote;
        private TextBox txtTotalCalls;
        private TextBox txtCustomerRegNumber;
        private TextBox txtCustomerRegLabel;
        private TextBox txtVatIDValue;
        private TextBox txtVatID;
        private TextBox textBox16;
        private TextBox txtOwnVATID;
        private Table TableData;
        private TextBox textBox20;
        private TextBox textBox21;
        private TextBox textBox22;
        private TextBox textBox25;
        private TextBox textBox3;
        private TextBox textBox15;
        private TextBox textBox8;
        private TextBox textBox9;
    }
}