using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.RouteSync.BP.Activities;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class BuildCustomerRoutesInput
    {
        public CustomerZoneDetailByZone CustomerZoneDetails { get; set; }
        public BaseQueue<RoutingCodeMatches> InputQueue { get; set; }
        public BaseQueue<CustomerRoutesBatch> OutputQueue { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public RoutingDatabaseType RoutingDatabaseType { get; set; }
        public bool IsFuture { get; set; }
        public bool GenerateAnalysisData { get; set; }
        public IEnumerable<RoutingCustomerInfo> ActiveRoutingCustomerInfos { get; set; }
        public int VersionNumber { get; set; }
        public int RoutingDatabaseId { get; set; }
        public List<SwitchInProcess> SwitchesInProcess { get; set; }
    }

    public sealed class BuildCustomerRoutes : DependentAsyncActivity<BuildCustomerRoutesInput>
    {
        [RequiredArgument]
        public InArgument<CustomerZoneDetailByZone> CustomerZoneDetails { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RoutingCodeMatches>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CustomerRoutesBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<RoutingDatabaseType> RoutingDatabaseType { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<bool> GenerateAnalysisData { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RoutingCustomerInfo>> ActiveRoutingCustomerInfos { get; set; }

        [RequiredArgument]
        public InArgument<int> VersionNumber { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<List<SwitchInProcess>> SwitchesInProcess { get; set; }


        protected override void DoWork(BuildCustomerRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            int routeBatchSize = 0;
            if (inputArgument.SwitchesInProcess != null && inputArgument.SwitchesInProcess.Count > 0)
            {
                InitialiseSwitchesInProcessQueues(inputArgument.SwitchesInProcess);
                RouteSync.Business.ConfigManager routeSyncConfigManager = new RouteSync.Business.ConfigManager();
                routeBatchSize = routeSyncConfigManager.GetRouteSyncProcessRouteBatchSize();
            }

            CustomerRoutesBatch customerRoutesBatch = new CustomerRoutesBatch();
            List<CustomerRoute> switchesInProcessRoutes = new List<CustomerRoute>();
            RouteBuilder builder = new RouteBuilder(RoutingProcessType.CustomerRoute);

            Dictionary<int, HashSet<int>> customerCountries = new Dictionary<int, HashSet<int>>();

            Dictionary<CustomerSaleZone, SaleZoneOptionsMarginStaging> saleZoneOptionsMarginStagingByCustomerSaleZone = null;
            if (inputArgument.GenerateAnalysisData)
                saleZoneOptionsMarginStagingByCustomerSaleZone = new Dictionary<CustomerSaleZone, SaleZoneOptionsMarginStaging>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodeMatch) =>
                    {
                        BuildCustomerRoutesContext customerRoutesContext = new BuildCustomerRoutesContext(preparedCodeMatch, inputArgument.CustomerZoneDetails, inputArgument.EffectiveDate,
                            inputArgument.IsFuture, inputArgument.ActiveRoutingCustomerInfos, customerCountries, inputArgument.VersionNumber, true, routingDatabase,
                            inputArgument.GenerateAnalysisData, saleZoneOptionsMarginStagingByCustomerSaleZone);

                        IEnumerable<CustomerRoute> customerRoutes = builder.BuildRoutes(customerRoutesContext, preparedCodeMatch.Code);

                        if (inputArgument.SwitchesInProcess != null && inputArgument.SwitchesInProcess.Count > 0)
                        {
                            switchesInProcessRoutes.AddRange(customerRoutes);
                            if (switchesInProcessRoutes.Count > routeBatchSize)
                            {
                                FillSwitchInProcessQueues(inputArgument.SwitchesInProcess, switchesInProcessRoutes);
                                switchesInProcessRoutes = new List<CustomerRoute>();
                            }
                        }

                        customerRoutesBatch.CustomerRoutes.AddRange(customerRoutes);
                        inputArgument.OutputQueue.Enqueue(customerRoutesBatch);
                        customerRoutesBatch = new CustomerRoutesBatch();
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            if (inputArgument.GenerateAnalysisData)
            {
                List<CustomerRouteMarginStaging> customerRouteMarginStagingList = GetCustomerRouteMarginStagingList(saleZoneOptionsMarginStagingByCustomerSaleZone);
                if (customerRouteMarginStagingList != null && customerRouteMarginStagingList.Count > 0)
                    new CustomerRouteMarginStagingManager().InsertCustomerRouteMarginStagingListToDB(inputArgument.RoutingDatabaseType, customerRouteMarginStagingList);
            }

            if (customerRoutesBatch.CustomerRoutes.Count > 0)
            {
                if (inputArgument.SwitchesInProcess != null && inputArgument.SwitchesInProcess.Count > 0)
                    switchesInProcessRoutes.AddRange(customerRoutesBatch.CustomerRoutes);

                inputArgument.OutputQueue.Enqueue(customerRoutesBatch);
            }

            if (switchesInProcessRoutes.Count > 0)
                FillSwitchInProcessQueues(inputArgument.SwitchesInProcess, switchesInProcessRoutes);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Building Customer Routes is done", null);
        }

        private List<CustomerRouteMarginStaging> GetCustomerRouteMarginStagingList(Dictionary<CustomerSaleZone, SaleZoneOptionsMarginStaging> saleZoneOptionsMarginStagingByCustomerSaleZone)
        {
            if (saleZoneOptionsMarginStagingByCustomerSaleZone == null || saleZoneOptionsMarginStagingByCustomerSaleZone.Count == 0)
                return null;

            Dictionary<CustomerRouteMarginIdentifier, CustomerRouteMarginStaging> results = new Dictionary<CustomerRouteMarginIdentifier, CustomerRouteMarginStaging>();

            foreach (var kvp in saleZoneOptionsMarginStagingByCustomerSaleZone)
            {
                CustomerSaleZone customerSaleZone = kvp.Key;
                SaleZoneOptionsMarginStaging saleZoneOptionsMarginStaging = kvp.Value;

                foreach (var codeOptionsMarginStaging in saleZoneOptionsMarginStaging.CodeOptionsMarginStagingList)
                {
                    foreach (var customerRouteOptionMarginBySupplierZone in codeOptionsMarginStaging.CustomerRouteOptionMarginStagingBySupplierZone)
                    {
                        long supplierZoneId = customerRouteOptionMarginBySupplierZone.Key;
                        CustomerRouteOptionMarginStaging customerRouteOptionMarginStaging = customerRouteOptionMarginBySupplierZone.Value;

                        var customerRouteMarginIdentifier = new CustomerRouteMarginIdentifier()
                        {
                            CustomerID = customerSaleZone.CustomerId,
                            SaleZoneID = customerSaleZone.SaleZoneId,
                            SupplierZoneID = supplierZoneId
                        };

                        if (results.TryGetValue(customerRouteMarginIdentifier, out CustomerRouteMarginStaging customerRouteMarginStaging))
                        {
                            if (customerRouteMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierRate < customerRouteOptionMarginStaging.SupplierRate)
                            {
                                customerRouteMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierZoneID = customerRouteOptionMarginStaging.SupplierZoneID;
                                customerRouteMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierServiceIDs = customerRouteOptionMarginStaging.SupplierServiceIDs;
                                customerRouteMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierRate = customerRouteOptionMarginStaging.SupplierRate;
                                customerRouteMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierDealID = customerRouteOptionMarginStaging.SupplierDealID;
                            }

                            customerRouteMarginStaging.Codes.Add(codeOptionsMarginStaging.Code);
                        }
                        else
                        {
                            results.Add(customerRouteMarginIdentifier, new CustomerRouteMarginStaging()
                            {
                                CustomerID = customerSaleZone.CustomerId,
                                SaleZoneID = customerSaleZone.SaleZoneId,
                                SaleRate = saleZoneOptionsMarginStaging.SaleRate,
                                SaleDealID = saleZoneOptionsMarginStaging.SaleDealID,
                                Codes = new HashSet<string>() { codeOptionsMarginStaging.Code },
                                CustomerRouteOptionMarginStaging = customerRouteOptionMarginStaging,
                                OptimalCustomerRouteOptionMarginStaging = new CustomerRouteOptionMarginStaging()
                                {
                                    SupplierZoneID = codeOptionsMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierZoneID,
                                    SupplierServiceIDs = codeOptionsMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierServiceIDs,
                                    SupplierRate = codeOptionsMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierRate,
                                    SupplierDealID = codeOptionsMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierDealID
                                }
                            });
                        }
                    }
                }
            }

            return results.Values.ToList();
        }

        protected override BuildCustomerRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new BuildCustomerRoutesInput
            {
                CustomerZoneDetails = this.CustomerZoneDetails.Get(context),
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                EffectiveDate = this.EffectiveDate.Get(context),
                RoutingDatabaseType = this.RoutingDatabaseType.Get(context),
                IsFuture = this.IsFuture.Get(context),
                GenerateAnalysisData = this.GenerateAnalysisData.Get(context),
                ActiveRoutingCustomerInfos = this.ActiveRoutingCustomerInfos.Get(context),
                VersionNumber = this.VersionNumber.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                SwitchesInProcess = this.SwitchesInProcess.Get(context),
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<CustomerRoutesBatch>());

            base.OnBeforeExecute(context, handle);
        }

        private void InitialiseSwitchesInProcessQueues(List<SwitchInProcess> switchesInProcess)
        {
            foreach (SwitchInProcess switchInProcess in switchesInProcess)
            {
                if (switchInProcess.RouteQueue == null)
                    switchInProcess.RouteQueue = new MemoryQueue<RouteBatch>();
            }
        }

        private void FillSwitchInProcessQueues(List<SwitchInProcess> switchesInProcess, IEnumerable<CustomerRoute> customerRoutes)
        {
            RouteBatch routeBatch = new RouteBatch() { Routes = Helper.BuildRoutesFromCustomerRoutes(customerRoutes) };

            foreach (SwitchInProcess switchInProcess in switchesInProcess)
            {
                switchInProcess.RouteQueue.Enqueue(routeBatch);
            }
        }
    }
}