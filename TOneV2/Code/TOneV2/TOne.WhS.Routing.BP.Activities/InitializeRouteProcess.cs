using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class InitializeRouteProcess : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<RoutingDatabaseType> RoutingDatabaseType { get; set; }

        [RequiredArgument]
        public InArgument<bool> GenerateAnalysisData { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            RoutingDatabaseType routingDatabaseType = this.RoutingDatabaseType.Get(context.ActivityContext);
            bool generateAnalysisData = this.GenerateAnalysisData.Get(context.ActivityContext);

            new RouteRuleManager().FillAndGetRulesChangedForProcessing();
            new RouteOptionRuleManager().FillAndGetRulesChangedForProcessing();

            if (generateAnalysisData)
                new CustomerRouteMarginStagingManager().CreateCustomerRouteMarginStagingTable(routingDatabaseType);
        }
    }
}