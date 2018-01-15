using System;
using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class ApplyRPQualityDataToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
        public int RoutingDatabaseId { get; set; }
    }

    public sealed class ApplyRPQualityDataToDB : DependentAsyncActivity<ApplyRPQualityDataToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }


        protected override void DoWork(ApplyRPQualityDataToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IRPQualityConfigurationDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPQualityConfigurationDataManager>();
            dataManager.RoutingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCustomerRouteQualityData) =>
                    {
                        dataManager.ApplyQualityConfigurationsToDB(preparedCustomerRouteQualityData);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Applying Routing Product Quality Data To DB is done", null);
        }

        protected override ApplyRPQualityDataToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyRPQualityDataToDBInput()
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}
