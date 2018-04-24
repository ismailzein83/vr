using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data;
using Vanrise.Data.SQL;
using TOne.WhS.RouteSync.Ericsson.Data;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
	class RouteSucceededDataManager : BaseSQLDataManager, IRouteSucceededDataManager
	{

		const string RouteSucceededTableName = "Route_Succeeded";
		readonly string[] columns = { "BO", "Code", "RCNumber" };
		public string SwitchId { get; set; }

		public RouteSucceededDataManager()
			: base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
		{ }
		
		public void Finalize(IRouteSucceededFinalizeContext context)
		{
			string query = string.Format(query_DropRouteSucceededTable, SwitchId, RouteSucceededTableName);
			ExecuteNonQueryText(query, null);
		}

		public IEnumerable<EricssonConvertedRoute> GetSucceededRoutes()
		{
			string query = string.Format(query_GetAllRouteSucceeded, SwitchId, RouteSucceededTableName);
			return GetItemsText(query, EricssonConvertedRouteSucceededMapper, null);
		}

		public object FinishDBApplyStream(object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.Close();
			return new StreamBulkInsertInfo
			{
				TableName = string.Format("[WhS_RouteSync_Ericsson_{0}].[{1}]", SwitchId, RouteSucceededTableName),
				Stream = streamForBulkInsert,
				TabLock = true,
				KeepIdentity = false,
				FieldSeparator = '^',
				ColumnNames = columns
			};
		}

		public object InitialiazeStreamForDBApply()
		{
			return base.InitializeStreamForBulkInsert();
		}

		public void WriteRecordToStream(EricssonConvertedRoute record, object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.WriteRecord("{0}^{1}^{2}", record.BO, record.Code, record.RCNumber);
		}

		public void ApplyRouteSucceededForDB(object preparedRoute)
		{
			InsertBulkToTable(preparedRoute as BaseBulkInsertInfo);
		}

		EricssonConvertedRoute EricssonConvertedRouteSucceededMapper(IDataReader reader)
		{
			return new EricssonConvertedRoute()
			{
				BO = reader["BO"] as string,
				Code = reader["Code"] as string,
				RCNumber = (int)reader["RCNumber"]
			};
		}

		#region Queries
		const string query_CreateRouteSucceededTable = @"IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                                    BEGIN
                                                    CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[{2}](
                                                          BO varchar(255) NOT NULL,
	                                                      Code varchar(20) NOT NULL,
	                                                      RCNumber int NOT NULL
                                                    CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.Route_{1}] PRIMARY KEY CLUSTERED 
                                                    (
                                                        BO ASC,
	                                                    Code ASC
                                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                    ) ON [PRIMARY]
                                                    END";

		const string query_DropRouteSucceededTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}') AND s.type in (N'U'))
                                                    BEGIN
                                                        DROP TABLE WhS_RouteSync_Ericsson_{0}.{1}
                                                    END";

		const string query_GetAllRouteSucceeded = @"	SELECT BO, Code, RCNumber
							FROM WhS_RouteSync_Ericsson_{0}.{1}";
		#endregion
	}
}
