using System;
using System.Activities;
using Vanrise.Common;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class UpdateBERouteInfo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<BERouteInfo> BERouteInfo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IRoutingEntityDetailsDataManager routingEntityDetailsDataManager = RoutingDataManagerFactory.GetDataManager<IRoutingEntityDetailsDataManager>();
            routingEntityDetailsDataManager.RoutingDatabase = this.RoutingDatabase.Get(context);

            RoutingEntityDetails routingEntityDetails = new RoutingEntityDetails() { RoutingEntityType = RoutingEntityType.BERouteInfo, RoutingEntityInfo = this.BERouteInfo.Get(context) };
            routingEntityDetailsDataManager.ApplyRoutingEntityDetails(routingEntityDetails);
        }
    }
}