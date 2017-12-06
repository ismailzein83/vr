using System;
using System.Activities;
using Vanrise.Common;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class GetBERouteInfo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public OutArgument<BERouteInfo> BERouteInfo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IRoutingEntityDetailsDataManager routingEntityDetailsDataManager = RoutingDataManagerFactory.GetDataManager<IRoutingEntityDetailsDataManager>();
            routingEntityDetailsDataManager.RoutingDatabase = this.RoutingDatabase.Get(context);

            RoutingEntityDetails routingEntityDetails = routingEntityDetailsDataManager.GetRoutingEntityDetails(RoutingEntityType.BERoute);
            routingEntityDetails.ThrowIfNull("routingEntityDetails");

            BERouteInfo beRouteInfo = routingEntityDetails.RoutingEntityInfo.CastWithValidate<BERouteInfo>("routingEntityDetails.RoutingEntityInfo");

            this.BERouteInfo.Set(context, beRouteInfo);
        }
    }
}