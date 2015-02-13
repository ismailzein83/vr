using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public abstract class BaseRouteRuleAction
    {
        public abstract string ActionDisplayName { get; }

        public virtual bool ApplyActionToRoute(RouteDetail route, RoutingRule ruleDefinition)
        {
            return true;
        }

    }
}
