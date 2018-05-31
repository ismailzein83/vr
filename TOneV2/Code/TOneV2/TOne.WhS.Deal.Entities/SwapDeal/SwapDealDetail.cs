using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealDetail : DealDefinitionDetail
    {
        public string ContractDescription { get; set; }

        public decimal SellingAmount { set; get; }

        public decimal BuyingAmount { set; get; }

        public int SellingVolume { set; get; }

        public int BuyingVolume { set; get; }
    }
}
