using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Rules;
using Vanrise.Rules.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class DeleteRouteRuleChanged : CodeActivity
    {
        public InArgument<int?> RouteRuleId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int? routeRuleId = this.RouteRuleId.Get(context);
            RouteRuleManager routeRuleManager = new RouteRuleManager();
            if (routeRuleId.HasValue)
            {
                routeRuleManager.DeleteRuleChanged(routeRuleId.Value);
            }
            else
            {
                routeRuleManager.DeleteRulesChanged();
            }
        }
    }
}