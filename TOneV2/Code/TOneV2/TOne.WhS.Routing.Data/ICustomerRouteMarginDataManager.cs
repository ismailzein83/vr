using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface ICustomerRouteMarginDataManager : IDataManager, IBulkApplyDataManager<CustomerRouteMargin>
    {
        void CreateCustomerRouteMarginTempTable(RoutingDatabaseType routingDatabaseType);

        void InsertCustomerRouteMarginsToDB(RoutingDatabaseType routingDatabaseType, List<CustomerRouteMargin> customerRouteMargins);

        void CreateIndexes(RoutingDatabaseType routingDatabaseType, Action<string> trackStep);

        void SwapTables(RoutingDatabaseType routingDatabaseType);
    }
}