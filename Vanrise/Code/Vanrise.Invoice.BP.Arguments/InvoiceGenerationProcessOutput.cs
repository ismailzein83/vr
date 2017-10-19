using System;

namespace Vanrise.Invoice.BP.Arguments
{
    public class InvoiceGenerationProcessOutput
    {
        public int TotalInvoices { get; set; }
        public int TotalSucceeded { get; set; }

        public string Message
        {
            get
            {
                if (TotalInvoices == 0)
                    return "No invoices were found";
                else
                {
                    int totalFailed = TotalInvoices - TotalSucceeded;
                    return string.Format("Number of successful invoices: {0}, Number of failed invoices: {1}", TotalSucceeded, totalFailed);
                }
            }
        }
    }
}
