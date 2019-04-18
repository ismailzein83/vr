using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Data;
using Vanrise.Data.RDB;

namespace TOne.WhS.RouteSync.Ericsson.RDB
{
    class WhSRouteSyncEricssonDataManager : IWhSRouteSyncEricssonDataManager
    {
        public string SwitchId { get; set; }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RouteSync", "TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString");
        }

        public void Initialize(IWhSRouteSyncEricssonInitializeContext context)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            queryContext.AddCreateSchemaIfNotExistsQuery().DBSchemaName($"WhS_RouteSync_Ericsson_{SwitchId}");
            queryContext.ExecuteNonQuery();
        }
    }
}
