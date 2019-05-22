using System;
using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class ApplyModifiedCustomerRoutesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
        public RoutingDatabase RoutingDatabase { get; set; }
    }

    public class ApplyModifiedCustomerRoutesPreviewToDB : DependentAsyncActivity<ApplyModifiedCustomerRoutesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        protected override void DoWork(ApplyModifiedCustomerRoutesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            IModifiedCustomerRoutePreviewDataManager modifiedCustomerRoutePreviewDataManager = RoutingDataManagerFactory.GetDataManager<IModifiedCustomerRoutePreviewDataManager>();

            modifiedCustomerRoutePreviewDataManager.RoutingDatabase = inputArgument.RoutingDatabase;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedMCRoutePreview) =>
                    {
                        modifiedCustomerRoutePreviewDataManager.ApplyModifiedCustomerRoutesPreviewForDB(preparedMCRoutePreview);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Saving Modified Customer Routes Preview To DB is done", null);
        }

        protected override ApplyModifiedCustomerRoutesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyModifiedCustomerRoutesToDBInput
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabase = this.RoutingDatabase.Get(context)
            };
        }
    }
}
