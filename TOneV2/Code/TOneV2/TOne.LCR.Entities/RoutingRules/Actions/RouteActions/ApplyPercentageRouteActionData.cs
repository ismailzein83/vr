using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class ApplyPercentageRouteActionData : BaseRouteRuleActionData
    {
        public override string GetDescription(BusinessEntity.Entities.IBusinessEntityInfoManager businessEntityManager)
        {
            return "Percentage Rule";
        }
    }
}
