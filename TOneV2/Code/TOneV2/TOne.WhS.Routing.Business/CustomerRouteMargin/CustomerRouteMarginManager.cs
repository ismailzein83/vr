using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class CustomerRouteMarginManager
    {
        public void CreateCustomerRouteMarginTempTable(RoutingDatabaseType routingDatabaseType)
        {
            ICustomerRouteMarginDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginDataManager>();
            dataManager.CreateCustomerRouteMarginTempTable(routingDatabaseType);
        }

        public void InsertCustomerRouteMarginsToDB(RoutingDatabaseType routingDatabaseType, List<CustomerRouteMargin> customerRouteMargins)
        {
            ICustomerRouteMarginDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginDataManager>();
            dataManager.InsertCustomerRouteMarginsToDB(routingDatabaseType, customerRouteMargins);
        }

        public void CreateIndexes(RoutingDatabaseType routingDatabaseType, Action<string> trackStep)
        {
            ICustomerRouteMarginDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginDataManager>();
            dataManager.CreateIndexes(routingDatabaseType, trackStep);
        }

        public void SwapTables(RoutingDatabaseType routingDatabaseType)
        {
            ICustomerRouteMarginDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginDataManager>();
            dataManager.SwapTables(routingDatabaseType);
        }
        public HashSet<string> GetSupplierZoneNames(int supplierId, RoutingDatabaseType routingDatabaseType)
        {
            ICustomerRouteMarginDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteMarginDataManager>();
            return dataManager.GetSupplierZoneNames(supplierId, routingDatabaseType);
        }
    }
}