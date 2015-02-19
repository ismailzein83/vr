using System;
using TOne.LCR.Entities;
namespace TOne.LCR.Data
{
    public interface IRoutingDatabaseDataManager : IDataManager
    {
        int CreateDatabase(string name, RoutingDatabaseType type, DateTime effectiveTime);
        bool SetReady(int databaseId);
    }
}
