using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public abstract class RouteRuleOptionFilterSettings
    {
        public abstract void Execute(IRouteOptionRuleExecutionContext context, RouteOptionRuleTarget target);
    }
}
