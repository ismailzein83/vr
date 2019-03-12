using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class CustomerInvoiceDealItemDetails
    {
        public long? SaleDealZoneGroupNb { get; set; }
        public int? SaleDealTierNb { get; set; }
        public int? SaleDeal { get; set; }
        public decimal? SaleDealRateTierNb { get; set; }
        public decimal Duration { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal Amount { get; set; }
        public int NumberOfCalls { get; set; }
        public int Currency { get; set; }
    }
}
