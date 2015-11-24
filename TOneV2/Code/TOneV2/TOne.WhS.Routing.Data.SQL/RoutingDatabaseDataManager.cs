using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
    public class RoutingDatabaseDataManager : BaseTOneDataManager, IRoutingDatabaseDataManager
    {
        public RoutingDatabaseDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        /// <summary>
        /// Create New Routing Database.
        /// </summary>
        /// <param name="name">Routing Database Name</param>
        /// <param name="type">Routing Database Type</param>
        /// <param name="effectiveTime">Effective Date</param>
        /// <returns>Created Routing Database Id</returns>
        public int CreateDatabase(string name, RoutingDatabaseType type, DateTime? effectiveTime)
        {
            object obj;
            if (ExecuteNonQuerySP("TOneWhS_Routing.sp_RoutingDatabase_Insert", out obj, name, (int)type, effectiveTime) > 0)
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

        /// <summary>
        /// Update Routing Ready Status to true.
        /// </summary>
        /// <param name="databaseId">Routing Database Id</param>
        /// <returns>Returns true if operation is success.</returns>
        public bool SetReady(int databaseId)
        {
            return ExecuteNonQuerySP("TOneWhS_Routing.sp_RoutingDatabase_SetReady", databaseId) > 0;
        }

        /// <summary>
        /// Get a list of Routing Databases that are not deleted.
        /// </summary>
        /// <returns>List of available Routing Databases</returns>
        public List<RoutingDatabase> GetNotDeletedDatabases()
        {
            return GetItemsSP("[TOneWhS_Routing].[sp_RoutingDatabase_GetNotDeleted]", RoutingDatabaseMapper);
        }

        /// <summary>
        /// Drop Routing Database by Id.
        /// </summary>
        /// <param name="databaseId">Routing Database Id</param>
        public void DropDatabase(int databaseId)
        {
            RoutingDataManager routingDataManager = new RoutingDataManager();
            routingDataManager.DatabaseId = databaseId;
            routingDataManager.DropDatabaseIfExists();
            ExecuteNonQuerySP("[TOneWhS_Routing].[sp_RoutingDatabase_Delete]", databaseId);
        }

        /// <summary>
        /// Get Routing Database Id by type and date.
        /// </summary>
        /// <param name="type">Routing database Type.</param>
        /// <param name="effectiveBefore">Effective Date</param>
        /// <returns>Routing Database Id</returns>
        public int GetIDByType(RoutingDatabaseType type, DateTime effectiveBefore)
        {
            object id = ExecuteScalarSP("TOneWhS_Routing.sp_RoutingDatabase_GetReadyDBIDByType", (int)type, effectiveBefore);
            if (id != null)
                return (int)id;
            else
                return 0;
        }

        public bool AreRoutingDatabasesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_Routing.RoutingDatabase", ref updateHandle);
        }

        RoutingDatabase RoutingDatabaseMapper(IDataReader reader)
        {
            return new RoutingDatabase
            {
                ID = (int)reader["ID"],
                Title = reader["Title"] as string,
                IsReady = GetReaderValue<bool>(reader, "IsReady"),
                Type = (RoutingDatabaseType)reader["Type"],
                EffectiveTime = (DateTime?)reader["EffectiveTime"],
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                ReadyTime = GetReaderValue<DateTime>(reader, "ReadyTime")
            };
        }
    }
}
