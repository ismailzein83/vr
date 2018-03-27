using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class SettlementInvoiceDetailSummary
    {
        public int CurrencyId { get; set; }
        public string CurrencyIdDescription { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountWithCommission { get; set; }

        public SettlementInvoiceDetailSummary() { }
        public IEnumerable<SettlementInvoiceDetailSummary> GetRDLCSettlementInvoiceDetailSummarySchema()
        {
            return null;
        }
    }
}
