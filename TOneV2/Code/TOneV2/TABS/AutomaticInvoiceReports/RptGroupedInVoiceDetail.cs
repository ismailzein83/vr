namespace TABS.AutomaticInvoiceReports
{
    using System.Drawing;
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for RptInVoiceDetail.
    /// </summary>
    public partial class RptGroupedInVoiceDetail : Report
    {
        public RptGroupedInVoiceDetail()
        {
            /// <summary>
            /// Required for telerik Reporting designer support
            /// </summary>
            InitializeComponent();
            //txtRateMin.Format = "{0:#,##0." + TABS.SystemConfiguration.GetRateFormat() + "}";
        }

        protected TABS.Billing_Invoice _BillingInvoice;
        public TABS.Billing_Invoice BillingInvoice
        {
            get { return _BillingInvoice; }
            set { _BillingInvoice = value; }
        }

        private void fillData()
        {
            TABS.CarrierAccount OurAccount = TABS.CarrierAccount.SYSTEM;
            if (OurAccount.CarrierProfile.CompanyLogo != null)
                piccompanylogo1.Value = Image.FromStream(new System.IO.MemoryStream(OurAccount.CarrierProfile.CompanyLogo));
            txtCustomerGroup.Value = BillingInvoice.Customer.CarrierProfile.CompanyName != null ? BillingInvoice.Customer.CarrierProfile.CompanyName : "";
            txtInvoiceDate.Value = BillingInvoice.IssueDate.ToString("dd/MM/yyyy");
            txtSerialNumber.Value = BillingInvoice.SerialNumber.ToString();
            txtAmount.Value = "Amount ";// +BillingInvoice.Currency.Symbol;
            txtRate.Value = "Rate/Min ";// +BillingInvoice.Currency.Symbol;
            txtRateMin.Format = "{0:0." + TABS.SystemConfiguration.GetRateFormat() + "}";
        }

        private void RptInVoiceDetail_ItemDataBinding(object sender, System.EventArgs e)
        {
            fillData();
        }
    }
}