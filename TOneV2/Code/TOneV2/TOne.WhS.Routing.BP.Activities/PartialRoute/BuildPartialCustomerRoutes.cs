using System;
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
    public class BuildPartialCustomerRoutesInput
    {
        public CustomerZoneDetailByZone CustomerZoneDetails { get; set; }
        public List<RoutingCodeMatches> RoutingCodeMatchesList { get; set; }
        public BaseQueue<PartialCustomerRoutesBatch> OutputQueue { get; set; }
        public int VersionNumber { get; set; }
        public List<CustomerRouteData> AffectedCustomerRoutes { get; set; }
        public DateTime EffectiveDate { get; set; }
        public RoutingDatabase RoutingDatabase { get; set; }
    }

    public sealed class BuildPartialCustomerRoutes : BaseAsyncActivity<BuildPartialCustomerRoutesInput>
    {
        [RequiredArgument]
        public InArgument<CustomerZoneDetailByZone> CustomerZoneDetails { get; set; }

        [RequiredArgument]
        public InArgument<List<RoutingCodeMatches>> RoutingCodeMatchesList { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<PartialCustomerRoutesBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> VersionNumber { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<List<CustomerRouteData>> AffectedCustomerRoutes { get; set; }

        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }


        protected override void DoWork(BuildPartialCustomerRoutesInput inputArgument, AsyncActivityHandle handle)
        {
            PartialCustomerRoutesBatch customerRoutesBatch = new PartialCustomerRoutesBatch();
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

            Dictionary<string, Dictionary<int, CustomerRouteData>> customerRoutesByCode = new Dictionary<string, Dictionary<int, CustomerRouteData>>();

            Dictionary<string, CustomerZoneDetailByZone> zoneDetailsByCode = new Dictionary<string, CustomerZoneDetailByZone>();
            if (inputArgument.AffectedCustomerRoutes != null && inputArgument.CustomerZoneDetails != null)
            {
                foreach (CustomerRouteData customerRoute in inputArgument.AffectedCustomerRoutes)
                {
                    Dictionary<int, CustomerRouteData> customerRoutesByCustomer = customerRoutesByCode.GetOrCreateItem(customerRoute.Code);
                    customerRoutesByCustomer.Add(customerRoute.CustomerId, customerRoute);

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
                            false, null, null, inputArgument.VersionNumber, false, inputArgument.RoutingDatabase);

                        IEnumerable<CustomerRoute> customerRoutes = builder.BuildRoutes(customerRoutesContext, routingCodeMatches.Code);

                        customerRoutesBatch.CustomerRoutes.AddRange(customerRoutes);
                        customerRoutesBatch.AffectedCustomerRoutes = customerRoutesByCode.GetRecord(code);
                        inputArgument.OutputQueue.Enqueue(customerRoutesBatch);
                        customerRoutesBatch = new PartialCustomerRoutesBatch();
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
                EffectiveDate = this.EffectiveDate.Get(context),
                RoutingDatabase = this.RoutingDatabase.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<PartialCustomerRoutesBatch>());

            base.OnBeforeExecute(context, handle);
        }
    }
}