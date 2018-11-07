using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei.SQL
{
    public class RouteDataManager : BaseSQLDataManager, IRouteDataManager
    {
        #region Properties/Ctor

        const string RouteTableName = "Route";
        const string RouteTempTableName = "Route_temp";
        const string RouteAddedTableName = "Route_Added";
        const string RouteUpdatedTableName = "Route_Updated";
        const string RouteDeletedTableName = "Route_Deleted";

        readonly string[] columns = { "RSSN", "Code", "RSName", "DNSet" };

        public string SwitchId { get; set; }

        public RouteDataManager()
            : base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void Initialize(IRouteInitializeContext context)
        {
            Guid guid = Guid.NewGuid();

            string createTempTableQuery = string.Format(query_CreateRouteTempTable, SwitchId, guid, RouteTempTableName);
            ExecuteNonQueryText(createTempTableQuery, null);

            string createTableQuery = string.Format(query_CreateRouteTable, SwitchId, guid, RouteTableName);
            ExecuteNonQueryText(createTableQuery, null);

            #region Added
            string syncWithAddedDataQuery = string.Format(query_SyncWithRouteAddedTable, SwitchId, RouteTableName, RouteAddedTableName);
            ExecuteNonQueryText(syncWithAddedDataQuery, null);

            string createAddedTableQuery = string.Format(query_CreateSucceedRouteTable, SwitchId, guid, RouteAddedTableName);
            ExecuteNonQueryText(createAddedTableQuery, null);
            #endregion

            #region Updated
            string syncWithUpdatedDataQuery = string.Format(query_SyncWithRouteUpdatedTable, SwitchId, RouteTableName, RouteUpdatedTableName);
            ExecuteNonQueryText(syncWithUpdatedDataQuery, null);

            string createUpdatedTableQuery = string.Format(query_CreateSucceedRouteTable, SwitchId, guid, RouteUpdatedTableName);
            ExecuteNonQueryText(createUpdatedTableQuery, null);
            #endregion

            #region Deleted
            string syncWithDeletedDataQuery = string.Format(query_SyncWithRouteDeletedTable, SwitchId, RouteTableName, RouteDeletedTableName);
            ExecuteNonQueryText(syncWithDeletedDataQuery, null);

            string createDeletedTableQuery = string.Format(query_CreateSucceedRouteTable, SwitchId, guid, RouteDeletedTableName);
            ExecuteNonQueryText(createDeletedTableQuery, null);
            #endregion
        }

        public void CompareTables(IRouteCompareTablesContext context)
        {
            var differences = new Dictionary<HuaweiConvertedRouteIdentifier, List<HuaweiConvertedRouteByCompare>>();

            string query = string.Format(query_CompareRouteTables, SwitchId, RouteTableName, RouteTempTableName);
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    var convertedRouteByCompare = new HuaweiConvertedRouteByCompare() { HuaweiConvertedRoute = HuaweiConvertedRouteMapper(reader), TableName = reader["tableName"] as string };

                    var routeIdentifier = new HuaweiConvertedRouteIdentifier()
                    {
                        RSSN = convertedRouteByCompare.HuaweiConvertedRoute.RSSN,
                        Code = convertedRouteByCompare.HuaweiConvertedRoute.Code,
                        DNSet = convertedRouteByCompare.HuaweiConvertedRoute.DNSet
                    };
                    List<HuaweiConvertedRouteByCompare> tempRouteDifferences = differences.GetOrCreateItem(routeIdentifier);
                    tempRouteDifferences.Add(convertedRouteByCompare);
                }
            }, null);


            if (differences.Count == 0)
                return;

            var differencesByRSSN = new Dictionary<int, HuaweiConvertedRouteDifferences>();

            foreach (var differenceKvp in differences)
            {
                var routeDifferences = differenceKvp.Value;
                var difference = differencesByRSSN.GetOrCreateItem(differenceKvp.Key.RSSN);

                if (routeDifferences.Count == 1)
                {
                    var singleRouteDifference = differenceKvp.Value[0];
                    if (singleRouteDifference.TableName == RouteTableName)
                        difference.RoutesToDelete.Add(new HuaweiConvertedRouteCompareResult() { Route = singleRouteDifference.HuaweiConvertedRoute });
                    else
                        difference.RoutesToAdd.Add(new HuaweiConvertedRouteCompareResult() { Route = singleRouteDifference.HuaweiConvertedRoute });
                }
                else //routeDifferences.Count = 2
                {
                    var newRoute = routeDifferences.FindRecord(item => (string.Compare(item.TableName, RouteTempTableName, true) == 0));
                    var existingRoute = routeDifferences.FindRecord(item => (string.Compare(item.TableName, RouteTableName, true) == 0));

                    difference.RoutesToUpdate.Add(new HuaweiConvertedRouteCompareResult()
                    {
                        Route = newRoute.HuaweiConvertedRoute,
                        ExistingRoute = existingRoute.HuaweiConvertedRoute
                    });
                }
            }

            context.RouteDifferencesByRSSN = differencesByRSSN;
        }

        public void InsertRoutesToTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            if (routes != null && routes.Any())
            {
                object dbApplyStream = InitialiazeStreamForDBApply();
                foreach (var route in routes)
                {
                    WriteRecordToStream(route, dbApplyStream);
                }
                object obj = FinishDBApplyStream(dbApplyStream);
                ApplyRouteForDB(obj);
            }
        }

        public void UpdateRoutesInTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            if (routes == null || !routes.Any())
                return;

            ExecuteNonQueryText(query_CreateHuaweiRouteTableType, null);
            DataTable dtRoutes = BuildRouteTable(routes);
            string query = string.Format(query_UpdateTempTable, SwitchId, RouteTempTableName, RouteTableName);
            ExecuteNonQueryText(query, (cmd) =>
            {
                var dtPrm = new SqlParameter("@Routes", SqlDbType.Structured);
                dtPrm.TypeName = "HuaweiRouteTableType";
                dtPrm.Value = dtRoutes;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public void RemoveRoutesFromTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            if (routes == null || !routes.Any())
                return;

            ExecuteNonQueryText(query_CreateHuaweiRouteTableType, null);
            DataTable dtRoutes = BuildRouteTable(routes);
            string query = string.Format(query_DeleteFromTempTable, SwitchId, RouteTempTableName);
            ExecuteNonQueryText(query, (cmd) =>
            {
                var dtPrm = new SqlParameter("@Routes", SqlDbType.Structured);
                dtPrm.TypeName = "HuaweiRouteTableType";
                dtPrm.Value = dtRoutes;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public void Finalize(IRouteFinalizeContext context)
        {
            string query = string.Format(query_SwapTables, SwitchId, RouteTableName, RouteTempTableName);
            ExecuteNonQueryText(query, null);

            string deleteAddedTabeQuery = string.Format(query_DeleteRouteTable, SwitchId, RouteAddedTableName);
            ExecuteNonQueryText(deleteAddedTabeQuery, null);

            string deleteUpdatedTabeQuery = string.Format(query_DeleteRouteTable, SwitchId, RouteUpdatedTableName);
            ExecuteNonQueryText(deleteUpdatedTabeQuery, null);

            string deleteDeletedTabeQuery = string.Format(query_DeleteRouteTable, SwitchId, RouteDeletedTableName);
            ExecuteNonQueryText(deleteDeletedTabeQuery, null);
        }

        public void ApplyRouteForDB(object preparedRoute)
        {
            InsertBulkToTable(preparedRoute as BaseBulkInsertInfo);
        }

        #endregion

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(HuaweiConvertedRoute record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}", record.RSSN, record.Code, record.RSName, record.DNSet);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = string.Format("[WhS_RouteSync_Huawei_{0}].[{1}]", SwitchId, RouteTempTableName),
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        #endregion

        #region Private Methods

        private HuaweiConvertedRoute HuaweiConvertedRouteMapper(IDataReader reader)
        {
            return new HuaweiConvertedRoute()
            {
                RSSN = (int)reader["RSSN"],
                Code = reader["Code"] as string,
                RSName = reader["RSName"] as string,
                DNSet = (int)reader["DNSet"]
            };
        }

        private DataTable BuildRouteTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            DataTable dtRoutes = new DataTable();
            dtRoutes.Columns.Add("RSSN", typeof(int));
            dtRoutes.Columns.Add("Code", typeof(string));
            dtRoutes.Columns.Add("RSName", typeof(string));
            dtRoutes.Columns.Add("DNSet", typeof(int));

            dtRoutes.BeginLoadData();
            foreach (var route in routes)
            {
                DataRow dr = dtRoutes.NewRow();
                dr["RSSN"] = route.RSSN;
                dr["Code"] = route.Code;
                dr["RSName"] = route.RSName;
                dr["DNSet"] = route.DNSet;
                dtRoutes.Rows.Add(dr);
            }
            dtRoutes.EndLoadData();
            return dtRoutes;
        }

        #endregion

        #region Private Classes

        private class HuaweiConvertedRouteByCompare
        {
            public HuaweiConvertedRoute HuaweiConvertedRoute { get; set; }

            public string TableName { get; set; }
        }

        #endregion

        #region Queries

        const string query_CreateRouteTempTable = @"IF EXISTS(SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{2}') AND s.type in (N'U'))
                                                    BEGIN
                                                        DROP TABLE WhS_RouteSync_Huawei_{0}.{2}
                                                    END
                                                    
                                                    CREATE TABLE [WhS_RouteSync_Huawei_{0}].[{2}](
                                                          [RSSN] int NOT NULL,
	                                                      [Code] varchar(20) NOT NULL,
	                                                      [RSName] varchar(max) NOT NULL,
                                                          [DNSet] int NOT NULL
                                                    CONSTRAINT [PK_WhS_RouteSync_Huawei_{0}.{2}{1}] PRIMARY KEY CLUSTERED 
                                                    (
                                                        [RSSN] ASC, [Code] ASC
                                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
													) ON [PRIMARY]";

        const string query_CreateRouteTable = @"IF NOT EXISTS(SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{2}') AND s.type in (N'U'))
                                                BEGIN
                                                    CREATE TABLE [WhS_RouteSync_Huawei_{0}].[{2}](
                                                          [RSSN] int NOT NULL,
	                                                      [Code] varchar(20) NOT NULL,
	                                                      [RSName] varchar(max) NOT NULL,
                                                          [DNSet] int NOT NULL
                                                    CONSTRAINT [PK_WhS_RouteSync_Huawei_{0}.{2}{1}] PRIMARY KEY CLUSTERED 
                                                    (
                                                        [RSSN] ASC, [Code] ASC
                                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                    ) ON [PRIMARY]
												END";

        const string query_CreateSucceedRouteTable = @"CREATE TABLE [WhS_RouteSync_Huawei_{0}].[{2}](
                                                          [RSSN] int NOT NULL,
	                                                      [Code] varchar(20) NOT NULL,
	                                                      [RSName] varchar(max) NOT NULL,
                                                          [DNSet] int NOT NULL
                                                       CONSTRAINT [PK_WhS_RouteSync_Huawei_{0}.{2}{1}] PRIMARY KEY CLUSTERED 
                                                       (
                                                           [RSSN] ASC, [Code] ASC
                                                       )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                       ) ON [PRIMARY]";

        const string query_CreateHuaweiRouteTableType = @"IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'HuaweiRouteTableType')
                                                          BEGIN
                                                              CREATE TYPE [HuaweiRouteTableType] AS TABLE(
	                                                          [RSSN] int NOT NULL,
	                                                          [Code] [varchar](20) NOT NULL,
	                                                          [RSName] [varchar](max) NOT NULL, 
                                                              [DNSet] int NOT NULL
                                                              PRIMARY KEY CLUSTERED 
                                                              (
											                     [RSSN] ASC, [Code] ASC
                                                              )WITH (IGNORE_DUP_KEY = OFF))
                                                          END";

        const string query_SyncWithRouteDeletedTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{2}') AND s.type in (N'U')) 
														 BEGIN
														 	DELETE WhS_RouteSync_Huawei_{0}.{1} 
														 	FROM WhS_RouteSync_Huawei_{0}.{1} as routes 
                                                            JOIN WhS_RouteSync_Huawei_{0}.{2} as deletedRoutes  
														 	ON routes.RSSN = deletedRoutes.RSSN and routes.Code = deletedRoutes.Code 
                                                         
														 	DROP TABLE WhS_RouteSync_Huawei_{0}.{2} 
														 END";

        const string query_SyncWithRouteUpdatedTable = @"IF EXISTS(SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{2}') AND s.type in (N'U'))
                                                         BEGIN
														     MERGE INTO WhS_RouteSync_Huawei_{0}.{1} as routes 
														     USING WhS_RouteSync_Huawei_{0}.{2} as updatedRoutes
														     ON routes.RSSN = updatedRoutes.RSSN and routes.Code = updatedRoutes.Code
														     WHEN MATCHED THEN UPDATE SET routes.RSName = updatedRoutes.RSName, routes.DNSet = updatedRoutes.DNSet;

														     DROP TABLE WhS_RouteSync_Huawei_{0}.{2}
                                                         END";

        const string query_SyncWithRouteAddedTable = @"IF EXISTS(SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{2}') AND s.type in (N'U'))
													   BEGIN
													      BEGIN TRANSACTION
													   	  BEGIN TRY
													   	      INSERT INTO  WhS_RouteSync_Huawei_{0}.{1} (RSSN, Code, RSName, DNSet)
													   	      SELECT RSSN, Code, RSName, DNSet
                                                                    FROM WhS_RouteSync_Huawei_{0}.{2}
													       
													   	      DROP TABLE WhS_RouteSync_Huawei_{0}.{2}
													   	      COMMIT Transaction
													   	  END TRY
                                                              
													   	  BEGIN CATCH
													   	      If @@TranCount>0
													   	  	    ROLLBACK Transaction;
													   	  	    DECLARE @ErrorMessage NVARCHAR(max);
													   	  	    DECLARE @ErrorSeverity INT;
													   	  	    DECLARE @ErrorState INT;
                                                              
													   	  	    SELECT 
													   	  		    @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)),
													   	  		    @ErrorSeverity = ERROR_SEVERITY(),
													   	  		    @ErrorState = ERROR_STATE();
                                                              
													   	  	    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
													   	  END CATCH
													   END";

        const string query_DeleteRouteTable = @"IF EXISTS(SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{1}') AND s.type in (N'U'))
												BEGIN 
												   DROP TABLE WhS_RouteSync_Huawei_{0}.{1} 
												END";

        const string query_SwapTables = @"IF EXISTS(SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{2}') AND s.type in (N'U'))
                                          BEGIN
											IF EXISTS(SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{1}_old') AND s.type in (N'U'))
											BEGIN
											    DROP TABLE WhS_RouteSync_Huawei_{0}.{1}_old
											END

											EXEC sp_rename 'WhS_RouteSync_Huawei_{0}.{1}', '{1}_old';
                                          END
	                                      EXEC sp_rename 'WhS_RouteSync_Huawei_{0}.{2}', '{1}';";

        const string query_UpdateTempTable = @"UPDATE tempRoutes 
                                               SET tempRoutes.RSName = routesToUpdate.RSName, tempRoutes.DNSet = routesToUpdate.DNSet
                                               FROM [WhS_RouteSync_Huawei_{0}].[{1}] as tempRoutes
                                               JOIN @Routes as routesToUpdate on routesToUpdate.RSSN = tempRoutes.RSSN and routesToUpdate.Code = tempRoutes.Code";

        const string query_DeleteFromTempTable = @"DELETE tempRoute
												   FROM WhS_RouteSync_Huawei_{0}.{1} as tempRoute 
                                                   JOIN @Routes as route ON route.RSSN = tempRoute.RSSN and route.Code = tempRoute.Code 
                                                           and route.RSName = tempRoute.RSName and route.DNSet = tempRoute.DNSet ";

        const string query_CompareRouteTables = @"IF EXISTS(SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{1}') AND s.type in (N'U'))
                                                     BEGIN
	                                                     SELECT [RSSN],[Code],[RSName],[DNSet], max(tableName) as tableName FROM (
		                                                     SELECT [RSSN],[Code],[RSName],[DNSet], '{1}' as tableName FROM [WhS_RouteSync_Huawei_{0}].[{1}]
		                                                     UNION ALL
		                                                     SELECT [RSSN],[Code],[RSName],[DNSet], '{2}' as tableName FROM [WhS_RouteSync_Huawei_{0}].[{2}]
	                                                     ) v
	                                                     GROUP BY [RSSN],[Code],[RSName],[DNSet]
	                                                     HAVING COUNT(1) = 1
                                                     END
                                                  ELSE
                                                     BEGIN
	                                                     SELECT [RSSN],[Code],[RSName],[DNSet], '{2}' as tableName FROM [WhS_RouteSync_Huawei_{0}].[{2}]
                                                     END";

        #endregion
    }
}