using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface ICustomerRouteMarginSummaryDataManager : IDataManager, IBulkApplyDataManager<CustomerRouteMarginSummary>
    {
        void CreateCustomerRouteMarginSummaryTempTable(RoutingDatabaseType routingDatabaseType);

        void InsertCustomerRouteMarginSummariesToDB(RoutingDatabaseType routingDatabaseType, List<CustomerRouteMarginSummary> customerRouteMarginSummaryList);

        void CreateIndexes(RoutingDatabaseType routingDatabaseType, Action<string> trackStep);

        void SwapTables(RoutingDatabaseType routingDatabaseType);
    }
}