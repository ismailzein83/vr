using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data
{
    public interface IRoutingDatabaseDataManager : IDataManager
    {
        int CreateDatabase(string name, RoutingDatabaseType type, RoutingProcessType processType, DateTime? effectiveTime, RoutingDatabaseInformation information, RoutingDatabaseSettings settings, IEnumerable<RoutingCustomerInfo> routingCustomerInfos);
        bool SetReady(int databaseId);
        List<RoutingDatabase> GetNotDeletedDatabases();
        void DropDatabase(RoutingDatabase routingDatabase);
        bool AreRoutingDatabasesUpdated(ref object updateHandle);
        RoutingDatabase GetRoutingDatabase(int routingDatabaseId);
    }
}
