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
        int CreateDatabase(string name, RoutingDatabaseType type, DateTime? effectiveTime);
        bool SetReady(int databaseId);
        List<RoutingDatabase> GetNotDeletedDatabases();
        void DropDatabase(int databaseId);
        int GetIDByType(RoutingDatabaseType type, DateTime effectiveBefore);
    }
}
