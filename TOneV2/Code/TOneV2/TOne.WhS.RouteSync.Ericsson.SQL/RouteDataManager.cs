using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Data.SQL;
using TOne.WhS.RouteSync.Ericsson.Data;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
	public class RouteDataManager : BaseSQLDataManager, IRouteDataManager
	{
		const string RouteTableName = "Route";
		const string RouteTempTableName = "Route_temp";
		const string RouteAddedTableName = "Route_Added";
		const string RouteUpdatedTableName = "Route_Updated";
		const string RouteDeletedTableName = "Route_Deleted";
		readonly string[] columns = { "BO", "Code", "RCNumber" };

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
			var differencesByBO = new Dictionary<string, EricssonConvertedRouteDifferences>();
			var differences = new Dictionary<EricssonConvertedRouteIdentifier, List<EricssonConvertedRouteByCompare>>();

			string query = string.Format(query_CompareRouteTables, SwitchId, RouteTableName, RouteTempTableName);
			ExecuteReaderText(query, (reader) =>
			{
				while (reader.Read())
				{
					var convertedRouteByCompare = new EricssonConvertedRouteByCompare() { EricssonConvertedRoute = EricssonConvertedRouteMapper(reader), TableName = reader["tableName"] as string };
					var routeIdentifier = new EricssonConvertedRouteIdentifier() { BO = convertedRouteByCompare.EricssonConvertedRoute.BO, Code = convertedRouteByCompare.EricssonConvertedRoute.Code };
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
								},
								OriginalValue = (routeOldValue != null) ? new EricssonConvertedRoute()
								{
									BO = routeOldValue.EricssonConvertedRoute.BO,
									Code = routeOldValue.EricssonConvertedRoute.Code,
									RCNumber = routeOldValue.EricssonConvertedRoute.RCNumber,
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
			streamForBulkInsert.WriteRecord("{0}^{1}^{2}", record.BO, record.Code, record.RCNumber);
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
			string query = string.Format(query_UpdateTempTable, SwitchId, RouteTempTableName);
			ExecuteNonQueryText(query, (cmd) =>
			{
				var dtPrm = new SqlParameter("@Routes", SqlDbType.Structured);
				dtPrm.TypeName = "EricssonConvertedRouteType";
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
				dtPrm.TypeName = "EricssonConvertedRouteType";
				dtPrm.Value = dtRoutes;
				cmd.Parameters.Add(dtPrm);
			});
		}

		public void CopyCustomerRoutesToTempTable(IEnumerable<string> customerBOs)
		{
			if (customerBOs == null || !customerBOs.Any())
				return;
			string filter = string.Format(" Where BO in ({0})", string.Join(",", customerBOs));
			string query = string.Format(query_CopyFromBaseTableToTempTable.Replace("#FILTER#", filter), SwitchId, RouteTableName, RouteTempTableName);
			ExecuteNonQueryText(query, null);
		}

		private DataTable BuildRouteTable(IEnumerable<EricssonConvertedRoute> routes)
		{
			DataTable dtRoutes = new DataTable();
			dtRoutes.Columns.Add("BO", typeof(string));
			dtRoutes.Columns.Add("Code", typeof(string));
			dtRoutes.Columns.Add("RCNumber", typeof(int));
			dtRoutes.BeginLoadData();
			foreach (var route in routes)
			{
				DataRow dr = dtRoutes.NewRow();
				dr["BO"] = route.BO;
				dr["Code"] = route.Code;
				dr["RCNumber"] = route.RCNumber;
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
				BO = reader["BO"] as string,
				Code = reader["Code"] as string,
				RCNumber = (int)reader["RCNumber"]
			};
		}

		#region Queries
		const string query_CreateRouteTempTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                                    BEGIN
                                                        DROP TABLE WhS_RouteSync_Ericsson_{0}.{2}
                                                    END

                                                    CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[{2}](
                                                          BO varchar(255) NOT NULL,
	                                                      Code varchar(20) NOT NULL,
	                                                      RCNumber int NOT NULL
                                                    CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.Route_{2}{1}] PRIMARY KEY CLUSTERED 
                                                    (
                                                        BO ASC,
	                                                    Code ASC
                                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
													) ON [PRIMARY]";

		const string query_CompareRouteTables = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}') AND s.type in (N'U'))
                                                  BEGIN
	                                                  SELECT [BO],[Code],[RCNumber], max(tableName) as tableName FROM (
		                                                  SELECT [BO],[Code],[RCNumber], '{1}' as tableName FROM [WhS_RouteSync_Ericsson_{0}].[{1}]
		                                                  UNION ALL
		                                                  SELECT [BO],[Code],[RCNumber], '{2}' as tableName FROM [WhS_RouteSync_Ericsson_{0}].[{2}]
	                                                  ) v
	                                                  GROUP BY [BO],[Code],[RCNumber]
	                                                  HAVING COUNT(1)=1
                                                  END
                                                  ELSE
                                                  BEGIN
	                                                  SELECT [BO],[Code],[RCNumber], '{2}' as tableName FROM [WhS_RouteSync_Ericsson_{0}].[{2}]
                                                  END";

		const string query_SwapTables = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                                    BEGIN
															IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}_old') AND s.type in (N'U'))
																	begin
																		DROP TABLE WhS_RouteSync_Ericsson_{0}.{1}_old
																	end
														EXEC sp_rename 'WhS_RouteSync_Ericsson_{0}.{1}', '{1}_old';
                                                    END

	                                        EXEC sp_rename 'WhS_RouteSync_Ericsson_{0}.{2}', '{1}';";

		const string query_SyncWithRouteDeletedTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U')) 
															BEGIN
																DELETE WhS_RouteSync_Ericsson_{0}.{1} 
																FROM WhS_RouteSync_Ericsson_{0}.{1} as routes join WhS_RouteSync_Ericsson_{0}.{2} as deletedRoutes  
																ON routes.BO = deletedRoutes.BO and routes.Code = deletedRoutes.Code 

																DROP TABLE WhS_RouteSync_Ericsson_{0}.{2} 
															END";

		const string query_SyncWithRouteUpdatedTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                                    BEGIN
														MERGE INTO WhS_RouteSync_Ericsson_{0}.{1}  as routes 
														USING WhS_RouteSync_Ericsson_{0}.{2} as updatedRoutes
														ON routes.BO = updatedRoutes.BO and routes.Code = updatedRoutes.Code
														WHEN MATCHED THEN
														UPDATE 
														SET routes.RCNumber = updatedRoutes.RCNumber;

														DROP TABLE WhS_RouteSync_Ericsson_{0}.{2}
                                                    END";

		const string query_SyncWithRouteAddedTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
														BEGIN
														BEGIN TRANSACTION
															BEGIN TRY
																INSERT INTO  WhS_RouteSync_Ericsson_{0}.{1} (BO, Code, RCNumber)
																SELECT BO, Code, RCNumber  FROM WhS_RouteSync_Ericsson_{0}.{2}
														
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
                                                    CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[{2}](
                                                          BO varchar(255) NOT NULL,
	                                                      Code varchar(20) NOT NULL,
	                                                      RCNumber int NOT NULL
                                                    CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.Route_{2}{1}] PRIMARY KEY CLUSTERED 
                                                    (
                                                        BO ASC,
	                                                    Code ASC
                                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                    ) ON [PRIMARY]
													END";

		const string query_CreateSucceedRouteTable = @"CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[{2}](
                                            BO varchar(255) NOT NULL,
	                                        Code varchar(20) NOT NULL,
	                                        RCNumber int NOT NULL
                                            CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.{2}{1}] PRIMARY KEY CLUSTERED 
                                                (
                                                  BO ASC,
	                                              Code ASC
                                                 )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                            ) ON [PRIMARY]";


		const string query_DeleteRouteTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}') AND s.type in (N'U'))
													BEGIN 
														DROP TABLE WhS_RouteSync_Ericsson_{0}.{1} 
													END";

		const string query_CopyFromBaseTableToTempTable = @"INSERT INTO  WhS_RouteSync_Ericsson_{0}.{2} (BO, Code, RCNumber)
														SELECT BO, Code, RCNumber FROM WhS_RouteSync_Ericsson_{0}.{1}
														#FILTER#";

		const string query_EricssonRouteTableType = @"IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'LongIDType')
                                         CREATE TYPE [EricssonRouteTableType] AS TABLE(
	                                     [BO] [varchar](255) NOT NULL,
	                                     [Code] [varchar](20) NOT NULL,
	                                     [RCNumber] [int] NOT NULL,
	                                     PRIMARY KEY CLUSTERED 
                                         (
											[BO] ASC,
	                                        [Code] ASC
                                         )WITH (IGNORE_DUP_KEY = OFF)
                                         )";

		const string query_DeleteFromTempTableTable = @"DELETE WhS_RouteSync_Ericsson_{0}.{1}
																FROM WhS_RouteSync_Ericsson_{0}.{1} as tempRoute join @Routes as route
																ON route.BO = tempRoute.BO
																and route.Code = tempRoute.Code
																and route.RCNumber = tempRoute.RCNumber";

		const string query_UpdateTempTable = @"UPDATE  [WhS_RouteSync_Ericsson_{0}].[{1}] 
														set tempRoutes.RCNumber = routesToUpdate.RCNumber
                                                    FROM [WhS_RouteSync_Ericsson_{0}].[{1}] as tempRoutes
                                                    JOIN @Routes as routesToUpdate on routesToUpdate.BO = tempRoutes.BO and routesToUpdate.Code = tempRoutes.Code";
		#endregion
	}
}