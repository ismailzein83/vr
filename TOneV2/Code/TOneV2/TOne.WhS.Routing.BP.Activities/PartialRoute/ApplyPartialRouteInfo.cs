using System;
using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class ApplyPartialRouteInfo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<int> LastVersionNumber { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }
        [RequiredArgument]
        public InArgument<DateTime?> NextOpenOrCloseRuleTime { get; set; }

        [RequiredArgument]
        public InArgument<PartialRouteInfo> PartialRouteInfo { get; set; }

        [RequiredArgument]
        public InArgument<int> LatestSaleRateVersionNumber { get; set; }

        [RequiredArgument]
        public InArgument<int> LatestCostRateVersionNumber { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            PartialRouteInfo partialRouteInfo = this.PartialRouteInfo.Get(context);
            partialRouteInfo.LastVersionNumber = this.LastVersionNumber.Get(context);
            partialRouteInfo.LatestRoutingDate = this.EffectiveDate.Get(context);
            partialRouteInfo.NextOpenOrCloseRuleTime = this.NextOpenOrCloseRuleTime.Get(context);
            partialRouteInfo.LatestSaleRateVersionNumber = this.LatestSaleRateVersionNumber.Get(context);
            partialRouteInfo.LatestCostRateVersionNumber = this.LatestCostRateVersionNumber.Get(context);

            RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);

            IRoutingEntityDetailsDataManager routingEntityDetailsDataManager = RoutingDataManagerFactory.GetDataManager<IRoutingEntityDetailsDataManager>();
            routingEntityDetailsDataManager.RoutingDatabase = routingDatabase;

            RoutingEntityDetails routingEntityDetails = new RoutingEntityDetails() { RoutingEntityType = RoutingEntityType.PartialRouteInfo, RoutingEntityInfo = partialRouteInfo };
            routingEntityDetailsDataManager.ApplyRoutingEntityDetails(routingEntityDetails);
        }
    }
}