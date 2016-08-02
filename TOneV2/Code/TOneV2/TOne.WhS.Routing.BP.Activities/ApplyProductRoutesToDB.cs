using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class ApplyProductRoutesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
        public int RoutingDatabaseId { get; set; }
    }

    public class ApplyProductRoutesToDB : DependentAsyncActivity<ApplyProductRoutesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }
        protected override void DoWork(ApplyProductRoutesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IRPRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            dataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedProductRoute) =>
                    {
                        dataManager.ApplyProductRouteForDB(preparedProductRoute);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Applying Product Routes To DB is done", null);
        }

        protected override ApplyProductRoutesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyProductRoutesToDBInput
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}
