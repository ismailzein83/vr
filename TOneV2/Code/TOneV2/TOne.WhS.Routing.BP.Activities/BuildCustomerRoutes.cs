using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class BuildCustomerRoutesInput
    {
        public CustomerZoneDetailByZone CustomerZoneDetails { get; set; }

        public BaseQueue<CodeMatches> InputQueue { get; set; }

        public BaseQueue<CustomerRoutesBatch> OutputQueue { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public bool IsFuture { get; set; }
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

        [RequiredArgument]
        public InOutArgument<BaseQueue<CustomerRoutesBatch>> OutputQueue { get; set; }

        protected override void DoWork(BuildCustomerRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            CustomerRoutesBatch customerRoutesBatch = new CustomerRoutesBatch();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodeMatch) =>
                    {
                        BuildCustomerRoutesContext customerRoutesContext = new BuildCustomerRoutesContext(preparedCodeMatch, inputArgument.CustomerZoneDetails, inputArgument.EffectiveDate, inputArgument.IsFuture);

                        IEnumerable<SellingProductRoute> sellingProductRoute;
                        RouteBuilder builder = new RouteBuilder();
                        IEnumerable<CustomerRoute> customerRoutes = builder.BuildRoutes(customerRoutesContext, preparedCodeMatch.Code, out sellingProductRoute);

                        customerRoutesBatch.CustomerRoutes.AddRange(customerRoutes);
                        inputArgument.OutputQueue.Enqueue(customerRoutesBatch);
                        customerRoutesBatch = new CustomerRoutesBatch();

                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            if (customerRoutesBatch.CustomerRoutes.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(customerRoutesBatch);
            }
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
                IsFuture = this.IsFuture.Get(context)
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
