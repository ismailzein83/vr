using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Business.RouteOptionRules
{
    public class BlockRouteOptionRule : RouteOptionRuleSettings
    {
        public override void Execute(IRouteOptionRuleExecutionContext context, RouteOptionRuleTarget target)
        {
            target.BlockOption = true;
        }
    }
}
