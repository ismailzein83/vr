using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class SettlementInvoiceItemSummaryDetail
    {

        public int CurrencyId { get; set; }
        public string CurrencyIdDescription { get; set; }

        public decimal DueToSystemAmount { get; set; }
        public decimal DueToSystemAmountAfterCommission { get; set; }
        public decimal DueToSystemAmountAfterCommissionWithTaxes { get; set; }

        public decimal DueToCarrierAmount { get; set; }
        public decimal DueToCarrierAmountAfterCommission { get; set; }
        public decimal DueToCarrierAmountAfterCommissionWithTaxes { get; set; }

        public int DueToSystemNumberOfCalls { get; set; }
        public int DueToCarrierNumberOfCalls { get; set; }

        public SettlementInvoiceItemSummaryDetail() { }
        public IEnumerable<SettlementInvoiceItemSummaryDetail> GetRDLCSettlementInvoiceDetailSchema()
        {
            return null;
        }
    }
}
