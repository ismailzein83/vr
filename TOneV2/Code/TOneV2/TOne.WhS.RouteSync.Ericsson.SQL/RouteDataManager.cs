﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TOne.WhS.RouteSync.Ericsson.Data;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
    public class RouteDataManager : BaseSQLDataManager, IRouteDataManager
    {
        const string RouteTableName = "Route";
        const string RouteTempTableName = "Route_temp";
        const string RouteAddedTableName = "Route_Added";
        const string RouteUpdatedTableName = "Route_Updated";
        const string RouteDeletedTableName = "Route_Deleted";

        readonly string[] columns = { "BO", "Code", "RCNumber", "TRD", "NextBTable", "OriginCode", "RouteType" };

        public string SwitchId { get; set; }
        public RouteDataManager()
            : base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
        {

        }

        public void Initialize(IRouteInitializeContext context)
        {
            Guid guid = Guid.NewGuid();
            string createTempTableQuery = string.Format(query_CreateRouteTempTable, SwitchId, guid, RouteTempTableName);
            ExecuteNonQueryText(createTempTableQuery, null);

            string createTableQuery = string.Format(query_CreateRouteTable, SwitchId, guid, RouteTableName);
            ExecuteNonQueryText(createTableQuery, null);

            #region Deleted
            string syncWithDeletedDataQuery = string.Format(query_SyncWithRouteDeletedTable, SwitchId, RouteTableName, RouteDeletedTableName);
            ExecuteNonQueryText(syncWithDeletedDataQuery, null);

            string createDeletedTableQuery = string.Format(query_CreateSucceedRouteTable, SwitchId, guid, RouteDeletedTableName);
            ExecuteNonQueryText(createDeletedTableQuery, null);
            #endregion

            #region Updated
            string syncWithUpdatedDataQuery = string.Format(query_SyncWithRouteUpdatedTable, SwitchId, RouteTableName, RouteUpdatedTableName);
            ExecuteNonQueryText(syncWithUpdatedDataQuery, null);

            string createUpdatedTableQuery = string.Format(query_CreateSucceedRouteTable, SwitchId, guid, RouteUpdatedTableName);
            ExecuteNonQueryText(createUpdatedTableQuery, null);
            #endregion

            #region Added
            string syncWithAddedDataQuery = string.Format(query_SyncWithRouteAddedTable, SwitchId, RouteTableName, RouteAddedTableName);
            ExecuteNonQueryText(syncWithAddedDataQuery, null);

            string createAddedTableQuery = string.Format(query_CreateSucceedRouteTable, SwitchId, guid, RouteAddedTableName);
            ExecuteNonQueryText(createAddedTableQuery, null);
            #endregion
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

        public void CompareTables(IRouteCompareTablesContext context)
        {
            var differencesByBO = new Dictionary<int, EricssonConvertedRouteDifferences>();
            var differences = new Dictionary<EricssonConvertedRouteIdentifier, List<EricssonConvertedRouteByCompare>>();

            string query = string.Format(query_CompareRouteTables, SwitchId, RouteTableName, RouteTempTableName);
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    var convertedRouteByCompare = new EricssonConvertedRouteByCompare() { EricssonConvertedRoute = EricssonConvertedRouteMapper(reader), TableName = reader["tableName"] as string };
                    var routeIdentifier = new EricssonConvertedRouteIdentifier() { BO = convertedRouteByCompare.EricssonConvertedRoute.BO, Code = convertedRouteByCompare.EricssonConvertedRoute.Code, RouteType = convertedRouteByCompare.EricssonConvertedRoute.RouteType, TRD = convertedRouteByCompare.EricssonConvertedRoute.TRD };
                    List<EricssonConvertedRouteByCompare> tempRouteDifferences = differences.GetOrCreateItem(routeIdentifier);
                    tempRouteDifferences.Add(convertedRouteByCompare);
                }
            }, null);

            if (differences.Count > 0)
            {
                foreach (var differenceKvp in differences)
                {
                    var routeDifferences = differenceKvp.Value;
                    var difference = differencesByBO.GetOrCreateItem(differenceKvp.Key.BO);
                    if (routeDifferences.Count == 1)
                    {
                        var singleRouteDifference = differenceKvp.Value[0];
                        if (singleRouteDifference.TableName == RouteTableName)
                            difference.RoutesToDelete.Add(new EricssonConvertedRouteCompareResult() { Route = singleRouteDifference.EricssonConvertedRoute });

                        else
                            difference.RoutesToAdd.Add(new EricssonConvertedRouteCompareResult() { Route = singleRouteDifference.EricssonConvertedRoute });
                    }
                    else //routeDifferences.Count = 2
                    {
                        var route = routeDifferences.FindRecord(item => (string.Compare(item.TableName, RouteTempTableName, true) == 0));
                        var routeOldValue = routeDifferences.FindRecord(item => (string.Compare(item.TableName, RouteTableName, true) == 0));

                        if (route != null)
                        {
                            difference.RoutesToUpdate.Add(new EricssonConvertedRouteCompareResult()
                            {
                                Route = new EricssonConvertedRoute()
                                {
                                    BO = route.EricssonConvertedRoute.BO,
                                    Code = route.EricssonConvertedRoute.Code,
                                    RCNumber = route.EricssonConvertedRoute.RCNumber,
                                    TRD = route.EricssonConvertedRoute.TRD,
                                    NextBTable = route.EricssonConvertedRoute.NextBTable,
                                    OriginCode = route.EricssonConvertedRoute.OriginCode,
                                    RouteType = route.EricssonConvertedRoute.RouteType
                                },
                                OriginalValue = (routeOldValue != null) ? new EricssonConvertedRoute()
                                {
                                    BO = routeOldValue.EricssonConvertedRoute.BO,
                                    Code = routeOldValue.EricssonConvertedRoute.Code,
                                    RCNumber = routeOldValue.EricssonConvertedRoute.RCNumber,
                                    TRD = routeOldValue.EricssonConvertedRoute.TRD,
                                    NextBTable = routeOldValue.EricssonConvertedRoute.NextBTable,
                                    OriginCode = routeOldValue.EricssonConvertedRoute.OriginCode,
                                    RouteType = routeOldValue.EricssonConvertedRoute.RouteType
                                } : null
                            });
                        }
                    }
                }
                context.RouteDifferencesByBO = differencesByBO;
            }
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = string.Format("[WhS_RouteSync_Ericsson_{0}].[{1}]", SwitchId, RouteTempTableName),
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
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", record.BO, record.Code, record.RCNumber, record.TRD, record.NextBTable, record.OriginCode, (int)record.RouteType);
        }

        public void ApplyRouteForDB(object preparedRoute)
        {
            InsertBulkToTable(preparedRoute as BaseBulkInsertInfo);
        }

        public void InsertRoutesToTempTable(IEnumerable<EricssonConvertedRoute> routes)
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

        public void RemoveRoutesFromTempTable(IEnumerable<EricssonConvertedRoute> routes)
        {
            if (routes == null || !routes.Any())
                return;
            ExecuteNonQueryText(query_EricssonRouteTableType, null);
            DataTable dtRoutes = BuildRouteTable(routes);
            string query = string.Format(query_DeleteFromTempTable, SwitchId, RouteTempTableName);
            ExecuteNonQueryText(query, (cmd) =>
            {
                var dtPrm = new SqlParameter("@Routes", SqlDbType.Structured);
                dtPrm.TypeName = "EricssonRouteTableType";
                dtPrm.Value = dtRoutes;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public void UpdateRoutesInTempTable(IEnumerable<EricssonConvertedRoute> routes)
        {
            if (routes == null || !routes.Any())
                return;
            ExecuteNonQueryText(query_EricssonRouteTableType, null);
            DataTable dtRoutes = BuildRouteTable(routes);
            string query = string.Format(query_UpdateTempTable, SwitchId, RouteTempTableName, RouteTableName);
            ExecuteNonQueryText(query, (cmd) =>
            {
                var dtPrm = new SqlParameter("@Routes", SqlDbType.Structured);
                dtPrm.TypeName = "EricssonRouteTableType";
                dtPrm.Value = dtRoutes;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public void CopyCustomerRoutesToTempTable(IEnumerable<int> customerBOs)
        {
            if (customerBOs == null || !customerBOs.Any())
                return;
            string filter = string.Format(" Where BO in ({0})", string.Join(",", customerBOs));
            string query = string.Format(query_CopyFromBaseTableToTempTable.Replace("#FILTER#", filter), SwitchId, RouteTableName, RouteTempTableName);
            ExecuteNonQueryText(query, null);
        }

        public Dictionary<int, List<EricssonConvertedRoute>> GetFilteredConvertedRouteByBO(IEnumerable<int> customerBOs)
        {
            var convertedRoutesByBO = new Dictionary<int, List<EricssonConvertedRoute>>();

            string filter = "";

            if (customerBOs != null && customerBOs.Any())
                filter = string.Format(" Where BO in ({0})", string.Join(",", customerBOs));

            string query = string.Format(query_GetFilteredRoute.Replace("#FILTER#", filter), SwitchId, RouteTempTableName);
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    var convertedRoute = EricssonConvertedRouteMapper(reader);
                    List<EricssonConvertedRoute> convertedRoutes = convertedRoutesByBO.GetOrCreateItem(convertedRoute.BO);
                    convertedRoutes.Add(convertedRoute);
                }
            }, null);
            return convertedRoutesByBO;
        }

        private DataTable BuildRouteTable(IEnumerable<EricssonConvertedRoute> routes)
        {
            DataTable dtRoutes = new DataTable();
            dtRoutes.Columns.Add("BO", typeof(int));
            dtRoutes.Columns.Add("Code", typeof(string));
            dtRoutes.Columns.Add("RCNumber", typeof(int));
            dtRoutes.Columns.Add("TRD", typeof(int));
            dtRoutes.Columns.Add("NextBTable", typeof(int?));
            dtRoutes.Columns.Add("OriginCode", typeof(int?));
            dtRoutes.Columns.Add("RouteType", typeof(int));
            dtRoutes.BeginLoadData();
            foreach (var route in routes)
            {
                DataRow dr = dtRoutes.NewRow();
                dr["BO"] = route.BO;
                dr["Code"] = route.Code;
                dr["RCNumber"] = route.RCNumber;
                dr["TRD"] = route.TRD;
                dr["NextBTable"] = route.NextBTable;
                dr["OriginCode"] = route.OriginCode;
                dr["RouteType"] = (int)route.RouteType;
                dtRoutes.Rows.Add(dr);
            }
            dtRoutes.EndLoadData();
            return dtRoutes;
        }

        private class EricssonConvertedRouteByCompare
        {
            public EricssonConvertedRoute EricssonConvertedRoute { get; set; }
            public string TableName { get; set; }
        }

        EricssonConvertedRoute EricssonConvertedRouteMapper(IDataReader reader)
        {
            return new EricssonConvertedRoute()
            {
                BO = (int)reader["BO"],
                Code = reader["Code"] as string,
                RCNumber = (int)reader["RCNumber"],
                TRD = (int)reader["TRD"],
                NextBTable = GetReaderValue<int?>(reader, "NextBTable"),
                OriginCode = reader["OriginCode"] as string,
                RouteType = (EricssonRouteType)GetReaderValue<int>(reader, "RouteType")
            };
        }

        #region Queries

        const string query_CreateRouteTempTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                                    BEGIN
                                                        DROP TABLE WhS_RouteSync_Ericsson_{0}.{2}
                                                    END

                                                    CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[{2}](
                                                        BO int NOT NULL,
	                                                    Code varchar(20) NOT NULL,
	                                                    RCNumber int NOT NULL,
	                                                    TRD int NOT NULL,
                                                        NextBTable int,
                                                        OriginCode varchar(20),
                                                        RouteType int NOT NULL
                                                        CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.Route_{2}{1}] PRIMARY KEY CLUSTERED 
                                                        (
                                                            BO ASC,
                                                            Code ASC,
                                                            RouteType ASC
                                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
													) ON [PRIMARY]";

        const string query_CompareRouteTables = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}') AND s.type in (N'U'))
                                                    BEGIN
                                                        SELECT [BO], [Code], [RCNumber], [TRD], [NextBTable], [OriginCode], [RouteType], max(tableName) as tableName 
                                                            FROM 
                                                                (
                                                                    SELECT [BO], [Code], [RCNumber], [TRD], [NextBTable], [OriginCode], [RouteType], '{1}' as tableName FROM [WhS_RouteSync_Ericsson_{0}].[{1}]
                                                                    UNION ALL
                                                                    SELECT [BO], [Code], [RCNumber], [TRD], [NextBTable], [OriginCode], [RouteType], '{2}' as tableName FROM [WhS_RouteSync_Ericsson_{0}].[{2}]
                                                                ) v
                                                        GROUP BY [BO], [Code], [RCNumber], [TRD], [NextBTable], [OriginCode], [RouteType]
                                                        HAVING COUNT(1)=1
                                                    END
                                                
                                                ELSE
                                                    BEGIN
                                                        SELECT [BO], [Code], [RCNumber], [TRD], [NextBTable], [OriginCode], [RouteType], '{2}' as tableName FROM [WhS_RouteSync_Ericsson_{0}].[{2}]
                                                    END";

        const string query_SwapTables = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                            BEGIN
                                                IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}_old') AND s.type in (N'U'))
                                                    BEGIN
                                                        DROP TABLE WhS_RouteSync_Ericsson_{0}.{1}_old
                                                    END
                                                EXEC sp_rename 'WhS_RouteSync_Ericsson_{0}.{1}', '{1}_old';
                                            END

	                                        EXEC sp_rename 'WhS_RouteSync_Ericsson_{0}.{2}', '{1}';";

        const string query_SyncWithRouteDeletedTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U')) 
															BEGIN
																DELETE WhS_RouteSync_Ericsson_{0}.{1} 
																FROM WhS_RouteSync_Ericsson_{0}.{1} as routes join WhS_RouteSync_Ericsson_{0}.{2} as deletedRoutes  
																ON routes.BO = deletedRoutes.BO and routes.Code = deletedRoutes.Code and routes.RouteType = deletedRoutes.RouteType

																DROP TABLE WhS_RouteSync_Ericsson_{0}.{2} 
															END";

        const string query_SyncWithRouteUpdatedTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                                            BEGIN
														        MERGE INTO WhS_RouteSync_Ericsson_{0}.{1}  as routes 
														        USING WhS_RouteSync_Ericsson_{0}.{2} as updatedRoutes
														        ON routes.BO = updatedRoutes.BO and routes.Code = updatedRoutes.Code and routes.RouteType = updatedRoutes.RouteType
														        WHEN MATCHED THEN
														        UPDATE 
														        SET routes.RCNumber = updatedRoutes.RCNumber,
                                                                    routes.NextBTable = updatedRoutes.NextBTable,
                                                                    routes.TRD = updatedRoutes.TRD;
                                                    
														        DROP TABLE WhS_RouteSync_Ericsson_{0}.{2}
                                                            END";

        const string query_SyncWithRouteAddedTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                                            BEGIN
                                                                BEGIN TRANSACTION
                                                                BEGIN TRY
                                                                    INSERT INTO  WhS_RouteSync_Ericsson_{0}.{1} (BO, Code, RCNumber, TRD, NextBTable, OriginCode, RouteType)
                                                                    SELECT BO, Code, RCNumber, TRD, NextBTable, OriginCode, RouteType  FROM WhS_RouteSync_Ericsson_{0}.{2}
                                                            
                                                                    DROP TABLE WhS_RouteSync_Ericsson_{0}.{2}
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

        const string query_CreateRouteTable = @"IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                                    BEGIN
                                                    CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[{2}]
                                                    (
                                                        BO int NOT NULL,
                                                        Code varchar(20) NOT NULL,
                                                        RCNumber int NOT NULL,
                                                        TRD int NOT NULL,
                                                        NextBTable int,
                                                        OriginCode varchar(20),
                                                        RouteType int NOT NULL
                                                        CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.Route_{2}{1}] PRIMARY KEY CLUSTERED 
                                                        (
                                                            BO ASC,
                                                            Code ASC,
                                                            RouteType ASC
                                                        )
                                                        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                    )
                                                    ON [PRIMARY]
													END";

        const string query_CreateSucceedRouteTable = @"CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[{2}](
                                                            BO int NOT NULL,
                                                            Code varchar(20) NOT NULL,
                                                            RCNumber int NOT NULL,
                                                            TRD int NOT NULL,
                                                            NextBTable int,
                                                            OriginCode varchar(20),
                                                            RouteType int NOT NULL
                                                            CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.{2}{1}] PRIMARY KEY CLUSTERED 
                                                                (
                                                                    BO ASC,
                                                                    Code ASC,
                                                                    RouteType ASC
                                                                )
                                                                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                        )
                                                        ON [PRIMARY]";

        const string query_DeleteRouteTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}') AND s.type in (N'U'))
													BEGIN 
														DROP TABLE WhS_RouteSync_Ericsson_{0}.{1} 
													END";

        const string query_CopyFromBaseTableToTempTable = @"INSERT INTO  WhS_RouteSync_Ericsson_{0}.{2} (BO, Code, RCNumber, TRD, NextBTable, OriginCode, RouteType)
														    SELECT BO, Code, RCNumber, TRD, NextBTable, OriginCode, RouteType FROM WhS_RouteSync_Ericsson_{0}.{1}
														    #FILTER#";

        const string query_EricssonRouteTableType = @"IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'EricssonRouteTableType')
                                                    CREATE TYPE [EricssonRouteTableType] AS TABLE
                                                    (
                                                        [BO] [int] NOT NULL,
                                                        [Code] [varchar](20) NOT NULL,
                                                        [RCNumber] [int] NOT NULL,
                                                        [TRD] [int] NOT NULL,
                                                        [NextBTable] [int],
                                                        [OriginCode] varchar(20),
                                                        [RouteType] [int] NOT NULL
                                                        
                                                        PRIMARY KEY CLUSTERED 
                                                        (
                                                            [BO] ASC,
                                                            [Code] ASC,
                                                            [RouteType] ASC
                                                        )
                                                        WITH (IGNORE_DUP_KEY = OFF)
                                                    )";

        const string query_DeleteFromTempTable = @"DELETE WhS_RouteSync_Ericsson_{0}.{1}
                                                        FROM WhS_RouteSync_Ericsson_{0}.{1} as tempRoute join @Routes as route
                                                        ON route.BO = tempRoute.BO
                                                        and route.Code = tempRoute.Code
                                                        and route.RCNumber = tempRoute.RCNumber
                                                        and route.TRD = tempRoute.TRD
                                                        and route.NextBTable = tempRoute.NextBTable
                                                        and route.OriginCode = tempRoute.OriginCode
                                                        and route.RouteType = tempRoute.RouteType";


        const string query_UpdateTempTable = @"UPDATE  tempRoutes 
                                                SET tempRoutes.RCNumber = routesToUpdate.RCNumber, tempRoutes.TRD = routesToUpdate.TRD, tempRoutes.NextBTable = routesToUpdate.NextBTable
                                                FROM [WhS_RouteSync_Ericsson_{0}].[{1}] as tempRoutes
                                                JOIN @Routes as routesToUpdate on routesToUpdate.BO = tempRoutes.BO and routesToUpdate.Code = tempRoutes.Code and routesToUpdate.RouteType = tempRoutes.RouteType @Routes as routesToUpdate on routesToUpdate.BO = tempRoutes.BO and routesToUpdate.Code = tempRoutes.Code and routesToUpdate.RouteType = tempRoutes.RouteType";

        const string query_GetFilteredRoute = @"Select BO, Code, RCNumber, TRD, NextBTable, OriginCode, RouteType
                                                FROM [WhS_RouteSync_Ericsson_{0}].[{1}]
                                                #FILTER#";

        #endregion
    }
}