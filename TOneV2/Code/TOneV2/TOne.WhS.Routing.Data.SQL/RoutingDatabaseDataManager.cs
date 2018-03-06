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
        public int CreateDatabase(string name, RoutingDatabaseType type, RoutingProcessType processType, DateTime? effectiveTime, RoutingDatabaseInformation information, RoutingDatabaseSettings settings)
        {
            object obj;
            if (ExecuteNonQuerySP("TOneWhS_Routing.sp_RoutingDatabase_Insert", out obj, name, (byte)type, (byte)processType, effectiveTime, information != null ? Vanrise.Common.Serializer.Serialize(information) : null) > 0)
            {
                int databaseId = (int)obj;
                RoutingDataManager routingDataManager = new RoutingDataManager();
                //routingDataManager.DatabaseId = databaseId;
                //routingDataManager.RoutingProcessType = processType;

                if (settings == null)
                    settings = new RoutingDatabaseSettings();

                settings.DatabaseName = routingDataManager.CreateDatabase(databaseId, processType);

                ExecuteNonQuerySP("TOneWhS_Routing.sp_RoutingDatabase_UpdateSettings", databaseId, Vanrise.Common.Serializer.Serialize(settings));
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
        public void DropDatabase(RoutingDatabase routingDatabase)
        {
            RoutingDataManager routingDataManager = new RoutingDataManager();
            routingDataManager.RoutingDatabase = routingDatabase;
            ExecuteNonQuerySP("[TOneWhS_Routing].[sp_RoutingDatabase_Delete]", routingDatabase.ID);
            routingDataManager.DropDatabaseIfExists();
        }

        public bool AreRoutingDatabasesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_Routing.RoutingDatabase", ref updateHandle);
        }

        public RoutingDatabase GetRoutingDatabase(int routingDatabaseId)
        {
            return GetItemSP("[TOneWhS_Routing].[sp_RoutingDatabase_GetById]", RoutingDatabaseMapper, routingDatabaseId);
        }

        RoutingDatabase RoutingDatabaseMapper(IDataReader reader)
        {
            RoutingProcessType processType = GetReaderValue<RoutingProcessType>(reader, "ProcessType");
            return new RoutingDatabase
            {
                ID = (int)reader["ID"],
                Title = reader["Title"] as string,
                IsReady = GetReaderValue<bool>(reader, "IsReady"),
                Type = GetReaderValue<RoutingDatabaseType>(reader, "Type"),
                ProcessType = processType,
                EffectiveTime = GetReaderValue<DateTime?>(reader, "EffectiveTime"),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                ReadyTime = GetReaderValue<DateTime>(reader, "ReadyTime"),
                Information = reader["Information"] != null ? Vanrise.Common.Serializer.Deserialize<RoutingDatabaseInformation>(reader["Information"].ToString()) : null,
                Settings = reader["Settings"] != null ? Vanrise.Common.Serializer.Deserialize<RoutingDatabaseSettings>(reader["Settings"].ToString()) : null
            };
        }
    }
}