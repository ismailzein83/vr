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

        public BaseQueue<List<CustomerRouteData>> CustomerRoutesDataQueueInput { get; set; }
    }

    public sealed class ApplyAffectedCustomerRoutesToDB : DependentAsyncActivity<ApplyAffectedCustomerRoutesToDBInput>
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<List<CustomerRouteData>>> CustomerRoutesDataQueueInput { get; set; }

        protected override void DoWork(ApplyAffectedCustomerRoutesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICustomerRouteDataManager customerRouteDataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            customerRouteDataManager.RoutingDatabase = inputArgument.RoutingDatabase;
            List<CustomerRouteData> customerRoutesToUpdate = new List<CustomerRouteData>();

            int partialRoutesUpdateBatchSize = new ConfigManager().GetPartialRoutesUpdateBatchSize();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.CustomerRoutesDataQueueInput.TryDequeue((customerRoutesDataList) =>
                    {
                        if (customerRoutesDataList != null)
                        {
                            customerRoutesToUpdate.AddRange(customerRoutesDataList);

                            if (customerRoutesToUpdate.Count > partialRoutesUpdateBatchSize)
                            {
                                customerRouteDataManager.UpdateCustomerRoutes(customerRoutesToUpdate.ToList());
                                customerRoutesToUpdate = new List<CustomerRouteData>();
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
                CustomerRoutesDataQueueInput = this.CustomerRoutesDataQueueInput.Get(context),
                RoutingDatabase = this.RoutingDatabase.Get(context)
            };
        }
    }
}