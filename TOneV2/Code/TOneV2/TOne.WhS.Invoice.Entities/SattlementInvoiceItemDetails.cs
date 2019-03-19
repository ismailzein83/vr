using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class SattlementInvoiceItemDetails
    {
        public Guid InvoiceTypeId { get; set; }
        public long InvoiceId { get; set; }
        public Decimal? Amount { get; set; }
        public int TotalNumberOfCalls { get; set; }
        public int? CurrencyId { get; set; }
        public Decimal DurationInSeconds { get; set; }
        public decimal? AmountWithCommission { get; set; }
        public decimal? Commission { get; set; }


        public string SerialNumber { get; set; }
        public DateTime IssueDate { get; set; }


        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? TimeZoneId { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? OriginalAmount { get; set; }
        public bool MultipleCurrencies { get; set; }

        public SattlementInvoiceItemDetails() { }
        public IEnumerable<SattlementInvoiceItemDetails> GetSattlementInvoiceItemDetailsRDLCSchema()
        {
            return null;
        }
    }
    public class SettlementInvoiceItemDetail
    {
        public long InvoiceId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyIdDescription { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public decimal DueToSystemAmount { get; set; }
        public decimal DueToSystemAmountAfterCommission { get; set; }
        public decimal DueToSystemAmountAfterCommissionWithTaxes { get; set; }
        public decimal DueToSystemAmountRecurringCharges { get; set; }
        public decimal DueToSystemTotalTrafficAmount { get; set; }
        public decimal DueToSystemTotalDealAmount { get; set; }
        public decimal DueToSystemTotalSMSAmount { get; set; }


        public decimal DueToCarrierAmount { get; set; }
        public decimal DueToCarrierAmountAfterCommission { get; set; }
        public decimal DueToCarrierAmountAfterCommissionWithTaxes { get; set; }
        public decimal DueToCarrierAmountRecurringCharges { get; set; }
        public decimal DueToCarrierTotalTrafficAmount { get; set; }
        public decimal DueToCarrierTotalDealAmount { get; set; }
        public decimal DueToCarrierTotalSMSAmount { get; set; }

        public int DueToSystemNumberOfCalls { get; set; }
        public int DueToCarrierNumberOfCalls { get; set; }

        public decimal DueToSystemDifference { get; set; }
        public decimal DueToCarrierDifference { get; set; }
        public string Month { get; set; }
        public string MonthDescription { get; set; }

        public SettlementInvoiceItemDetail() { }
        public IEnumerable<SettlementInvoiceItemDetail> GetSettlementInvoiceItemDetailRDLCSchema()
        {
            return null;
        }
    }
}
