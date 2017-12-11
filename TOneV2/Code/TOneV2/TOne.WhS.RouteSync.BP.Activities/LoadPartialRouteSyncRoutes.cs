using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.BP.Activities
{
    public sealed class LoadPartialRouteSyncRoutes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Dictionary<string, SwitchSyncData>> SwitchSyncDataBySwitchId { get; set; }

        [RequiredArgument]
        public InArgument<int> MinVersionNb { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<string, List<CustomerRoute>>> CustomerRoutesBySwitchId { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Load Partial Route Sync Routes has started");

            var switchSyncDataBySwitchId = this.SwitchSyncDataBySwitchId.Get(context);
            var minVersionNb = this.MinVersionNb.Get(context);

            RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);
            ICustomerRouteDataManager customerRouteDataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            customerRouteDataManager.RoutingDatabase = routingDatabase;

            List<CustomerRoute> customerRoutes = customerRouteDataManager.GetCustomerRoutesAfterVersionNb(minVersionNb);

            Dictionary<string, List<CustomerRoute>> customerRoutesBySwitchId = new Dictionary<string, List<CustomerRoute>>();

            foreach (var customerRoute in customerRoutes)
            {
                foreach(var kvp in switchSyncDataBySwitchId)
                {
                    string switchId = kvp.Key;
                    SwitchSyncData switchSyncData = kvp.Value;

                    if(customerRoute.VersionNumber > switchSyncData.LastVersionNumber)
                    {
                        List<CustomerRoute> currentSwitchCustomerRoutes = customerRoutesBySwitchId.GetOrCreateItem(switchId);
                        currentSwitchCustomerRoutes.Add(customerRoute);
                    }
                }
            }

            context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Load Partial Route Sync Routes has finished");

            this.CustomerRoutesBySwitchId.Set(context, customerRoutesBySwitchId);
        }
    }
}