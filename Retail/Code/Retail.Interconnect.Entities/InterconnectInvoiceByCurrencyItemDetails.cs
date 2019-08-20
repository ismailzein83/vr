using System;
using System.Collections.Generic;

namespace Retail.Interconnect.Entities
{
    public class InterconnectInvoiceByCurrencyItemDetails
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountWithTaxes { get; set; }
        public decimal SMSAmount { get; set; }
        public decimal SMSAmountWithTaxes { get; set; }
        public decimal TotalSMSAmount { get; set; }
        public decimal TotalTrafficAmount { get; set; }
        public decimal TotalRecurringChargeAmount { get; set; }
        public decimal TotalFullAmount { get; set; }
        public int NumberOfCalls { get; set; }
        public int NumberOfSMS { get; set; }
        public decimal Duration { get; set; }
        public int CurrencyId { get; set; }
        public string Month { get; set; }
    }

    public class InterconnectInvoiceByCurrencyItemDetailsByCurrency
    {
        public HashSet<int> Currencies { get; set; }
        public Dictionary<int, List<InterconnectInvoiceByCurrencyItemDetails>> MonthsByCurrency { get; set; }
    }
}