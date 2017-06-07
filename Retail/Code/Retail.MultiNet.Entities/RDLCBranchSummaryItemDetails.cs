using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.Entities
{
    public class RDLCBranchSummaryItemDetails
    {
        public string UsageDescription { get; set; }
        public int Quantity { get; set; }
        public Decimal NetAmount { get; set; }
        public Guid ChargeableEntityId { get; set; }

        public RDLCBranchSummaryItemDetails()
        {
                
        }
        public IEnumerable<RDLCBranchSummaryItemDetails> GetRDLCSummaryInvoiceItemDetailsSchema()
        {
            return null;
        }
    }
}
