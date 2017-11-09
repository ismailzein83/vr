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
    public class BuildPartialCustomerRoutesInput
    {
        public CustomerZoneDetailByZone CustomerZoneDetails { get; set; }

        public List<RoutingCodeMatches> RoutingCodeMatchesList { get; set; }

        public BaseQueue<CustomerRoutesBatch> OutputQueue { get; set; }

        public int VersionNumber { get; set; }

        public List<CustomerRoute> AffectedCustomerRoutes { get; set; }

        public DateTime EffectiveDate { get; set; }
    }

    public sealed class BuildPartialCustomerRoutes : BaseAsyncActivity<BuildPartialCustomerRoutesInput>
    {

        [RequiredArgument]
        public InArgument<CustomerZoneDetailByZone> CustomerZoneDetails { get; set; }

        [RequiredArgument]
        public InArgument<List<RoutingCodeMatches>> RoutingCodeMatchesList { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<List<CustomerRoute>> AffectedCustomerRoutes { get; set; }

        [RequiredArgument]
        public InArgument<int> VersionNumber { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CustomerRoutesBatch>> OutputQueue { get; set; }

        protected override void DoWork(BuildPartialCustomerRoutesInput inputArgument, AsyncActivityHandle handle)
        {
            CustomerRoutesBatch customerRoutesBatch = new CustomerRoutesBatch();
            RouteBuilder builder = new RouteBuilder(RoutingProcessType.CustomerRoute);

            Dictionary<string, CustomerZoneDetail> customerZoneDetailsDict = new Dictionary<string, CustomerZoneDetail>();
            if (inputArgument.CustomerZoneDetails != null)
            {
                foreach (var customerZoneDetailByZone in inputArgument.CustomerZoneDetails)
                {
                    List<CustomerZoneDetail> customerZoneDetails = customerZoneDetailByZone.Value;
                    foreach (CustomerZoneDetail customerZoneDetail in customerZoneDetails)
                    {
                        string key = string.Concat(customerZoneDetail.CustomerId, "~", customerZoneDetail.SaleZoneId);
                        customerZoneDetailsDict.Add(key, customerZoneDetail);
                    }
                }
            }

            Dictionary<string, CustomerZoneDetailByZone> zoneDetailsByCode = new Dictionary<string, CustomerZoneDetailByZone>();
            if (inputArgument.AffectedCustomerRoutes != null && inputArgument.CustomerZoneDetails != null)
            {
                foreach (CustomerRoute customerRoute in inputArgument.AffectedCustomerRoutes)
                {
                    List<CustomerZoneDetail> matchZoneDetails = zoneDetailsByCode.GetOrCreateItem(customerRoute.Code).GetOrCreateItem(customerRoute.SaleZoneId);
                    string key = string.Concat(customerRoute.CustomerId, "~", customerRoute.SaleZoneId);

                    CustomerZoneDetail customerZoneDetail = customerZoneDetailsDict.GetRecord(key);
                    if (customerZoneDetail != null)
                        matchZoneDetails.Add(customerZoneDetail);
                }
            }

            if (inputArgument.RoutingCodeMatchesList != null && inputArgument.RoutingCodeMatchesList.Count > 0)
            {
                foreach (RoutingCodeMatches routingCodeMatches in inputArgument.RoutingCodeMatchesList)
                {
                    string code = routingCodeMatches.Code;
                    CustomerZoneDetailByZone customerZoneDetailByZone = zoneDetailsByCode.GetRecord(code);
                    if (customerZoneDetailByZone != null)
                    {
                        BuildCustomerRoutesContext customerRoutesContext = new BuildCustomerRoutesContext(routingCodeMatches, customerZoneDetailByZone, inputArgument.EffectiveDate,
                            false, null, null, inputArgument.VersionNumber, false);

                        IEnumerable<CustomerRoute> customerRoutes = builder.BuildRoutes(customerRoutesContext, routingCodeMatches.Code);

                        customerRoutesBatch.CustomerRoutes.AddRange(customerRoutes);
                        inputArgument.OutputQueue.Enqueue(customerRoutesBatch);
                        customerRoutesBatch = new CustomerRoutesBatch();
                    }
                }
            }

            if (customerRoutesBatch.CustomerRoutes.Count > 0)
                inputArgument.OutputQueue.Enqueue(customerRoutesBatch);


            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Building Customer Routes is done", null);
        }

        protected override BuildPartialCustomerRoutesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new BuildPartialCustomerRoutesInput
            {
                CustomerZoneDetails = this.CustomerZoneDetails.Get(context),
                RoutingCodeMatchesList = this.RoutingCodeMatchesList.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
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