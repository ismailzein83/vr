namespace TABS.AutomaticInvoiceReports
{
    using System.Drawing;
    using Telerik.Reporting;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Summary description for RptCarrierDailySummary.
    /// </summary>
    public partial class RptInvoice : Report
    {
        public RptInvoice(TABS.Billing_Invoice invoice)
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
            TABS.CarrierAccount OurAccount = Customer.CustomerMaskAccount; //TABS.CarrierAccount.SYSTEM;
            txtOwnVATID.Value = string.IsNullOrEmpty(OurAccount.CarrierProfile.VatID) ? string.Empty : OurAccount.CarrierProfile.VatID;
            txtBillingPeriod.Value = string.Format("{0} - {1}", BillingInvoice.BeginDate.ToString("dd/MM/yyyy"), BillingInvoice.EndDate.ToString("dd/MM/yyyy"));
            txtDueDate.Value = BillingInvoice.DueDate.ToString("dd/MM/yyyy");
            txtInvoiceDate.Value = BillingInvoice.IssueDate.ToString("dd/MM/yyyy");
            txtSerialNumber.Value = BillingInvoice.SerialNumber.ToString();

            txtOwnName.Value = OurAccount.CarrierProfile.CompanyName != null ? OurAccount.CarrierProfile.CompanyName : "";
            txtOwnAdress.Value = OurAccount.CarrierProfile.Address != null ? OurAccount.CarrierProfile.Address : "";
            txtOwnRegNmber.Value = OurAccount.CarrierProfile.RegistrationNumber != null ? OurAccount.CarrierProfile.RegistrationNumber : "";
            if (OurAccount.CarrierProfile.CompanyLogo != null)
                picBoxOwnLogo.Value = Image.FromStream(new System.IO.MemoryStream(OurAccount.CarrierProfile.CompanyLogo));

            txtCustomer.Value = Customer.CompanyNameForPricelistAndInvoice;//CarrierProfile.CompanyName != null ? Customer.CarrierProfile.CompanyName : "";
            txtCustomerAddress.Value = Customer.CarrierProfile.Address != null ? Customer.CarrierProfile.Address : "";
            txtCustomerPhone.Value = Customer.CarrierProfile.Telephone != null ? Customer.CarrierProfile.Telephone : "";
            txtCustomerFax.Value = Customer.CarrierProfile.Fax != null ? Customer.CarrierProfile.Fax : "";

            txtBillingDuration.Value = BillingInvoice.Duration.ToString("#,##0.00");
            txtCalls.Value = BillingInvoice.NumberOfCalls.ToString("#,##0");

            //lblVat.Value = string.Format("VAT ({0:#.00} %)", BillingInvoice.VatValue);
            lblVat.Value = string.Format(@"VAT: {0:#.00}%", BillingInvoice.Customer.CarrierProfile.VAT.ToString());

            if (Customer.CarrierProfile.VatID != null) { txtVatID.Visible = true; txtVatIDValue.Visible = true; txtVatIDValue.Value = Customer.CarrierProfile.VatID; }
            else { txtVatID.Visible = false; txtVatIDValue.Visible = false; }



            string CustomerCurrency = Customer.CarrierProfile.Currency.Symbol;

            List<TABS.SpecialSystemParameters.BankingDetails> BankDetails = TABS.SpecialSystemParameters.BankingDetails.Get(TABS.SystemParameter.BankingDetails);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();


            TABS.CarrierAccount virtualCustomer = OurAccount.Equals(TABS.CarrierAccount.SYSTEM) ? Customer :
                (string.IsNullOrEmpty(OurAccount.BankReferences) ? Customer : OurAccount);


            if (string.IsNullOrEmpty(virtualCustomer.BankReferences))
            {
                foreach (TABS.SpecialSystemParameters.BankingDetails bankdetail in BankDetails)
                {
                    if (bankdetail.CurrencyID.Equals(CustomerCurrency))
                    {
                        GetDefinitionString(sb, bankdetail);

                    }
                }
            }
            else
            {
                string[] bankReferences = virtualCustomer.BankReferences.Split(',');

                foreach (TABS.SpecialSystemParameters.BankingDetails bankdetail in BankDetails)
                {
                    for (int i = 0; i < bankReferences.Length; i++)
                    {
                        if (bankdetail.GUID.Equals(bankReferences[i]))
                        {
                            GetDefinitionString(sb, bankdetail);
                        }
                    }
                }
            }

            //customer registration number
            if (!string.IsNullOrEmpty(Customer.CarrierProfile.RegistrationNumber) && TABS.SystemParameter.IncludeCustomerRegistrationNumberInInvoice.BooleanValue.Value)
            {
                txtCustomerRegLabel.Visible = true;
                txtCustomerRegNumber.Visible = true;
                txtCustomerRegNumber.Value = Customer.CarrierProfile.RegistrationNumber;
            }
            else
            {
                txtCustomerRegLabel.Visible = false;
                txtCustomerRegNumber.Visible = false;
            }

            txtBankingDetails.Value = sb.ToString();
            //short gmtDifference = TABS.CarrierAccount.SYSTEM.GMTTime;
            //if (gmtDifference == 0) txtBillingTime.Value = "GMT Zone";
            //if (gmtDifference > 0) txtBillingTime.Value = string.Format("GMT Zone +{0}", gmtDifference);
            //if (gmtDifference < 0) txtBillingTime.Value = string.Format("GMT Zone {0}", gmtDifference);

            txtBillingTime.Value = BillingInvoice.InvoiceNotes != null ? BillingInvoice.InvoiceNotes : "";

            txtBillingAmount.Value = BillingInvoice.Amount.ToString("#,##0.00");
            txtCurrency.Value = BillingInvoice.Currency.Symbol;
            txtTotalDuration.Value = txtBillingDuration.Value;
            txtTotalCalls.Value = txtCalls.Value;

            decimal totalAmount = BillingInvoice.Amount;

            // include VAT            
            decimal vatAmount = BillingInvoice.VatValue.HasValue ? totalAmount * BillingInvoice.VatValue.Value / 100.0m : 0m; //totalAmount * Customer.CarrierProfile.VAT / 100.0m;
            txtVatAmount.Value = vatAmount.ToString("#,##0.00");
            totalAmount += vatAmount;

            txtTotalAmount.Value = totalAmount.ToString("#,##0.00");


            // Billing invoice printed note

            txtInvoiceNote.Value = BillingInvoice.InvoicePrintedNote != null ? BillingInvoice.InvoicePrintedNote : "";
        }

        private void GetDefinitionString(System.Text.StringBuilder sb, TABS.SpecialSystemParameters.BankingDetails bankdetail)
        {
            System.Text.StringBuilder DefintionBuilder = new System.Text.StringBuilder();

            string[] Definitions = bankdetail.DefinitionDisplay.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string def in Definitions)
            {
                if (def.StartsWith("Correspondent"))
                {
                    if (BillingInvoice.Currency.Symbol == "USD")
                    {
                        DefintionBuilder.AppendFormat("{0}\r\n", def);
                    }
                }
                else
                {
                    DefintionBuilder.AppendFormat("{0}\r\n", def);
                }
            }
            sb.AppendFormat("{0}\r\n", DefintionBuilder.ToString());
        }

        private void pageFooterSection1_ItemDataBinding(object sender, EventArgs e)
        {
            txtFooter.Value = TABS.CarrierAccount.SYSTEM.CarrierProfile.Address != null ? TABS.CarrierAccount.SYSTEM.CarrierProfile.Address : "";
        }



    }
}