using System;
using System.Collections.Generic;

namespace Retail.Interconnect.Entities
{
    public class SettlementInvoiceItemDetailByCurrency
    {
        public long InvoiceId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyIdDescription { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Month { get; set; }
        public string MonthDescription { get; set; }

        public decimal DueToSystemAmount { get; set; }
        public decimal DueToSystemAmountWithTaxes { get; set; }
        public decimal DueToSystemAmountRecurringCharges { get; set; }
        public decimal DueToSystemTotalTrafficAmount { get; set; }
        public decimal DueToSystemTotalDealAmount { get; set; }
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

        public SettlementInvoiceItemDetailByCurrency()
        {
        }

        public IEnumerable<SettlementInvoiceItemDetailByCurrency> GetSettlementInvoiceItemDetailByCurrencyRDLCSchema()
        {
            return null;
        }
    }
}