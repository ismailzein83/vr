using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class BillingInvoiceDetail
    {
        public decimal TotalAmount { get; set; }
        public decimal TotalRecurringCharges { get; set; }
        public decimal TotalActivationCharges { get; set; }
        public decimal TotalSuspensionCharges { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? TotalAmountAfterTaxes { get; set; }
        public decimal VatPercentage { get; set; }
        public int Currency { get; set; }
        public BillingInvoiceDetail()
        {

        }
        public IEnumerable<BillingInvoiceDetail> GetInvoiceDetailsRDLCSchema()
        {
            return null;
        }

    }
}
