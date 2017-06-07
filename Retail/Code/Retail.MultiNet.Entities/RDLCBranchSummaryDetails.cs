using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.Entities
{
    public class RDLCBranchSummaryDetails
    {
        public decimal WHTax { get; set; }
        public decimal WHTaxAmount { get; set; }
        public decimal SalesTax { get; set; }
        public decimal SalesTaxAmount { get; set; }
        public int CurrencyId { get; set; }
        public int Quantity { get; set; }
        public decimal CurrentCharges { get; set; }
        public decimal TotalCurrentCharges { get; set; }
        public long AccountId { get; set; }
        public RDLCBranchSummaryDetails()
        {

        }
        public IEnumerable<RDLCBranchSummaryDetails> GetRDLCBranchSummaryDetailsSchema()
        {
            return null;
        }
    }
}
