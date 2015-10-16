using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Orders
{
    public class OptionOrderByRate : RouteRuleOptionOrderSettings
    {
        public override void Execute(BusinessEntity.Entities.IRouteRuleExecutionContext context, BusinessEntity.Entities.RouteRuleTarget target)
        {
            target.Options = target.Options.OrderBy(itm => itm.SupplierRate).ToList();
        }
    }
}
