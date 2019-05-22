using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class CheckModifiedCustomerRoutesInput
    {
        public BaseQueue<PartialCustomerRoutesBatch> CustomerRoutesBatchQueueInput { get; set; }
        public BaseQueue<List<CustomerRouteData>> CustomerRoutesDataQueueOutput { get; set; }
        public BaseQueue<PartialCustomerRoutesPreviewBatch> CustomerRoutesPreviewBatchQueueInput { get; set; }
        public bool NeedsApproval { get; set; }

    }

    public class CheckModifiedCustomerRoutesOutput
    {
        public bool HasModifiedCustomerRoute { get; set; }
        public long TotalChangedRoutes { get; set; }
    }

    public sealed class CheckModifiedCustomerRoutes : DependentAsyncActivity<CheckModifiedCustomerRoutesInput, CheckModifiedCustomerRoutesOutput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<PartialCustomerRoutesBatch>> CustomerRoutesBatchQueueInput { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<List<CustomerRouteData>>> CustomerRoutesDataQueueOutput { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<PartialCustomerRoutesPreviewBatch>> CustomerRoutesPreviewBatchQueueInput { get; set; }

        [RequiredArgument]
        public InArgument<bool> NeedsApproval { get; set; }

        [RequiredArgument]
        public OutArgument<bool> HasModifiedCustomerRoute { get; set; }

        [RequiredArgument]
        public OutArgument<long> TotalChangedRoutes { get; set; }

        protected override CheckModifiedCustomerRoutesOutput DoWorkWithResult(CheckModifiedCustomerRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            BaseQueue<PartialCustomerRoutesBatch> customerRoutesBatchQueueInput = inputArgument.CustomerRoutesBatchQueueInput;
            BaseQueue<List<CustomerRouteData>> customerRoutesDataQueueOutput = inputArgument.CustomerRoutesDataQueueOutput;
            BaseQueue<PartialCustomerRoutesPreviewBatch> customerRoutesPreviewBatchQueueInput = inputArgument.CustomerRoutesPreviewBatchQueueInput;

            var partialCustomerRoutesPreviewBatch = new PartialCustomerRoutesPreviewBatch();

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
                        if (customerRoutesBatch == null || customerRoutesBatch.CustomerRoutes == null || customerRoutesBatch.AffectedCustomerRoutes == null)
                            return;

                        List<CustomerRouteData> customerRoutesToUpdate = new List<CustomerRouteData>();
                        List<ModifiedCustomerRoutePreviewData> modifiedCustomerRoutesToPreview = new List<ModifiedCustomerRoutePreviewData>();

                        foreach (CustomerRoute customerRoute in customerRoutesBatch.CustomerRoutes)
                        {
                            CustomerRouteData customerRouteData = BuildCustomerRouteData(customerRoute, customerRouteManager);
                            var affectedCustomerRouteData = customerRoutesBatch.AffectedCustomerRoutes.GetRecord(customerRoute.CustomerId);

                            if (!AreEquals(customerRouteData, affectedCustomerRouteData))
                            {
                                customerRoutesToUpdate.Add(customerRouteData);

                                if (inputArgument.NeedsApproval)
                                    modifiedCustomerRoutesToPreview.Add(BuildModifiedCustomerRouteToPreview(customerRouteData, affectedCustomerRouteData));
                            }
                        }

                        if (customerRoutesToUpdate.Count == 0)
                            return;

                        numberOfRoutesToUpdate += customerRoutesToUpdate.Count;
                        customerRoutesDataQueueOutput.Enqueue(customerRoutesToUpdate);

                        if (customerRoutesPreviewBatchQueueInput != null && modifiedCustomerRoutesToPreview.Count > 0)
                        {
                            partialCustomerRoutesPreviewBatch.AffectedPartialCustomerRoutesPreview.AddRange(modifiedCustomerRoutesToPreview);
                            customerRoutesPreviewBatchQueueInput.Enqueue(partialCustomerRoutesPreviewBatch);
                            partialCustomerRoutesPreviewBatch = new PartialCustomerRoutesPreviewBatch();
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            hasModifiedCustomerRoute = numberOfRoutesToUpdate > 0;
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Checking Modified Routes is done. {0} Routes are modified", numberOfRoutesToUpdate);

            return new CheckModifiedCustomerRoutesOutput() { HasModifiedCustomerRoute = hasModifiedCustomerRoute, TotalChangedRoutes = numberOfRoutesToUpdate };
        }

        private ModifiedCustomerRoutePreviewData BuildModifiedCustomerRouteToPreview(CustomerRouteData customerRouteData, CustomerRouteData affectedCustomerRouteData)
        {
            return new ModifiedCustomerRoutePreviewData
            {
                Code = customerRouteData.Code,
                CustomerId = customerRouteData.CustomerId,
                SaleZoneId = customerRouteData.SaleZoneId,
                OrigIsBlocked = affectedCustomerRouteData.IsBlocked,
                OrigExecutedRuleId = affectedCustomerRouteData.ExecutedRuleId,
                OrigRouteOptions = affectedCustomerRouteData.Options,
                IsBlocked = customerRouteData.IsBlocked,
                ExecutedRuleId = customerRouteData.ExecutedRuleId,
                RouteOptions = customerRouteData.Options,
                SupplierIds = customerRouteData.SupplierIds,
                IsApproved = true
            };
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
                VersionNumber = customerRoute.VersionNumber,
                SupplierIds = Helper.GetConcatenatedSupplierIds(customerRoute)
            };
        }

        protected override CheckModifiedCustomerRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CheckModifiedCustomerRoutesInput
            {
                CustomerRoutesBatchQueueInput = this.CustomerRoutesBatchQueueInput.Get(context),
                CustomerRoutesDataQueueOutput = this.CustomerRoutesDataQueueOutput.Get(context),
                CustomerRoutesPreviewBatchQueueInput = this.CustomerRoutesPreviewBatchQueueInput.Get(context),
                NeedsApproval = this.NeedsApproval.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CheckModifiedCustomerRoutesOutput result)
        {
            this.HasModifiedCustomerRoute.Set(context, result.HasModifiedCustomerRoute);
            this.TotalChangedRoutes.Set(context, result.TotalChangedRoutes);
        }
    }
}