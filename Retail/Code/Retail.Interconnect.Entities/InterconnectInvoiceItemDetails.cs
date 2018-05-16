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
        public long OperatorId { get; set; }
        public decimal Rate { get; set; }
        public int RateTypeId { get; set; }
        public int TrafficDirection { get; set; }
        public int CurrencyId { get; set; }
    }
}
