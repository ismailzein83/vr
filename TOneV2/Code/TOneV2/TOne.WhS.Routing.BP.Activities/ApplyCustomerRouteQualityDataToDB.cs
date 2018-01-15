using System;
using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class ApplyCustomerRouteQualityDataToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
        public int RoutingDatabaseId { get; set; }
    }

    public sealed class ApplyCustomerRouteQualityDataToDB : DependentAsyncActivity<ApplyCustomerRouteQualityDataToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }
        
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void DoWork(ApplyCustomerRouteQualityDataToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICustomerQualityConfigurationDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerQualityConfigurationDataManager>();
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

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Applying Customer Quality Data To DB is done", null);
        }

        protected override ApplyCustomerRouteQualityDataToDBInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new ApplyCustomerRouteQualityDataToDBInput()
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}