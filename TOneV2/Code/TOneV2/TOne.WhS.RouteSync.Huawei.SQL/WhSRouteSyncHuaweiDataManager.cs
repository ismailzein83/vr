using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.Huawei.SQL
{
    public class WhSRouteSyncHuaweiDataManager : BaseSQLDataManager, IWhSRouteSyncHuaweiDataManager
    {
        public string SwitchId { get; set; }

        public WhSRouteSyncHuaweiDataManager()
            : base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
        {

        }

        public void Initialize(IWhSRouteSyncHuaweiInitializeContext context)
        {
            string query = string.Format(query_CreateSchema, SwitchId);
            ExecuteNonQueryText(query, null);
        }

        const string query_CreateSchema = @"IF NOT EXISTS(SELECT * FROM sys.schemas s WHERE s.name = 'WhS_RouteSync_Huawei_{0}')
		                                           BEGIN
			                                            EXEC ('CREATE SCHEMA WhS_RouteSync_Huawei_{0}')
		                                           END";
    }
}