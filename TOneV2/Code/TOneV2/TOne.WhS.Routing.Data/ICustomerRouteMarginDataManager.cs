using System;
using Vanrise.Data;
using TOne.WhS.Routing.Entities;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Data
{
    public interface ICustomerRouteMarginDataManager : IDataManager, IBulkApplyDataManager<CustomerRouteMargin>
    {
        void CreateCustomerRouteMarginTempTable(RoutingDatabaseType routingDatabaseType);

        void InsertCustomerRouteMarginsToDB(RoutingDatabaseType routingDatabaseType, List<CustomerRouteMargin> customerRouteMargins);

        void CreateIndexes(RoutingDatabaseType routingDatabaseType, Action<string> trackStep);

        void SwapTables(RoutingDatabaseType routingDatabaseType);

        HashSet<string> GetSupplierZoneNames(int supplierId, RoutingDatabaseType routingDatabaseType);
    }
}