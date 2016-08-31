using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.Whs.Routing.Data.TOneV1SQL
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
            int databaseId = default(int);
            RoutingDataManager routingDataManager = new RoutingDataManager();
            routingDataManager.CreateDatabase(databaseId, processType);

            return databaseId;
        }

        /// <summary>
        /// Update Routing Ready Status to true.
        /// </summary>
        /// <param name="databaseId">Routing Database Id</param>
        /// <returns>Returns true if operation is success.</returns>
        public bool SetReady(int databaseId)
        {
            return true;
        }

        /// <summary>
        /// Get a list of Routing Databases that are not deleted.
        /// </summary>
        /// <returns>List of available Routing Databases</returns>
        public List<RoutingDatabase> GetNotDeletedDatabases()
        {
            return new List<RoutingDatabase>();
        }

        /// <summary>
        /// Drop Routing Database by Id.
        /// </summary>
        /// <param name="databaseId">Routing Database Id</param>
        public void DropDatabase(RoutingDatabase routingDatabase)
        {

        }

        public bool AreRoutingDatabasesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_Routing.RoutingDatabase", ref updateHandle);
        }

        public RoutingDatabase GetRoutingDatabase(int routingDatabaseId)
        {
            return null;
        }
    }
}