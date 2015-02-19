using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.LCR.Entities;

namespace TOne.LCR.Data.SQL
{
    public class RoutingDatabaseDataManager : BaseTOneDataManager, IRoutingDatabaseDataManager
    {
        public RoutingDatabaseDataManager() : base("TransactionDBConnString")
        {

        }

        public int CreateDatabase(string name, RoutingDatabaseType type, DateTime effectiveTime)
        {
            object obj;
            if (ExecuteNonQuerySP("LCR.sp_RoutingDatabase_Insert", out obj, name, (int)type, effectiveTime) > 0)
            {
                int databaseId = (int)obj;
                RoutingDataManager routingDataManager = new RoutingDataManager();
                routingDataManager.DatabaseId = databaseId;
                ExecuteNonQueryText(String.Format(@"CREATE DATABASE {0}", routingDataManager.GetDatabaseName()), null);
                routingDataManager.CreateDatabaseSchema();
                return databaseId;
            }                
            else
                throw new Exception(String.Format("Could not add Routing Database '{0}' to database table", name));
        }

        public bool SetReady(int databaseId)
        {
            return ExecuteNonQuerySP("LCR.sp_RoutingDatabase_SetReady", databaseId) > 0;
        }

        public int GetIDByType(RoutingDatabaseType type, DateTime effectiveBefore)
        {
            object id = ExecuteScalarSP("LCR.sp_RoutingDatabaseInfo_GetIDByType", (int)type, effectiveBefore);
            if (id != null)
                return (int)id;
            else
                return 0;
        }
    }
}
