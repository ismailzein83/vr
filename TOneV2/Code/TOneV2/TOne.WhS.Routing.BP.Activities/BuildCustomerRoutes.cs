﻿using System;
using System.Collections.Generic;
using System.Activities;
using TOne.WhS.Routing.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;
using TOne.WhS.RouteSync.BP.Activities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class BuildCustomerRoutesInput
    {
        public CustomerZoneDetailByZone CustomerZoneDetails { get; set; }

        public BaseQueue<CodeMatches> InputQueue { get; set; }

        public BaseQueue<CustomerRoutesBatch> OutputQueue { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public bool IsFuture { get; set; }

        public List<SwitchInProcess> SwitchesInProcess { get; set; }
    }

    public class BuildCustomerRoutesContext : IBuildCustomerRoutesContext
    {
        public List<SaleCodeMatch> SaleCodeMatches { get; set; }

        public CustomerZoneDetailByZone CustomerZoneDetails { get; set; }

        public List<SupplierCodeMatchWithRate> SupplierCodeMatches { get; set; }

        public SupplierCodeMatchWithRateBySupplier SupplierCodeMatchesBySupplier { get; set; }

        public DateTime? EntitiesEffectiveOn { get; set; }

        public bool EntitiesEffectiveInFuture { get; set; }

        public BuildCustomerRoutesContext(CodeMatches codeMatches, CustomerZoneDetailByZone customerZoneDetails, DateTime? effectiveDate, bool isFuture)
        {
            this.SaleCodeMatches = codeMatches.SaleCodeMatches;
            this.SupplierCodeMatches = codeMatches.SupplierCodeMatches;
            this.SupplierCodeMatchesBySupplier = codeMatches.SupplierCodeMatchesBySupplier;
            this.CustomerZoneDetails = customerZoneDetails;
            this.EntitiesEffectiveOn = effectiveDate;
            this.EntitiesEffectiveInFuture = isFuture;
        }
    }

    public sealed class BuildCustomerRoutes : DependentAsyncActivity<BuildCustomerRoutesInput>
    {

        [RequiredArgument]
        public InArgument<CustomerZoneDetailByZone> CustomerZoneDetails { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<CodeMatches>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        public InArgument<List<SwitchInProcess>> SwitchesInProcess { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CustomerRoutesBatch>> OutputQueue { get; set; }

        protected override void DoWork(BuildCustomerRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int routeBatchSize = 0;
            if (inputArgument.SwitchesInProcess != null && inputArgument.SwitchesInProcess.Count > 0)
            {
                InitialiseSwitchesInProcessQueues(inputArgument.SwitchesInProcess);
                RouteSync.Business.ConfigManager routeSyncConfigManager = new RouteSync.Business.ConfigManager();
                routeBatchSize = routeSyncConfigManager.GetRouteSyncProcessSettings().RouteBatchSize;
            }

            CustomerRoutesBatch customerRoutesBatch = new CustomerRoutesBatch();
            List<CustomerRoute> switchesInProcessRoutes = new List<CustomerRoute>();
            RouteBuilder builder = new RouteBuilder();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodeMatch) =>
                    {
                        BuildCustomerRoutesContext customerRoutesContext = new BuildCustomerRoutesContext(preparedCodeMatch, inputArgument.CustomerZoneDetails, inputArgument.EffectiveDate, inputArgument.IsFuture);

                        IEnumerable<SellingProductRoute> sellingProductRoute;
                       
                        IEnumerable<CustomerRoute> customerRoutes = builder.BuildRoutes(customerRoutesContext, preparedCodeMatch.Code, out sellingProductRoute);

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



            if (customerRoutesBatch.CustomerRoutes.Count > 0)
            {
                if (inputArgument.SwitchesInProcess != null && inputArgument.SwitchesInProcess.Count > 0)
                {
                    switchesInProcessRoutes.AddRange(customerRoutesBatch.CustomerRoutes);
                }
                inputArgument.OutputQueue.Enqueue(customerRoutesBatch);
            }

            if (switchesInProcessRoutes.Count > 0)
                FillSwitchInProcessQueues(inputArgument.SwitchesInProcess, switchesInProcessRoutes);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Building Customer Routes is done", null);
        }

        protected override BuildCustomerRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new BuildCustomerRoutesInput
            {
                CustomerZoneDetails = this.CustomerZoneDetails.Get(context),
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                EffectiveDate = this.EffectiveDate.Get(context),
                IsFuture = this.IsFuture.Get(context),
                SwitchesInProcess = this.SwitchesInProcess.Get(context)
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
            RouteBatch routeBatch = new RouteBatch() { Routes = new List<Route>() };
            foreach (CustomerRoute customerRoute in customerRoutes)
            {
                Route route = new Route()
                {
                    Code = customerRoute.Code,
                    CustomerId = customerRoute.CustomerId.ToString(),
                };
                if (customerRoute.Options != null && customerRoute.Options.Count > 0)
                {
                    route.Options = new List<RouteSync.Entities.RouteOption>();
                    foreach (Routing.Entities.RouteOption option in customerRoute.Options)
                    {
                        route.Options.Add(new RouteSync.Entities.RouteOption()
                        {
                            Percentage = option.Percentage,
                            SupplierId = option.SupplierId.ToString()
                        });
                    }
                }
                routeBatch.Routes.Add(route);
            }

            foreach (SwitchInProcess switchInProcess in switchesInProcess)
            {
                switchInProcess.RouteQueue.Enqueue(routeBatch);
            }
        }
    }
}