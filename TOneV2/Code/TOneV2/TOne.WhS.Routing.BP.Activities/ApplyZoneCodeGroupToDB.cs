using System;
using System.Activities;
using Vanrise.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{
    public class ApplyZoneCodeGroupToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }

        public int RoutingDatabaseId { get; set; }
    }

    public sealed class ApplyZoneCodeGroupToDB : DependentAsyncActivity<ApplyZoneCodeGroupToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void DoWork(ApplyZoneCodeGroupToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IRPZoneCodeGroupDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPZoneCodeGroupDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            dataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedZoneCodeGroup) =>
                    {
                        dataManager.ApplyZoneCodeGroupsForDB(preparedZoneCodeGroup);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Applying Zone Code Group To DB is done", null);
        }

        protected override ApplyZoneCodeGroupToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyZoneCodeGroupToDBInput()
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}