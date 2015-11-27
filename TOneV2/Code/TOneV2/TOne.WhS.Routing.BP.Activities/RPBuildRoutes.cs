using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.WhS.Routing.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{
    public class RPBuildRoutesInput
    {
        public IEnumerable<SupplierZoneToRPOptionPolicy> SupplierZoneRPOptionPolicies { get; set; }
        public BaseQueue<RPCodeMatchesByZone> InputQueue { get; set; }

        public BaseQueue<RPRouteBatch> OutputQueue { get; set; }
    }

    public class BuildRoutingProductRoutesContext : IBuildRoutingProductRoutesContext
    {
        public BuildRoutingProductRoutesContext(RPCodeMatchesByZone codeMatch, IEnumerable<int> routingProductIds, IEnumerable<SupplierZoneToRPOptionPolicy> policies)
        {
            RoutingProductIds = routingProductIds;
            this.SupplierCodeMatches = codeMatch.SupplierCodeMatches.ToList();
            this.SupplierCodeMatchesBySupplier = codeMatch.SupplierCodeMatchesBySupplier;
            this.SupplierZoneToRPOptionPolicies = policies; //new List<SupplierZoneToRPOptionPolicy>() { new SupplierZoneToRPOptionHighestRatePolicy() { ConfigId = 1 }, new SupplierZoneToRPOptionLowestRatePolicy() { ConfigId = 2 }, new SupplierZoneToRPOptionAverageRatePolicy() { ConfigId = 3 } };


        }
        public IEnumerable<int> RoutingProductIds { get; set; }
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
                        BuildRoutingProductRoutesContext routingProductContext = new BuildRoutingProductRoutesContext(preparedRPCodeMatch, routingProductIds, inputArgument.SupplierZoneRPOptionPolicies);
                        RouteBuilder builder = new RouteBuilder();
                        IEnumerable<RPRoute> productRoutes = builder.BuildRoutes(routingProductContext, preparedRPCodeMatch.SaleZoneId);

                        foreach (RPRoute productRoute in productRoutes)
                        {
                            productRoutesBatch.RPRoutes.Add(productRoute);
                            if (productRoutesBatch.RPRoutes.Count >= 10)
                            {
                                inputArgument.OutputQueue.Enqueue(productRoutesBatch);
                                productRoutesBatch = new RPRouteBatch();
                            }
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);


            });
        }

        protected override RPBuildRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new RPBuildRoutesInput()
            {
                InputQueue = this.InputQueue.Get(context),
                SupplierZoneRPOptionPolicies = this.SupplierZoneRPOptionPolicies.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
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
