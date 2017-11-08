using System;
using System.Collections.Generic;
using System.Activities;
using TOne.WhS.Routing.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;
using TOne.WhS.RouteSync.BP.Activities;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.Routing.Business.Extensions;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class BuildPartialCustomerRoutesInput
    {
        public CustomerZoneDetailByZone CustomerZoneDetails { get; set; }

        public BaseQueue<RoutingCodeMatches> InputQueue { get; set; }

        public BaseQueue<CustomerRoutesBatch> OutputQueue { get; set; }

        public IEnumerable<RoutingCustomerInfo> ActiveRoutingCustomerInfos { get; set; }

        public int VersionNumber { get; set; }

        public List<CustomerRoute> AffectedCustomerRoutes { get; set; }

        public DateTime EffectiveDate { get; set; }
    }

    public sealed class BuildPartialCustomerRoutes : DependentAsyncActivity<BuildPartialCustomerRoutesInput>
    {

        [RequiredArgument]
        public InArgument<CustomerZoneDetailByZone> CustomerZoneDetails { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RoutingCodeMatches>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RoutingCustomerInfo>> ActiveRoutingCustomerInfos { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<List<CustomerRoute>> AffectedCustomerRoutes { get; set; }

        [RequiredArgument]
        public InArgument<int> VersionNumber { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CustomerRoutesBatch>> OutputQueue { get; set; }

        protected override void DoWork(BuildPartialCustomerRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            CustomerRoutesBatch customerRoutesBatch = new CustomerRoutesBatch();
            List<CustomerRoute> switchesInProcessRoutes = new List<CustomerRoute>();
            RouteBuilder builder = new RouteBuilder(RoutingProcessType.CustomerRoute);

            Dictionary<int, HashSet<int>> customerCountries = new Dictionary<int, HashSet<int>>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodeMatch) =>
                    {
                        BuildCustomerRoutesContext customerRoutesContext = new BuildCustomerRoutesContext(preparedCodeMatch, inputArgument.CustomerZoneDetails, inputArgument.EffectiveDate,
                            false, inputArgument.ActiveRoutingCustomerInfos, customerCountries, inputArgument.VersionNumber, false);

                        IEnumerable<CustomerRoute> customerRoutes = builder.BuildRoutes(customerRoutesContext, preparedCodeMatch.Code);

                        customerRoutesBatch.CustomerRoutes.AddRange(customerRoutes);
                        inputArgument.OutputQueue.Enqueue(customerRoutesBatch);
                        customerRoutesBatch = new CustomerRoutesBatch();

                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            if (customerRoutesBatch.CustomerRoutes.Count > 0)
                inputArgument.OutputQueue.Enqueue(customerRoutesBatch);


            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Building Customer Routes is done", null);
        }

        protected override BuildPartialCustomerRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new BuildPartialCustomerRoutesInput
            {
                CustomerZoneDetails = this.CustomerZoneDetails.Get(context),
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                ActiveRoutingCustomerInfos = this.ActiveRoutingCustomerInfos.Get(context),
                VersionNumber = this.VersionNumber.Get(context),
                AffectedCustomerRoutes = this.AffectedCustomerRoutes.Get(context),
                EffectiveDate = this.EffectiveDate.Get(context)
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