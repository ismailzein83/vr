using System;
using System.Collections.Generic;

namespace Retail.QualityNet.Entities
{
    public class RDLCInvoiceItemDetails
    {
        public DateTime AttemptDateTime { get; set; }
        public string AttemptDateTimeDescription { get; set; }
        public String CallingNumber { get; set; }
        public string CallingNumberDescription { get; set; }
        public String CalledNumber { get; set; }
        public string CalledNumberDescription { get; set; }
        public Guid ServiceTypeId { get; set; }
        public string ServiceTypeIdDescription { get; set; }
        public Decimal DurationInSeconds { get; set; }
        public string DurationInSecondsDescription { get; set; }
        public int SaleCurrency { get; set; }
        public string SaleCurrencyIdDescription { get; set; }
        public Decimal SaleAmount { get; set; }
        public string SaleAmountDescription { get; set; }
        public string Country { get; set; }
        public string CountryDescription { get; set; }
        public string CountryInArabic { get; set; }
        public string CountryInArabicDescription { get; set; }
        public string TotalAmountInArabicWords { get; set; }
        public string TotalAmountInArabicWordsDescription { get; set; }
        public Decimal TotalAmount { get; set; }
        public int TotalNumberOfCalls { get; set; }
        public Decimal GrandTotalAmount { get; set; }

        public RDLCInvoiceItemDetails() { }

        public IEnumerable<RDLCInvoiceItemDetails> GetRDLCInvoiceItemDetailsSchema()
        {
            return null;
        }
    }
}
