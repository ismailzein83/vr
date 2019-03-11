using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class SupplierInvoiceDealItemDetails
    {
        public long? CostDealZoneGroupNb { get; set; }
        public int? CostDealTierNb { get; set; }
        public int? CostDeal { get; set; }
        public decimal? CostDealRateTierNb { get; set; }
        public decimal Duration { get; set; }
        public decimal Amount { get; set; }
        public int NumberOfCalls { get; set; }
        public int Currency { get; set; }
    }
}
