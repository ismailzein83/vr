using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public abstract class RouteRuleOptionPercentageSettings
    {
        public int ConfigId { get; set; }

        public abstract void Execute(IRouteRuleExecutionContext context, RouteRuleTarget target);
    }
}
