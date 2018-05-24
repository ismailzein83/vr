using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Interconnect.Entities
{
    public class InterconnectInvoiceItemDetails
    {
        public decimal Duration { get; set; }
        public decimal Amount { get; set; }
        public long DestinationZoneId { get; set; }
        public string DestinationZoneIdDescription { get; set; }

        public long OriginationZoneId { get; set; }
        public string OriginationZoneIdDescription { get; set; }

        public long OperatorId { get; set; }
        public string OperatorIdDescription { get; set; }

        public decimal Rate { get; set; }
        public string RateDescription { get; set; }

        public int RateTypeId { get; set; }
        public string RateTypeIdDescription { get; set; }

        public int TrafficDirection { get; set; }
        public string TrafficDirectionDescription { get; set; }

        public int CurrencyId { get; set; }
        public string CurrencyIdDescription { get; set; }

        public InterconnectInvoiceItemDetails() { }
        public IEnumerable<InterconnectInvoiceItemDetails> GetCustomerInvoiceItemDetailsRDLCSchema()
        {
            return null;
        }
    }
}
