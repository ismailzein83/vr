using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface IRiskyMarginCodeDataManager : IDataManager, IBulkApplyDataManager<RiskyMarginCode>
    {
        void CreateRiskyMarginCodeTempTable(RoutingDatabaseType routingDatabaseType, Action<string> trackStep);

        void InsertRiskyMarginCodesToDB(RoutingDatabaseType customerRouteMarginTableType, List<RiskyMarginCode> riskyMarginCodes, Action<string> trackStep);

        void CreateIndexes(RoutingDatabaseType routingDatabaseType, Action<string> trackStep);

        void SwapTables(RoutingDatabaseType routingDatabaseType);
    }
}