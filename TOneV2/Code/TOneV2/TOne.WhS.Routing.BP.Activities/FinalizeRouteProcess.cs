﻿using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class FinalizeRouteProcess : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<bool> BuildRouteSync { get; set; }

        [RequiredArgument]
        public InArgument<RoutingDatabaseType> RoutingDatabaseType { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            bool buildRouteSync = this.BuildRouteSync.Get(context.ActivityContext);
            RoutingDatabaseType routingDatabaseType = this.RoutingDatabaseType.Get(context.ActivityContext);
            int routingDatabaseId = this.RoutingDatabaseId.Get(context.ActivityContext);

            FinalizeRouteContext finalizeRouteContext = new FinalizeRouteContext()
            {
                UpdateLastRouteSync = buildRouteSync,
                UpdateLastRouteBuild = routingDatabaseType == Entities.RoutingDatabaseType.Current
            };

            IRoutingDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            dataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(routingDatabaseId);

            dataManager.FinalizeRoutingProcess(finalizeRouteContext, (message) =>
            {
                context.ActivityContext.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, null);
            });
        }
    }
}