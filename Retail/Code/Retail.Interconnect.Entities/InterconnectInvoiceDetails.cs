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
        public decimal CostAmount { get; set; }
        public decimal Duration { get; set; }
        public int InterconnectCurrencyId { get; set; }
        public string InterconnectCurrency { get; set; }
        public Decimal TotalAmount { get; set; }
        public int? TimeZoneId { get; set; }
        public CommissionType? CommissionType { get; set; }

        public decimal? Commission { get; set; }
        public bool DisplayComission { get; set; }
        public string Offset { get; set; }

        public InterconnectInvoiceDetails() { }
        public IEnumerable<InterconnectInvoiceDetails> GetSupplierInvoiceDetailsRDLCSchema()
        {
            return null;
        }
    }
}

