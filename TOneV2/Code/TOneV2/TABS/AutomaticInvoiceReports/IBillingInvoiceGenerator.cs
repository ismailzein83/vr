
namespace TABS.AutomaticInvoiceReports
{
    public interface IBillingInvoiceGenerator
    {
        Telerik.Reporting.IReportDocument GetBillingInvoice(TABS.Billing_Invoice Invoice);
    }
}
