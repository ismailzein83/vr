using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.LCR.Entities;

namespace TOne.LCR.Data.SQL
{
    public class RoutingDatabaseDataManager : BaseTOneDataManager, IRoutingDatabaseDataManager
    {
        public int CreateDatabase(string name, RoutingDatabaseType type, DateTime effectiveTime)
        {
            object obj;
            if (ExecuteNonQuerySP("LCR.sp_RoutingDatabase_Insert", out obj, name, (int)type, effectiveTime) > 0)
            {
                int databaseId = (int)obj;
                RoutingDataManager routingDataManager = new RoutingDataManager();
                routingDataManager.DatabaseId = databaseId;
                routingDataManager.CreateDatabase();
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
            object id = ExecuteScalarSP("LCR.sp_RoutingDatabase_GetReadyDBIDByType", (int)type, effectiveBefore);
            if (id != null)
                return (int)id;
            else
                return 0;
        }


        public List<RoutingDatabase> GetNotDeletedDatabases()
        {
            return GetItemsSP("[LCR].[sp_RoutingDatabase_GetNotDeleted]", RoutingDatabaseMapper);
        }

        public void DropDatabase(int databaseId)
        {
            RoutingDataManager routingDataManager = new RoutingDataManager();
            routingDataManager.DatabaseId = databaseId;
            routingDataManager.DropDatabaseIfExists();            
            ExecuteNonQuerySP("[LCR].[sp_RoutingDatabase_Delete]", databaseId);
        }

        #region Private Methods

        private RoutingDatabase RoutingDatabaseMapper(IDataReader reader)
        {
            return new RoutingDatabase
            {
                ID = (int)reader["ID"],
                Title = reader["Title"] as string,
                IsReady = GetReaderValue<bool>(reader, "IsReady"),
                Type = (RoutingDatabaseType)reader["Type"],
                EffectiveTime = (DateTime)reader["EffectiveTime"],
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                ReadyTime = GetReaderValue<DateTime>(reader, "ReadyTime")
            };
        }

        #endregion
    }
}
