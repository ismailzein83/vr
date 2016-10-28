﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.WhS.Routing.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class RPBuildRoutesInput
    {
        public IEnumerable<SupplierZoneToRPOptionPolicy> SupplierZoneRPOptionPolicies { get; set; }

        public BaseQueue<RPCodeMatchesByZone> InputQueue { get; set; }

        public BaseQueue<RPRouteBatch> OutputQueue { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public bool IsFuture { get; set; }

        public bool IncludeBlockedSupplierZones { get; set; }
    }

    public class BuildRoutingProductRoutesContext : IBuildRoutingProductRoutesContext
    {
        public BuildRoutingProductRoutesContext(RPCodeMatchesByZone codeMatch, IEnumerable<RoutingProduct> routingProducts, IEnumerable<SupplierZoneToRPOptionPolicy> policies, DateTime? effectiveDate, bool isFuture)
        {
            this.RoutingProducts = routingProducts;
            this.SupplierCodeMatches = codeMatch.SupplierCodeMatches.ToList();
            this.SupplierCodeMatchesBySupplier = codeMatch.SupplierCodeMatchesBySupplier;
            this.SupplierZoneToRPOptionPolicies = policies;
            this.EntitiesEffectiveOn = effectiveDate;
            this.EntitiesEffectiveInFuture = isFuture;
        }

        public IEnumerable<RoutingProduct> RoutingProducts { get; set; }
        public List<SupplierCodeMatchWithRate> SupplierCodeMatches { get; set; }
        public SupplierCodeMatchesWithRateBySupplier SupplierCodeMatchesBySupplier { get; set; }
        public IEnumerable<SupplierZoneToRPOptionPolicy> SupplierZoneToRPOptionPolicies { get; set; }
        public DateTime? EntitiesEffectiveOn { get; set; }
        public bool EntitiesEffectiveInFuture { get; set; }
    }

    public sealed class RPBuildRoutes : DependentAsyncActivity<RPBuildRoutesInput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<SupplierZoneToRPOptionPolicy>> SupplierZoneRPOptionPolicies { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RPCodeMatchesByZone>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<RPRouteBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<bool> IncludeBlockedSupplierZones { get; set; }


        protected override void DoWork(RPBuildRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            RoutingProductManager routingProductManager = new RoutingProductManager();
            RPRouteBatch productRoutesBatch = new RPRouteBatch();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedRPCodeMatch) =>
                    {
                        IEnumerable<int> routingProductIds = routingProductManager.GetRoutingProductIdsBySaleZoneId(preparedRPCodeMatch.SaleZoneId);
                        Dictionary<int, RoutingProduct> matchingRoutingProducts = routingProductManager.GetAllRoutingProductsByIds(routingProductIds);

                        if (matchingRoutingProducts != null)
                        {
                            BuildRoutingProductRoutesContext routingProductContext = new BuildRoutingProductRoutesContext(preparedRPCodeMatch, matchingRoutingProducts.Values, inputArgument.SupplierZoneRPOptionPolicies, inputArgument.EffectiveDate, inputArgument.IsFuture);
                            RouteBuilder builder = new RouteBuilder();
                            IEnumerable<RPRoute> productRoutes = builder.BuildRoutes(routingProductContext, preparedRPCodeMatch.SaleZoneId, inputArgument.IncludeBlockedSupplierZones);

                            productRoutesBatch.RPRoutes.AddRange(productRoutes);
                            inputArgument.OutputQueue.Enqueue(productRoutesBatch);
                            productRoutesBatch = new RPRouteBatch();
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Building RP Routes is done", null);
        }

        protected override RPBuildRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new RPBuildRoutesInput()
            {
                InputQueue = this.InputQueue.Get(context),
                SupplierZoneRPOptionPolicies = this.SupplierZoneRPOptionPolicies.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                EffectiveDate = this.EffectiveDate.Get(context),
                IsFuture = this.IsFuture.Get(context),
                IncludeBlockedSupplierZones = this.IncludeBlockedSupplierZones.Get(context),
            };
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<RPRouteBatch>());

            base.OnBeforeExecute(context, handle);
        }
    }
}
