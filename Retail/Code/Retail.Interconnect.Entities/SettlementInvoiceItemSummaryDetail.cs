using System.Collections.Generic;

namespace Retail.Interconnect.Entities
{
    public class SettlementInvoiceItemSummaryDetail
    {
        public int CurrencyId { get; set; }
        public string CurrencyIdDescription { get; set; }

        public decimal DueToSystemAmount { get; set; }
        public decimal DueToSystemAmountWithTaxes { get; set; }
        public decimal DueToSystemAmountRecurringCharges { get; set; }
        public decimal DueToSystemTotalTrafficAmount { get; set; }
        public decimal DueToSystemTotalSMSAmount { get; set; }
        public decimal DueToSystemFullAmount { get; set; }
        public int DueToSystemNumberOfCalls { get; set; }
        public decimal DueToSystemDifference { get; set; }

        public decimal DueToCompanyAmount { get; set; }
        public decimal DueToCompanyAmountWithTaxes { get; set; }
        public decimal DueToCompanyAmountRecurringCharges { get; set; }
        public decimal DueToCompanyTotalTrafficAmount { get; set; }
        public decimal DueToCompanyTotalSMSAmount { get; set; }
        public decimal DueToCompanyFullAmount { get; set; }
        public int DueToCompanyNumberOfCalls { get; set; }
        public decimal DueToCompanyDifference { get; set; }


        public SettlementInvoiceItemSummaryDetail()
        {
        }

        public IEnumerable<SettlementInvoiceItemSummaryDetail> GetRDLCSettlementInvoiceDetailSchema()
        {
            return null;
        }
    }
}
