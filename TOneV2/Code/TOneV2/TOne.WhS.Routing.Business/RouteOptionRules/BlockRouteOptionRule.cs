using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class BlockRouteOptionRule : RouteOptionRuleSettings
    {
        public const int ExtensionConfigId = 35;
        public override void Execute(IRouteOptionRuleExecutionContext context, RouteOptionRuleTarget target)
        {
            target.BlockOption = true;
        }
    }
}
