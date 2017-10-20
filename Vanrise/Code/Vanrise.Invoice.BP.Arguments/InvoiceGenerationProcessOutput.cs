using System;
using System.Text;

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
                    StringBuilder strBuilder = new StringBuilder("Invoice generation completed");
                    if (TotalSucceeded > 0)
                        strBuilder.AppendFormat(", {0} Invoices successfully generated", TotalSucceeded);

                    if (totalFailed > 0)
                        strBuilder.AppendFormat(", {0} Invoices failed", totalFailed);

                    return strBuilder.ToString();
                }
            }
        }
    }
}
