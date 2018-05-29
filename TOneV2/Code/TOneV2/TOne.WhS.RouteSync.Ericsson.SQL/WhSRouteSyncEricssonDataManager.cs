using System;
using Vanrise.Data.SQL;
using TOne.WhS.RouteSync.Ericsson.Data;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
	public class WhSRouteSyncEricssonDataManager : BaseSQLDataManager, IWhSRouteSyncEricssonDataManager
    {
        public string SwitchId { get; set; }

        public WhSRouteSyncEricssonDataManager()
            : base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
        {

        }

        public void Initialize(IWhSRouteSyncEricssonInitializeContext context)
        {
            string query = string.Format(query_CreateSchema, SwitchId);
            ExecuteNonQueryText(query, null);
        }

        const string query_CreateSchema = @"IF  NOT EXISTS( SELECT * FROM sys.schemas s WHERE s.name = 'WhS_RouteSync_Ericsson_{0}')
		                                            begin
			                                             EXEC ('CREATE SCHEMA WhS_RouteSync_Ericsson_{0}')
		                                            end";
    }
}