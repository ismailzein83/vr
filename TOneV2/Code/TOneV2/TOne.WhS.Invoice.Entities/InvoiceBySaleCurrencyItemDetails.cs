using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class InvoiceBySaleCurrencyItemDetails
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal AmountAfterCommission { get; set; }
        public decimal AmountAfterCommissionWithTaxes { get; set; }
        public decimal TotalTrafficAmount { get; set; }
        public decimal TotalRecurringChargeAmount { get; set; }
        public int NumberOfCalls { get; set; }
        public decimal Duration { get; set; }
        public int CurrencyId { get; set; }
        public decimal Amount { get; set; }

    }
}
