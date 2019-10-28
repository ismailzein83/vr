using System;

namespace Retail.QualityNet.Entities
{
    public class InvoiceDetails
    {
        public long SubscriberAccountId { get; set; }
        public int CurrencyId { get; set; }
        public int TotalNumberOfCalls { get; set; }
        public decimal TotalDurationInMin { get; set; }
        public decimal GrandTotalAmount { get; set; }
        public int InternationalTotalNumberOfCalls { get; set; }
        public decimal InternationalTotalDurationInMin { get; set; }
        public decimal InternationalGrandTotalAmount { get; set; }
    }
}