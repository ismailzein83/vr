using System;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class BlockRouteOptionRule : RouteOptionRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("5a998636-0de9-4654-b430-c24805dd78d9"); } }

        public override void Execute(IRouteOptionRuleExecutionContext context, BaseRouteOptionRuleTarget target)
        {
            target.BlockOption = true;
        }

        public override RouteOptionRuleSettings BuildLinkedRouteOptionRuleSettings(ILinkedRouteOptionRuleContext context)
        {
            return new BlockRouteOptionRule();
        }
    }
}