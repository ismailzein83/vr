using System;
using System.Collections.Generic;
using TOne.LCR.Entities;
namespace TOne.LCR.Data
{
    public interface IRoutingDatabaseDataManager : IDataManager
    {
        int CreateDatabase(string name, RoutingDatabaseType type, DateTime effectiveTime, bool isLcrOnly);
        bool SetReady(int databaseId);

        List<RoutingDatabase> GetNotDeletedDatabases();

        void DropDatabase(int databaseId);
    }
}
