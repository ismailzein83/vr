using System;
using System.Collections.Generic;

namespace Retail.QualityNet.Entities
{
    public class RDLCInvoiceItemDetails
    {
        public DateTime AttemptDateTime { get; set; }
        public string AttemptDateTimeDescription { get; set; }
        public DateTime ConnectDateTime { get; set; }
        public string ConnectDateTimeDescription { get; set; }
        public String CallingNumber { get; set; }
        public string CallingNumberDescription { get; set; }
        public String CalledNumber { get; set; }
        public string CalledNumberDescription { get; set; }
        public Guid ServiceTypeId { get; set; }
        public string ServiceTypeIdDescription { get; set; }
        public int SaleCurrency { get; set; }
        public string SaleCurrencyIdDescription { get; set; }
        public int CountryId { get; set; }
        public string CountryIdDescription { get; set; }
        public int CountryInArabicId { get; set; }
        public string CountryInArabicIdDescription { get; set; }

        public Decimal DurationInSeconds { get; set; }
        public Decimal SaleAmount { get; set; }
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
