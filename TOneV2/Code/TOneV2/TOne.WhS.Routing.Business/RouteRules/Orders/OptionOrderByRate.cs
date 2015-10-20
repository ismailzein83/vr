using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Orders
{
    public class OptionOrderByRate : RouteRuleOptionOrderSettings
    {
        public override IEnumerable<RouteOptionRuleTarget> Execute(IRouteRuleExecutionContext context, IEnumerable<RouteOptionRuleTarget> options)
        {
            return options.OrderBy(itm => itm.SupplierRate);
        }
    }
}
