using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Invoice.Entities
{
    public class InvoiceBySaleCurrencyItemDetails
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int NumberOfCalls { get; set; }
        public decimal Duration { get; set; }
        public int CurrencyId { get; set; }
        public decimal Amount { get; set; }
    }
}
