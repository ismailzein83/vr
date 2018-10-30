using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.Huawei.SQL
{
    public class RouteDataManager : BaseSQLDataManager, IRouteDataManager
    {
        const string RouteTableName = "Route";
        const string RouteTempTableName = "Route_temp";
        const string RouteAddedTableName = "Route_Added";
        const string RouteUpdatedTableName = "Route_Updated";
        const string RouteDeletedTableName = "Route_Deleted";

        readonly string[] columns = { "CustomerId", "Code", "RouteCase" };

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
            throw new NotImplementedException();
        }

        public void InsertRoutesToTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            throw new NotImplementedException();
        }

        public void RemoveRoutesFromTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            throw new NotImplementedException();
        }

        public void UpdateRoutesInTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            throw new NotImplementedException();
        }

        public void Finalize(IRouteFinalizeContext context)
        {
            throw new NotImplementedException();
        }

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(HuaweiConvertedRoute record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}", record.RSSN, record.Code, record.RSName);
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

        #region Queries

        const string query_CreateRouteTempTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{2}') AND s.type in (N'U'))
                                                    BEGIN
                                                        DROP TABLE WhS_RouteSync_Huawei_{0}.{2}
                                                    END

                                                    CREATE TABLE [WhS_RouteSync_Huawei_{0}].[{2}](
                                                          CustomerId varchar(255) NOT NULL,
	                                                      Code varchar(20) NOT NULL,
	                                                      RSName varchar(max) NOT NULL,
                                                          DNSet int NOT NULL
                                                    CONSTRAINT [PK_WhS_RouteSync_Huawei_{0}.{2}{1}] PRIMARY KEY CLUSTERED 
                                                    (
                                                        CustomerId ASC,
	                                                    Code ASC
                                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
													) ON [PRIMARY]";

        const string query_CreateRouteTable = @"IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{2}') AND s.type in (N'U'))
                                                    BEGIN
                                                        CREATE TABLE [WhS_RouteSync_Huawei_{0}].[{2}](
                                                              CustomerId varchar(255) NOT NULL,
	                                                          Code varchar(20) NOT NULL,
	                                                          RSName varchar(max) NOT NULL,
                                                              DNSet int NOT NULL
                                                        CONSTRAINT [PK_WhS_RouteSync_Huawei_{0}.{2}{1}] PRIMARY KEY CLUSTERED 
                                                        (
                                                            CustomerId ASC,
	                                                        Code ASC
                                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                        ) ON [PRIMARY]
													END";

        const string query_CreateSucceedRouteTable = @"CREATE TABLE [WhS_RouteSync_Huawei_{0}].[{2}](
                                                          CustomerId varchar(255) NOT NULL,
	                                                      Code varchar(20) NOT NULL,
	                                                      RSName varchar(max) NOT NULL,
                                                          DNSet int NOT NULL
                                                       CONSTRAINT [PK_WhS_RouteSync_Huawei_{0}.{2}{1}] PRIMARY KEY CLUSTERED 
                                                       (
                                                           CustomerId ASC,
	                                                       Code ASC
                                                       )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                       ) ON [PRIMARY]";

        const string query_SyncWithRouteDeletedTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{2}') AND s.type in (N'U')) 
															BEGIN
																DELETE WhS_RouteSync_Huawei_{0}.{1} 
																FROM WhS_RouteSync_Huawei_{0}.{1} as routes 
                                                                JOIN WhS_RouteSync_Huawei_{0}.{2} as deletedRoutes  
																ON routes.CustomerId = deletedRoutes.CustomerId and routes.Code = deletedRoutes.Code 

																DROP TABLE WhS_RouteSync_Huawei_{0}.{2} 
															END";

        const string query_SyncWithRouteUpdatedTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{2}') AND s.type in (N'U'))
                                                            BEGIN
														        MERGE INTO WhS_RouteSync_Huawei_{0}.{1} as routes 
														        USING WhS_RouteSync_Huawei_{0}.{2} as updatedRoutes
														        ON routes.CustomerId = updatedRoutes.CustomerId and routes.Code = updatedRoutes.Code
														        WHEN MATCHED THEN UPDATE SET routes.RSName = updatedRoutes.RSName, routes.DNSet = updatedRoutes.DNSet;

														        DROP TABLE WhS_RouteSync_Huawei_{0}.{2}
                                                            END";

        const string query_SyncWithRouteAddedTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Huawei_{0}.{2}') AND s.type in (N'U'))
														BEGIN
														    BEGIN TRANSACTION
															BEGIN TRY
															    INSERT INTO  WhS_RouteSync_Huawei_{0}.{1} (CustomerId, Code, RSName, DNSet)
															    SELECT CustomerId, Code, RSName, DNSet
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

        #endregion
    }
}