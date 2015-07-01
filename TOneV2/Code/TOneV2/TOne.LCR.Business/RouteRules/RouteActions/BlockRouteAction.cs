using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class BlockRouteAction : BaseRouteAction
    {
        public override Type GetActionDataType()
        {
            return typeof(BlockRouteActionData);
        }

        public override RouteActionResult Execute(IRouteBuildContext context, object actionData, RouteRule rule)
        {
            context.BlockRoute();
            if (rule != null)
            {
                context.Route.RuleId = rule.RouteRuleId;
                context.Route.RuleActionType = RouteRuleActionType.Block;
            }
            return null;
        }
    }
}
