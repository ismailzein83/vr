using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class ApplyModifiedCustomerRoutingQualityDataInput
    {
        public RoutingDatabase RoutingDatabase { get; set; }
        public BaseQueue<List<CustomerRouteQualityConfigurationData>> InputQueue { get; set; }
    }

    public sealed class ApplyModifiedCustomerRoutingQualityData : DependentAsyncActivity<ApplyModifiedCustomerRoutingQualityDataInput>
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<List<CustomerRouteQualityConfigurationData>>> InputQueue { get; set; }

        protected override void DoWork(ApplyModifiedCustomerRoutingQualityDataInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICustomerQualityConfigurationDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerQualityConfigurationDataManager>();
            dataManager.RoutingDatabase = inputArgument.RoutingDatabase;

            int partialRoutesUpdateBatchSize = new ConfigManager().GetPartialRoutesUpdateBatchSize();

            List<CustomerRouteQualityConfigurationData> customerRouteQualityConfigurationDataToUpdate = new List<CustomerRouteQualityConfigurationData>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
             {
                 bool hasItem = false;
                 do
                 {
                     hasItem = inputArgument.InputQueue.TryDequeue((customerRouteQualityConfigurations) =>
                         {
                             if (customerRouteQualityConfigurations != null)
                             {
                                 customerRouteQualityConfigurationDataToUpdate.AddRange(customerRouteQualityConfigurations);

                                 if (customerRouteQualityConfigurationDataToUpdate.Count > partialRoutesUpdateBatchSize)
                                 {
                                     dataManager.UpdateCustomerRouteQualityConfigurationsData(customerRouteQualityConfigurationDataToUpdate);
                                     customerRouteQualityConfigurationDataToUpdate = new List<CustomerRouteQualityConfigurationData>();
                                 }
                             }
                         });
                 } while (!ShouldStop(handle) && hasItem);
             });

            if (customerRouteQualityConfigurationDataToUpdate.Count > 0)
                dataManager.UpdateCustomerRouteQualityConfigurationsData(customerRouteQualityConfigurationDataToUpdate);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Updating Modified Customer Route Quality Configuration is done", null);
        }

        protected override ApplyModifiedCustomerRoutingQualityDataInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyModifiedCustomerRoutingQualityDataInput
            {
                RoutingDatabase = this.RoutingDatabase.Get(context),
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
