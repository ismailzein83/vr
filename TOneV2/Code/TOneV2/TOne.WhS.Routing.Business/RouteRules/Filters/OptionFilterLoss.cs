using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Filters
{
    public class OptionFilterLoss : RouteRuleOptionFilterSettings
    {
        public override void Execute(BusinessEntity.Entities.IRouteOptionRuleExecutionContext context, BusinessEntity.Entities.RouteOptionRuleTarget target)
        {
            if (target.RouteTarget.SaleRate.HasValue && target.RouteTarget.SaleRate.Value < target.SupplierRate)
                target.FilterOption = true;
        }
    }
}
