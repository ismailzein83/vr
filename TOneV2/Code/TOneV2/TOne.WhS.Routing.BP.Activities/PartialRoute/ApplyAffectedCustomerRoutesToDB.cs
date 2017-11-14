using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;
using System.Linq;

namespace TOne.WhS.Routing.BP.Activities
{
    public class ApplyAffectedCustomerRoutesToDBInput
    {
        public RoutingDatabase RoutingDatabase { get; set; }

        public BaseQueue<CustomerRoutesBatch> CustomerRoutesBatchQueueInput { get; set; }
    }

    public sealed class ApplyAffectedCustomerRoutesToDB : DependentAsyncActivity<ApplyAffectedCustomerRoutesToDBInput>
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<CustomerRoutesBatch>> CustomerRoutesBatchQueueInput { get; set; }

        protected override void DoWork(ApplyAffectedCustomerRoutesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICustomerRouteDataManager customerRouteDataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            customerRouteDataManager.RoutingDatabase = inputArgument.RoutingDatabase;
            List<CustomerRoute> customerRoutesToUpdate = new List<CustomerRoute>();

            int partialRoutesUpdateBatchSize = new ConfigManager().GetPartialRoutesUpdateBatchSize();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.CustomerRoutesBatchQueueInput.TryDequeue((customerRoutesBatch) =>
                    {
                        if (customerRoutesBatch != null && customerRoutesBatch.CustomerRoutes != null)
                        {
                            customerRoutesToUpdate.AddRange(customerRoutesBatch.CustomerRoutes);

                            while (customerRoutesToUpdate.Count > partialRoutesUpdateBatchSize)
                            {
                                IEnumerable<CustomerRoute> customerRoutesBatchForUpdate = customerRoutesToUpdate.Take(partialRoutesUpdateBatchSize);
                                customerRoutesToUpdate.RemoveRange(0, partialRoutesUpdateBatchSize);
                                customerRouteDataManager.UpdateCustomerRoutes(customerRoutesBatchForUpdate.ToList());
                            }
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            if (customerRoutesToUpdate.Count > 0)
                customerRouteDataManager.UpdateCustomerRoutes(customerRoutesToUpdate);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Updating Affected Routes is done", null);
        }

        protected override ApplyAffectedCustomerRoutesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyAffectedCustomerRoutesToDBInput
            {
                CustomerRoutesBatchQueueInput = this.CustomerRoutesBatchQueueInput.Get(context),
                RoutingDatabase = this.RoutingDatabase.Get(context)
            };
        }
    }
}