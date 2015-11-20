using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules
{
    public class BlockRouteRule : RouteRuleSettings
    {
        public override void Execute(IRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            target.BlockRoute = true;
        }
    }
}
