using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{
    public class CheckModifiedCustomerRoutesInput
    {
        public BaseQueue<PartialCustomerRoutesBatch> CustomerRoutesBatchQueueInput { get; set; }

        public BaseQueue<List<CustomerRouteData>> CustomerRoutesDataQueueOutput { get; set; }
    }

    public class CheckModifiedCustomerRoutesOutput
    {
        public bool HasModifiedCustomerRoute { get; set; }
    }

    public sealed class CheckModifiedCustomerRoutes : DependentAsyncActivity<CheckModifiedCustomerRoutesInput, CheckModifiedCustomerRoutesOutput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<PartialCustomerRoutesBatch>> CustomerRoutesBatchQueueInput { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<List<CustomerRouteData>>> CustomerRoutesDataQueueOutput { get; set; }

        [RequiredArgument]
        public OutArgument<bool> HasModifiedCustomerRoute { get; set; }

        protected override CheckModifiedCustomerRoutesOutput DoWorkWithResult(CheckModifiedCustomerRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            BaseQueue<PartialCustomerRoutesBatch> customerRoutesBatchQueueInput = inputArgument.CustomerRoutesBatchQueueInput;
            BaseQueue<List<CustomerRouteData>> customerRoutesDataQueueOutput = inputArgument.CustomerRoutesDataQueueOutput;
            CustomerRouteManager customerRouteManager = new CustomerRouteManager();

            long numberOfRoutesToUpdate = 0;
            bool hasModifiedCustomerRoute = false;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.CustomerRoutesBatchQueueInput.TryDequeue((customerRoutesBatch) =>
                    {
                        if (customerRoutesBatch != null && customerRoutesBatch.CustomerRoutes != null && customerRoutesBatch.AffectedCustomerRoutes != null)
                        {
                            List<CustomerRouteData> customerRoutesToUpdate = new List<CustomerRouteData>();
                            foreach (CustomerRoute customerRoute in customerRoutesBatch.CustomerRoutes)
                            {
                                CustomerRouteData customerRouteData = BuildCustomerRouteData(customerRoute, customerRouteManager);
                                if (!AreEquals(customerRouteData, customerRoutesBatch.AffectedCustomerRoutes.GetRecord(customerRoute.CustomerId)))
                                    customerRoutesToUpdate.Add(customerRouteData);
                            }

                            if (customerRoutesToUpdate.Count > 0)
                            {
                                numberOfRoutesToUpdate += customerRoutesToUpdate.Count;
                                customerRoutesDataQueueOutput.Enqueue(customerRoutesToUpdate);
                            }
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            hasModifiedCustomerRoute = numberOfRoutesToUpdate > 0;
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Checking Modified Routes is done. {0} Routes are modified", numberOfRoutesToUpdate);

            return new CheckModifiedCustomerRoutesOutput() { HasModifiedCustomerRoute = hasModifiedCustomerRoute };
        }

        private bool AreEquals(CustomerRouteData currentCustomerRouteData, CustomerRouteData previousCustomerRouteData)
        {
            if (currentCustomerRouteData.ExecutedRuleId != previousCustomerRouteData.ExecutedRuleId)
                return false;

            if (currentCustomerRouteData.IsBlocked != previousCustomerRouteData.IsBlocked)
                return false;

            if (string.Compare(currentCustomerRouteData.Options, previousCustomerRouteData.Options) != 0)
                return false;

            return true;
        }

        private CustomerRouteData BuildCustomerRouteData(CustomerRoute customerRoute, CustomerRouteManager customerRouteManager)
        {
            return new CustomerRouteData()
            {
                Code = customerRoute.Code,
                CustomerId = customerRoute.CustomerId,
                ExecutedRuleId = customerRoute.ExecutedRuleId,
                IsBlocked = customerRoute.IsBlocked,
                Options = customerRoute.Options != null ? Helper.SerializeOptions(customerRoute.Options) : null,
                SaleZoneId = customerRoute.SaleZoneId,
                VersionNumber = customerRoute.VersionNumber
            };
        }

        protected override CheckModifiedCustomerRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CheckModifiedCustomerRoutesInput
            {
                CustomerRoutesBatchQueueInput = this.CustomerRoutesBatchQueueInput.Get(context),
                CustomerRoutesDataQueueOutput = this.CustomerRoutesDataQueueOutput.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CheckModifiedCustomerRoutesOutput result)
        {
            this.HasModifiedCustomerRoute.Set(context, result.HasModifiedCustomerRoute);
        }
    }
}