using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Billing.Entities
{
    public class SupplierInvoiceDetailGroupedByDay
    {
        public String Day { get; set; }
        public decimal DurationInMinutes { get; set; }
        public float Amount { get; set; }
        public string CurrencyID { get; set; }
    }
}
