using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.LCR.Entities
{
    public class BlockRouteOptionActionData : BaseRouteRuleActionData
    {
        public MultipleSelection<string> Customers { get; set; }

        public override string GetDescription(BusinessEntity.Entities.IBusinessEntityInfoManager businessEntityManager)
        {
            return "Block Route Option";
        }
    }
}
