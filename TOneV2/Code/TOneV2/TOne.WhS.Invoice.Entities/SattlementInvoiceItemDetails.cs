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
}
