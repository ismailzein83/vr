using System;
using System.Activities;
using Vanrise.Common;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class GetPartialRouteInfo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public OutArgument<PartialRouteInfo> PartialRouteInfo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IRoutingEntityDetailsDataManager routingEntityDetailsDataManager = RoutingDataManagerFactory.GetDataManager<IRoutingEntityDetailsDataManager>();
            routingEntityDetailsDataManager.RoutingDatabase = this.RoutingDatabase.Get(context);

            RoutingEntityDetails routingEntityDetails = routingEntityDetailsDataManager.GetRoutingEntityDetails(RoutingEntityType.PartialRouteInfo);
            routingEntityDetails.ThrowIfNull("routingEntityDetails");

            PartialRouteInfo partialRouteInfo = routingEntityDetails.RoutingEntityInfo.CastWithValidate<PartialRouteInfo>("routingEntityDetails.RoutingEntityInfo");

            this.PartialRouteInfo.Set(context, partialRouteInfo);
        }
    }
}