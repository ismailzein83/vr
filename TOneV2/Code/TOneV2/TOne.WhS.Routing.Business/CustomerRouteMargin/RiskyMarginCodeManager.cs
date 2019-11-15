using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RiskyMarginCodeManager
    {
        public void CreateRiskyMarginCodeTempTable(RoutingDatabaseType routingDatabaseType)
        {
            IRiskyMarginCodeDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRiskyMarginCodeDataManager>();
            dataManager.CreateRiskyMarginCodeTempTable(routingDatabaseType);
        }

        public void InsertRiskyMarginCodesToDB(RoutingDatabaseType routingDatabaseType, List<RiskyMarginCode> riskyMarginCodes)
        {
            IRiskyMarginCodeDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRiskyMarginCodeDataManager>();
            dataManager.InsertRiskyMarginCodesToDB(routingDatabaseType, riskyMarginCodes);
        }

        public void CreateIndexes(RoutingDatabaseType routingDatabaseType, Action<string> trackStep)
        {
            IRiskyMarginCodeDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRiskyMarginCodeDataManager>();
            dataManager.CreateIndexes(routingDatabaseType, trackStep);
        }

        public void SwapTables(RoutingDatabaseType routingDatabaseType)
        {
            IRiskyMarginCodeDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRiskyMarginCodeDataManager>();
            dataManager.SwapTables(routingDatabaseType);
        }
    }
}