namespace TABS.AutomaticInvoiceReports
{
    using System.Drawing;
    using Telerik.Reporting;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Summary description for RptCarrierDailySummary.
    /// </summary>
    public partial class RptGroupedInvoice : Report
    {
        public RptGroupedInvoice(TABS.Billing_Invoice invoice)
        {
            /// <summary>
            /// Required for telerik Reporting designer support
            /// </summary>
            InitializeComponent();

            this.BillingInvoice = invoice;
            if (invoice.Customer.CarrierProfile.VAT == 0) this.VatPanel.Parent.Items.Remove(this.VatPanel);
            DisplayHeader();
        }

        public TABS.Billing_Invoice BillingInvoice { get; set; }
        public TABS.CarrierAccount Customer { get { return this.BillingInvoice.Customer; } }

        public SubReport InvoiceDetails
        {
            get { return SRinvoiceDetail; }
        }

        private void reportHeaderSection1_ItemDataBinding_1(object sender, System.EventArgs e)
        {
            //DisplayHeader();
        }

        private void DisplayHeader()
        {
            TABS.CarrierAccount OurAccount = TABS.CarrierAccount.SYSTEM;

            //txtBillingPeriod.Value = string.Format("{0} - {1}", BillingInvoice.BeginDate.ToString("dd/MM/yyyy"), BillingInvoice.EndDate.ToString("dd/MM/yyyy"));
            txtDueDate.Value = BillingInvoice.DueDate.ToString("dd/MM/yyyy");
            txtInvoiceDate.Value = BillingInvoice.IssueDate.ToString("dd/MM/yyyy");
            txtSerialNumber.Value = BillingInvoice.SerialNumber.ToString();

            txtOwnName.Value = OurAccount.CarrierProfile.CompanyName != null ? OurAccount.CarrierProfile.CompanyName : "";
            txtOwnAdress.Value = OurAccount.CarrierProfile.Address != null ? OurAccount.CarrierProfile.Address : "";
            txtOwnRegNmber.Value = OurAccount.CarrierProfile.RegistrationNumber != null ? OurAccount.CarrierProfile.RegistrationNumber : "";
            if (OurAccount.CarrierProfile.CompanyLogo != null)
                picBoxOwnLogo.Value = Image.FromStream(new System.IO.MemoryStream(OurAccount.CarrierProfile.CompanyLogo));

            txtCustomer.Value = Customer.CompanyNameForPricelistAndInvoice;//CarrierProfile.CompanyName != null ? Customer.CarrierProfile.CompanyName : "";
            txtCustomerAccountNo.Value = Customer.CarrierProfile.Telephone != null ? Customer.CarrierProfile.Telephone : "";
            txtCustomerAddress.Value = Customer.CarrierProfile.Address != null ? Customer.CarrierProfile.Address : "";
            txtCustomerPhone.Value = Customer.CarrierProfile.Telephone != null ? Customer.CarrierProfile.Telephone : "";
            txtCustomerFax.Value = Customer.CarrierProfile.Fax != null ? Customer.CarrierProfile.Fax : "";

            //txtBillingDuration.Value = BillingInvoice.Duration.ToString("#,##0.00");
            //txtCalls.Value = BillingInvoice.NumberOfCalls.ToString("#,##0.00");

            var CustomerCurrencies = BillingInvoice.Billing_Invoice_Details.Select(b => b.Currency.Symbol).Distinct().ToArray();

            List<TABS.SpecialSystemParameters.BankingDetails> BankDetails = TABS.SpecialSystemParameters.BankingDetails.Get(TABS.SystemParameter.BankingDetails);


            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (string.IsNullOrEmpty(Customer.BankReferences))
            {
                foreach (TABS.SpecialSystemParameters.BankingDetails bankdetail in BankDetails)
                {
                    if (CustomerCurrencies.Contains(bankdetail.CurrencyID))
                        sb.AppendFormat("{0}\r\n", bankdetail.DefinitionDisplay);
                }
            }
            else
            {
                string[] bankReferences = Customer.BankReferences.Split(',');

                foreach (TABS.SpecialSystemParameters.BankingDetails bankdetail in BankDetails)
                {
                    for (int i = 0; i < bankReferences.Length; i++)
                    {
                        if (bankdetail.GUID.Equals(bankReferences[i]))
                            sb.AppendFormat("{0}\r\n", bankdetail.DefinitionDisplay);
                    }
                }
            }

            txtBankingDetails.Value = sb.ToString();
            //short gmtDifference = TABS.CarrierAccount.SYSTEM.GMTTime;
            //if (gmtDifference == 0) txtBillingTime.Value = "GMT Zone";
            //if (gmtDifference > 0) txtBillingTime.Value = string.Format("GMT Zone +{0}", gmtDifference);
            //if (gmtDifference < 0) txtBillingTime.Value = string.Format("GMT Zone {0}", gmtDifference);

            //txtBillingTime.Value = BillingInvoice.InvoiceNotes != null ? BillingInvoice.InvoiceNotes : "";

            //txtBillingAmount.Value = BillingInvoice.Amount.ToString("#,##0.00");
            //txtCurrency.Value = BillingInvoice.Currency.Symbol;
            //txtTotalDuration.Value = txtBillingDuration.Value;
            //txtTotalCalls.Value = txtCalls.Value;

            decimal totalAmount = BillingInvoice.Amount;

            // include VAT            
            decimal vatAmount = totalAmount * Customer.CarrierProfile.VAT / 100.0m;
            txtVatAmount.Value = vatAmount.ToString("#,##0.00");
            totalAmount += vatAmount;

            //txtTotalAmount.Value = totalAmount.ToString("#,##0.00");


            // Billing invoice printed note

            txtInvoiceNote.Value = BillingInvoice.InvoicePrintedNote != null ? BillingInvoice.InvoicePrintedNote : "";
            BindTable();
        }

