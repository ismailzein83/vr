using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class RouteRuleSettings
    {
        public int ConfigId { get; set; }

        public virtual void Execute(IRouteRuleExecutionContext context, RouteRuleTarget target)
        {
        }

        public virtual void ApplyOptionsOrder(IRouteRuleExecutionContext context, RouteRuleTarget target)
        {

        }
    }
}
