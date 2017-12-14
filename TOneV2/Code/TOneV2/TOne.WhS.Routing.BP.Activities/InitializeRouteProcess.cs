using System;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class InitializeRouteProcess : BaseCodeActivity
    {
        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            new RouteRuleManager().FillAndGetRulesChangedForProcessing();
            new RouteOptionRuleManager().FillAndGetRulesChangedForProcessing();
        }
    }
}
