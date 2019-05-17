using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Interconnect.Entities
{
    public class SettlementInvoiceDetailSummary
    {
        public int CurrencyId { get; set; }
        public string CurrencyIdDescription { get; set; }
        public decimal Amount { get; set; }

        public SettlementInvoiceDetailSummary() { }
        public IEnumerable<SettlementInvoiceDetailSummary> GetRDLCSettlementInvoiceDetailSummarySchema()
        {
            return null;
        }
    }
    public class SettlementInvoiceDetailByCurrency
    {
        public long InvoiceId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyIdDescription { get; set; }
        public decimal Amount { get; set; }

        public decimal OriginalAmount { get; set; }
        public int NumberOfCalls { get; set; }
        public decimal TotalDuration { get; set; }
        public decimal TotalTrafficAmount { get; set; }
        public decimal TotalSMSAmount { get; set; }
        public decimal TotalRecurringChargeAmount { get; set; }
        public decimal TotalFullAmount { get; set; }
    }
}
