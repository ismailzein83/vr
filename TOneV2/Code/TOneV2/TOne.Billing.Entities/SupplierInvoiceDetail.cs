using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Billing.Entities
{
    public class SupplierInvoiceDetail
    {
        public long DetailID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime TillDate { get; set; }
        public string Destination { get; set; }
        public int NumberOfCalls { get; set; }
        public decimal Duration { get; set; }
        public decimal Rate { get; set; }
        public string RateType { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyID { get; set; }
    }
}
