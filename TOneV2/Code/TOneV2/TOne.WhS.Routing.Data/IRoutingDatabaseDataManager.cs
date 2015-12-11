using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data
{
    public interface IRoutingDatabaseDataManager : IDataManager
    {
        int CreateDatabase(string name, RoutingDatabaseType type, RoutingProcessType processType, DateTime? effectiveTime, RoutingDatabaseInformation information);
        bool SetReady(int databaseId);
        List<RoutingDatabase> GetNotDeletedDatabases();
        List<RoutingDatabase> GetNotDeletedDatabases(RoutingProcessType processType);
        void DropDatabase(int databaseId);
        int GetIDByType(RoutingDatabaseType type, RoutingProcessType processType, DateTime effectiveBefore);

        bool AreRoutingDatabasesUpdated(ref object updateHandle);
    }
}
