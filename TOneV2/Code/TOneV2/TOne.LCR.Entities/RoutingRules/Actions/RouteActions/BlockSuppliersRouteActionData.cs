using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class BlockSuppliersRouteActionData : BaseRouteRuleActionData
    {
        public HashSet<string> BlockedOptions { get; set; }

        public override string GetDescription(BusinessEntity.Entities.IBusinessEntityInfoManager businessEntityManager)
        {
            return "Block Suppliers";
        }
    }

    public class BlockSupplierOption
    {
        public string SupplierId { get; set; }
    }
}
