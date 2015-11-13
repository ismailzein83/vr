using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{

    public class BuildCustomerRoutesInput
    {
        public BaseQueue<CodeMatches> InputQueue { get; set; }

        public BaseQueue<CustomerRoutesBatch> OutputQueue { get; set; }
    }

    public class BuildCustomerRoutesContext : IBuildCustomerRoutesContext
    {
        public List<SaleCodeMatch> SaleCodeMatches { get; set; }

        public CustomerZoneDetailByZone CustomerZoneDetails { get; set; }

        public List<SupplierCodeMatchWithRate> SupplierCodeMatches { get; set; }

        public SupplierCodeMatchWithRateBySupplier SupplierCodeMatchesBySupplier { get; set; }

        public DateTime? EntitiesEffectiveOn { get; set; }

        public bool EntitiesEffectiveInFuture { get; set; }
    }

    public sealed class BuildCustomerRoutes : DependentAsyncActivity<BuildCustomerRoutesInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<CodeMatches>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CustomerRoutesBatch>> OutputQueue { get; set; }

        protected override void DoWork(BuildCustomerRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodeMatch) =>
                    {
                        BuildCustomerRoutesContext customerRoutesContext = new BuildCustomerRoutesContext();

                        IEnumerable<SellingProductRoute> sellingProductRoute;
                        RouteBuilder builder = new RouteBuilder();
                        IEnumerable<CustomerRoute> customerRoutes = builder.BuildRoutes(customerRoutesContext, preparedCodeMatch.Code, out sellingProductRoute);

                        CustomerRoutesBatch customerRoutesBatch = new CustomerRoutesBatch();

                        foreach (CustomerRoute route in customerRoutes)
                        {
                            customerRoutesBatch.CustomerRoutes.Add(route);
                            if (customerRoutesBatch.CustomerRoutes.Count >= 10)
                            {
                                inputArgument.OutputQueue.Enqueue(customerRoutesBatch);
                                customerRoutesBatch = new CustomerRoutesBatch();
                            }
                        }

                        if (customerRoutesBatch.CustomerRoutes.Count > 0)
                        {
                            inputArgument.OutputQueue.Enqueue(customerRoutesBatch);
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override BuildCustomerRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new BuildCustomerRoutesInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<CustomerRoutesBatch>());

            base.OnBeforeExecute(context, handle);
        }
    }
}
