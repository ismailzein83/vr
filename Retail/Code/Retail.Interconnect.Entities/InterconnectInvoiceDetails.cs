using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.Interconnect.Entities
{
    public class InterconnectInvoiceDetails
    {
        public decimal Amount { get; set; }
        public decimal AmountWithTaxes { get; set; }

        public decimal Duration { get; set; }
        public int InterconnectCurrencyId { get; set; }
        public string InterconnectCurrency { get; set; }
        public int TotalNumberOfCalls { get; set; }
        public InterconnectInvoiceDetails() { }
        public IEnumerable<InterconnectInvoiceDetails> GetInterconnectInvoiceDetailsRDLCSchema()
        {
            return null;
        }
    }
}

