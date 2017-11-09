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
        public InArgument<PartialRouteInfo> PartialRouteInfo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            PartialRouteInfo partialRouteInfo = this.PartialRouteInfo.Get(context);
            partialRouteInfo.LastVersionNumber = this.LastVersionNumber.Get(context);
            partialRouteInfo.LatestRoutingDate = this.EffectiveDate.Get(context);

            RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);

            IPartialRouteInfoDataManager partialRouteInfoDataManager = RoutingDataManagerFactory.GetDataManager<IPartialRouteInfoDataManager>();
            partialRouteInfoDataManager.RoutingDatabase = routingDatabase;

            partialRouteInfoDataManager.ApplyPartialRouteInfo(partialRouteInfo);
        }
    }
}