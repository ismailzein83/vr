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
        public Decimal Amount { get; set; }
        public int TotalNumberOfCalls { get; set; }
        public int CurrencyId { get; set; }
        public Decimal DurationInSeconds { get; set; }
        public decimal AmountWithCommission { get; set; }
        public decimal? Commission { get; set; }
      
        public SattlementInvoiceItemDetails() { }
        public IEnumerable<SattlementInvoiceItemDetails> GetSattlementInvoiceItemDetailsRDLCSchema()
        {
            return null;
        }
    }
}
