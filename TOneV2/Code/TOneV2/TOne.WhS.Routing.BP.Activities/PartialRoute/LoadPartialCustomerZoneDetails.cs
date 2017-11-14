using System;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Queueing;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Data;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class LoadPartialCustomerZoneDetailsInput
    {
        public RoutingDatabase RoutingDatabase { get; set; }

        public HashSet<CustomerRouteDefinition> AffectedCustomerRoutes { get; set; }
    }

    public class LoadPartialCustomerZoneDetailsOutput
    {
        public CustomerZoneDetailByZone CustomerZoneDetails { get; set; }
    }

    public sealed class LoadPartialCustomerZoneDetails : BaseAsyncActivity<LoadPartialCustomerZoneDetailsInput, LoadPartialCustomerZoneDetailsOutput>
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<HashSet<CustomerRouteDefinition>> AffectedCustomerRoutes { get; set; }

        [RequiredArgument]
        public OutArgument<CustomerZoneDetailByZone> CustomerZoneDetails { get; set; }

        protected override LoadPartialCustomerZoneDetailsOutput DoWorkWithResult(LoadPartialCustomerZoneDetailsInput inputArgument, AsyncActivityHandle handle)
        {
            HashSet<CustomerRouteDefinition> affectedCustomerRoutes = inputArgument.AffectedCustomerRoutes;

            HashSet<CustomerSaleZone> customerSaleZones = affectedCustomerRoutes.Select(itm => new CustomerSaleZone() { CustomerId = itm.CustomerId, SaleZoneId = itm.SaleZoneId }).ToHashSet();
            ICustomerZoneDetailsDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerZoneDetailsDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            dataManager.RoutingDatabase = routingDatabaseManager.GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);

            List<CustomerZoneDetail> customerZoneDetails = dataManager.GetCustomerZoneDetails(customerSaleZones);
            CustomerZoneDetailByZone customerZoneDetailByZone = new CustomerZoneDetailByZone();
            foreach (CustomerZoneDetail customerZoneDetail in customerZoneDetails)
            {
                List<CustomerZoneDetail> customerZoneDetailList = customerZoneDetailByZone.GetOrCreateItem(customerZoneDetail.SaleZoneId);
                customerZoneDetailList.Add(customerZoneDetail);
            }
            return new LoadPartialCustomerZoneDetailsOutput() { CustomerZoneDetails = customerZoneDetailByZone };
        }

        protected override LoadPartialCustomerZoneDetailsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadPartialCustomerZoneDetailsInput()
            {
                AffectedCustomerRoutes = this.AffectedCustomerRoutes.Get(context),
                RoutingDatabase = this.RoutingDatabase.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadPartialCustomerZoneDetailsOutput result)
        {
            this.CustomerZoneDetails.Set(context, result.CustomerZoneDetails);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Loading Customer Zone Details is done", null);
        }
    }
}