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
        public override Guid ConfigId { get { return new Guid("5a998636-0de9-4654-b430-c24805dd78d9"); } }

        public override void Execute(IRouteOptionRuleExecutionContext context, RouteOptionRuleTarget target)
        {
            target.BlockOption = true;
        }
    }
}
