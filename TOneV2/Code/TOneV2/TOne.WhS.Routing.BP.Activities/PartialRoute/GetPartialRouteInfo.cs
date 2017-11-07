using System;
using System.Activities;
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
            IPartialRouteInfoDataManager partialRouteInfoDataManager = RoutingDataManagerFactory.GetDataManager<IPartialRouteInfoDataManager>();
            partialRouteInfoDataManager.RoutingDatabase = this.RoutingDatabase.Get(context);

            PartialRouteInfo partialRouteInfo = partialRouteInfoDataManager.GetPartialRouteInfo();
            if (partialRouteInfo == null)
                partialRouteInfo = new PartialRouteInfo() { LastVersionNumber = 0 };

            partialRouteInfo.LastVersionNumber++;

            this.PartialRouteInfo.Set(context, partialRouteInfo);
        }
    }
}