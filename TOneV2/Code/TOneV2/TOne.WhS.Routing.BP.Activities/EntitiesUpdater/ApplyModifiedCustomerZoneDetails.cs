using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{
    public class ApplyModifiedCustomerZoneDetailsInput
    {
        public RoutingDatabase RoutingDatabase { get; set; }
        public BaseQueue<List<CustomerZoneDetail>> InputQueue { get; set; }
    }

    public sealed class ApplyModifiedCustomerZoneDetails : DependentAsyncActivity<ApplyModifiedCustomerZoneDetailsInput>
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<List<CustomerZoneDetail>>> InputQueue { get; set; }

        protected override void DoWork(ApplyModifiedCustomerZoneDetailsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICustomerZoneDetailsDataManager customerZoneDetailsDataManager = RoutingDataManagerFactory.GetDataManager<ICustomerZoneDetailsDataManager>();
            customerZoneDetailsDataManager.RoutingDatabase = inputArgument.RoutingDatabase;

            int partialRoutesUpdateBatchSize = new ConfigManager().GetPartialRoutesUpdateBatchSize();

            List<CustomerZoneDetail> customerZoneDetailToUpdate = new List<CustomerZoneDetail>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((customerZoneDetails) =>
                    {
                        if (customerZoneDetails != null)
                        {
                            customerZoneDetailToUpdate.AddRange(customerZoneDetails);

                            if (customerZoneDetailToUpdate.Count > partialRoutesUpdateBatchSize)
                            {
                                customerZoneDetailsDataManager.UpdateCustomerZoneDetails(customerZoneDetailToUpdate);
                                customerZoneDetailToUpdate = new List<CustomerZoneDetail>();
                            }
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            if (customerZoneDetailToUpdate.Count > 0)
                customerZoneDetailsDataManager.UpdateCustomerZoneDetails(customerZoneDetailToUpdate);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Updating Modified Customer Zone Details is done", null);
        }

        protected override ApplyModifiedCustomerZoneDetailsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyModifiedCustomerZoneDetailsInput
            {
                RoutingDatabase = this.RoutingDatabase.Get(context),
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}