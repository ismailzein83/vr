using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class CustomerRouteMarginSummaryManager
    {
        public void CreateCustomerRouteMarginSummaryTempTable(RoutingDatabaseType routingDatabaseType, Action<string> trackStep)
        {
            var dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginSummaryDataManager>();
            dataManager.CreateCustomerRouteMarginSummaryTempTable(routingDatabaseType, trackStep);
        }

        public void InsertCustomerRouteMarginSummariesToDB(RoutingDatabaseType routingDatabaseType, List<CustomerRouteMarginSummary> customerRouteMarginSummaryList, Action<string> trackStep)
        {
            var dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginSummaryDataManager>();
            dataManager.InsertCustomerRouteMarginSummariesToDB(routingDatabaseType, customerRouteMarginSummaryList, trackStep);
        }

        public void CreateIndexes(RoutingDatabaseType routingDatabaseType, Action<string> trackStep)
        {
            var dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginSummaryDataManager>();
            dataManager.CreateIndexes(routingDatabaseType, trackStep);
        }

        public void SwapTables(RoutingDatabaseType routingDatabaseType)
        {
            var dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginSummaryDataManager>();
            dataManager.SwapTables(routingDatabaseType);
        }
    }
}