        protected void BindTable()
        {
            var groupedInvoice = BillingInvoice.Billing_Invoice_Details
                        .GroupBy(d => d.Currency)
                        .Select(g =>
                            new
                            {
                                Curr = g.Key.Symbol,
                                Period = string.Format("{0:yyyy-MM-dd} / {1:yyyy-MM-dd}", BillingInvoice.BeginDate, BillingInvoice.EndDate),
                                Timeshift = BillingInvoice.InvoiceNotes != null ? BillingInvoice.InvoiceNotes : "",
                                Calls = string.Format("{0:#,##0.00}", g.Sum(p => p.NumberOfCalls)),
                                Duration = string.Format("{0:#,##0.00}", g.Sum(p => p.Duration)),
                                Amount = string.Format("{0:#,##0.00}", g.Sum(p => p.Amount))
                            }
                            ).ToList();

            TableData.DataSource = groupedInvoice;

        }

        private void pageFooterSection1_ItemDataBinding(object sender, EventArgs e)
        {
            txtFooter.Value = TABS.CarrierAccount.SYSTEM.CarrierProfile.Address != null ? TABS.CarrierAccount.SYSTEM.CarrierProfile.Address : "";
        }

        private void textBox3_ItemDataBound(object sender, EventArgs e)
        {

        }

        private void TableData_ItemDataBound(object sender, EventArgs e)
        {
            //    textBox21.Format = string.IsNullOrEmpty(textBox21.Value) ? "" : string.Format("{0:#,##0.00}", decimal.Parse(textBox21.Value));
            //    textBox22.Value = string.IsNullOrEmpty(textBox22.Value) ? "" : string.Format("{0:#,##0.00}", decimal.Parse(textBox22.Value));
            //    textBox25.Value = string.IsNullOrEmpty(textBox25.Value) ? "" : string.Format("{0:#,##0.00}", decimal.Parse(textBox25.Value));
        }



    }
}