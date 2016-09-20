using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public abstract class RouteOptionRuleSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IRouteOptionRuleExecutionContext context, RouteOptionRuleTarget target);
    }
}
