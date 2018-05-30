using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Data.SQL;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
	public class CustomerMappingDataManager : BaseSQLDataManager, ICustomerMappingDataManager
	{
		const string CustomerMappingTableName = "CustomerMapping";
		const string CustomerMappingTempTableName = "CustomerMapping_temp";
		const string CustomerMappingSucceededTableName = "CustomerMapping_Succeeded";
		readonly string[] columns = { "BO", "CustomerMapping" };

		public string SwitchId { get; set; }

		public CustomerMappingDataManager()
			: base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
		{

		}

		public void Initialize(ICustomerMappingInitializeContext context)
		{
			Guid guid = Guid.NewGuid();
			string createTempTableQuery = string.Format(query_CreateCustomerMappingTempTable, SwitchId, CustomerMappingTempTableName, guid);
			ExecuteNonQueryText(createTempTableQuery, null);

			string createTableQuery = string.Format(query_CreateCustomerMappingTable, SwitchId, CustomerMappingTableName, guid);
			ExecuteNonQueryText(createTableQuery, null);

			string syncWithSucceededTableQuery = string.Format(query_SyncWithCustomerMappingSucceededTable, SwitchId, CustomerMappingTableName, CustomerMappingSucceededTableName);
			ExecuteNonQueryText(syncWithSucceededTableQuery, null);

			string createSucceedTableQuery = string.Format(query_CreateSucceedCustomerMappingTable, SwitchId, CustomerMappingSucceededTableName, guid);
			ExecuteNonQueryText(createSucceedTableQuery, null);
		}

		public void Finalize(ICustomerMappingFinalizeContext context)
		{
			string query = string.Format(query_SwapTables, SwitchId, CustomerMappingTableName, CustomerMappingTempTableName);
			ExecuteNonQueryText(query, null);
			string deleteSucceededTableQuery = string.Format(query_DeleteCustomerMappingTable, SwitchId, CustomerMappingSucceededTableName);
			ExecuteNonQueryText(deleteSucceededTableQuery, null);
		}

		public void CompareTables(ICustomerMappingTablesContext context)
		{
			var differences = new Dictionary<string, List<CustomerMappingByCompare>>();

			string query = string.Format(query_CompareCustomerMappingTables, SwitchId, CustomerMappingTableName, CustomerMappingTempTableName);
			ExecuteReaderText(query, (reader) =>
			{
				while (reader.Read())
				{
					var customerMappingByCompare = new CustomerMappingByCompare() { CustomerMapping = CustomerMappingMapper(reader), TableName = reader["tableName"] as string };
					List<CustomerMappingByCompare> tempCustomerMappingDifferences = differences.GetOrCreateItem(customerMappingByCompare.CustomerMapping.BO);
					tempCustomerMappingDifferences.Add(customerMappingByCompare);
				}
			}, null);

			if (differences.Count > 0)
			{
				List<CustomerMappingSerialized> customerMappingsToAdd = new List<CustomerMappingSerialized>();
				List<CustomerMappingSerialized> customerMappingsToUpdate = new List<CustomerMappingSerialized>();
				List<CustomerMappingSerialized> customerMappingsToDelete = new List<CustomerMappingSerialized>();
				foreach (var differenceKvp in differences)
				{
					var customerMappingDifferences = differenceKvp.Value;
					if (customerMappingDifferences.Count == 1)
					{
						var singleCustomerMappingDifference = differenceKvp.Value[0];
						if (singleCustomerMappingDifference.TableName == CustomerMappingTableName)
							customerMappingsToDelete.Add(singleCustomerMappingDifference.CustomerMapping);
						else
							customerMappingsToAdd.Add(singleCustomerMappingDifference.CustomerMapping);
					}
					else
					{
						var customerMapping = customerMappingDifferences.FindRecord(item => (string.Compare(item.TableName, CustomerMappingTempTableName, true) == 0));
						var customerMappingOldValue = customerMappingDifferences.FindRecord(item => (string.Compare(item.TableName, CustomerMappingTableName, true) == 0));
						var customerMappingToUpdate = new CustomerMappingSerialized();
						if (customerMapping != null)
						{
							customerMappingToUpdate.BO = customerMapping.CustomerMapping.BO;
							customerMappingToUpdate.CustomerMappingAsString = customerMapping.CustomerMapping.CustomerMappingAsString;
							if (customerMappingOldValue != null)
								customerMappingToUpdate.CustomerMappingOldValueAsString = customerMappingOldValue.CustomerMapping.CustomerMappingAsString;

							customerMappingsToUpdate.Add(customerMappingToUpdate);
						}
					}
				}

				if (customerMappingsToAdd.Count > 0)
					context.CustomerMappingsToAdd = customerMappingsToAdd;

				if (customerMappingsToUpdate.Count > 0)
					context.CustomerMappingsToUpdate = customerMappingsToUpdate;

				if (customerMappingsToDelete.Count > 0)
					context.CustomerMappingsToDelete = customerMappingsToDelete;
			}
		}

		#region handleFailedRecords
		public void RemoveCutomerMappingsFromTempTable(IEnumerable<string> failedRecordsBO)
		{
			if (failedRecordsBO == null || !failedRecordsBO.Any())
				return;
			string filter = string.Format(" Where BO in ({0})", string.Join(",", failedRecordsBO));
			string query = string.Format(query_RemoveFailedRecords.Replace("#FILTER#", filter), SwitchId, CustomerMappingTableName, CustomerMappingTempTableName);
			ExecuteNonQueryText(query, null);
		}

		public void UpdateCustomerMappingsInTempTable(IEnumerable<CustomerMapping> customerMappingsToUpdate)
		{
			DataTable dtCustomerMappings = BuildCustomerMappingsTable(customerMappingsToUpdate);
			ExecuteNonQueryText(query_UpdateRecordsWithFailedTrunk, (cmd) =>
			{
				var dtPrm = new SqlParameter("@UpdatedCustomerMappings", SqlDbType.Structured);
				dtPrm.TypeName = "CustomerRouteType";
				dtPrm.Value = dtCustomerMappings;
				cmd.Parameters.Add(dtPrm);
			});
		}

		#endregion

		#region BCP
		public object InitialiazeStreamForDBApply()
		{
			return base.InitializeStreamForBulkInsert();
		}

		public void WriteRecordToStream(CustomerMappingSerialized customerMapping, object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.WriteRecord("{0}^{1}", customerMapping.BO, customerMapping.CustomerMappingAsString);
		}

		public void ApplyCustomerMappingForDB(object preparedCustomerMapping)
		{
			InsertBulkToTable(preparedCustomerMapping as BaseBulkInsertInfo);
		}

		public object FinishDBApplyStream(object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.Close();
			return new StreamBulkInsertInfo
			{
				TableName = string.Format("[WhS_RouteSync_Ericsson_{0}].[{1}]", SwitchId, CustomerMappingTempTableName),
				Stream = streamForBulkInsert,
				TabLock = true,
				KeepIdentity = false,
				FieldSeparator = '^',
				ColumnNames = columns
			};
		}

		public void InsertCutomerMappingsToTempTable(IEnumerable<CustomerMapping> customerMappings)
		{
			if (customerMappings != null && customerMappings.Any())
			{
				object dbApplyStream = InitialiazeStreamForDBApply();
				foreach (var customerMapping in customerMappings)
				{
					CustomerMappingSerialized customerMappingSerialized = new CustomerMappingSerialized() { BO = customerMapping.BO, CustomerMappingAsString = Helper.SerializeCustomerMapping(customerMapping) };
					WriteRecordToStream(customerMappingSerialized, dbApplyStream);
				}
				object obj = FinishDBApplyStream(dbApplyStream);
				ApplyCustomerMappingForDB(obj);
			}
		}
		#endregion

		#region Mappers
		CustomerMappingSerialized CustomerMappingMapper(IDataReader reader)
		{
			return new CustomerMappingSerialized()
			{
				BO = reader["BO"] as string,
				CustomerMappingAsString = reader["CustomerMapping"] as string
			};
		}
		#endregion

		private DataTable BuildCustomerMappingsUpdateTable(IEnumerable<CustomerMappingWithActionType> customerMappingsToUpdate)
		{
			DataTable dtCustomerMappings = new DataTable();
			dtCustomerMappings.Columns.Add("BO", typeof(string));
			dtCustomerMappings.Columns.Add("CustomerMapping", typeof(string));
			dtCustomerMappings.BeginLoadData();
			foreach (var customerMappingToUpdate in customerMappingsToUpdate)
			{
				DataRow dr = dtCustomerMappings.NewRow();
				dr["BO"] = customerMappingToUpdate.CustomerMapping.BO;
				dr["CustomerMapping"] = Helper.SerializeCustomerMapping(customerMappingToUpdate.CustomerMappingOldValue);
				dtCustomerMappings.Rows.Add(dr);
			}
			dtCustomerMappings.EndLoadData();
			return dtCustomerMappings;
		}

		private DataTable BuildCustomerMappingsTable(IEnumerable<CustomerMapping> customerMappingsToUpdate)
		{
			DataTable dtCustomerMappings = new DataTable();
			dtCustomerMappings.Columns.Add("BO", typeof(string));
			dtCustomerMappings.Columns.Add("CustomerMapping", typeof(string));
			dtCustomerMappings.BeginLoadData();
			foreach (var customerMappingToUpdate in customerMappingsToUpdate)
			{
				DataRow dr = dtCustomerMappings.NewRow();
				dr["BO"] = customerMappingToUpdate.BO;
				dr["CustomerMapping"] = Helper.SerializeCustomerMapping(customerMappingToUpdate);
				dtCustomerMappings.Rows.Add(dr);
			}
			dtCustomerMappings.EndLoadData();
			return dtCustomerMappings;
		}
		#region queries

		const string query_CreateCustomerMappingTempTable = @"IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}') AND s.type in (N'U'))
                                                          begin
                                                              DROP TABLE WhS_RouteSync_Ericsson_{0}.{1}
                                                          end
                                                          
                                                          CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[{1}](
                                                                BO varchar(255) NOT NULL,
	                                                            CustomerMapping varchar(max) NOT NULL,

                                                          CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.CustomerMapping_{1}{2}] PRIMARY KEY CLUSTERED 
                                                         (
                                                             BO ASC
                                                         )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                         ) ON [PRIMARY]";

		const string query_CreateCustomerMappingTable = @"IF  NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}') AND s.type in (N'U'))
                                                          BEGIN
                                                          CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[{1}](
                                                                BO varchar(255) NOT NULL,
	                                                            CustomerMapping varchar(max) NOT NULL,

                                                          CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.CustomerMapping_{1}{2}] PRIMARY KEY CLUSTERED 
                                                         (
                                                             BO ASC
                                                         )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                         ) ON [PRIMARY]
														 END";

		const string query_CompareCustomerMappingTables = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}') AND s.type in (N'U'))
                                                  BEGIN
	                                                  SELECT [BO], [CustomerMapping], max(tableName) as tableName FROM (
		                                                  SELECT [BO], [CustomerMapping], '{1}' as tableName FROM [WhS_RouteSync_Ericsson_{0}].[{1}]
		                                                  UNION ALL
		                                                  SELECT [BO], [CustomerMapping], '{2}' as tableName FROM [WhS_RouteSync_Ericsson_{0}].[{2}]
	                                                  ) v
	                                                  GROUP BY [BO],[CustomerMapping]
	                                                  HAVING COUNT(1)=1
                                                  END
                                                  ELSE
                                                  BEGIN
	                                                  SELECT [BO], [CustomerMapping], '{2}' as tableName FROM [WhS_RouteSync_Ericsson_{0}].[{2}]
                                                  END";

		const string query_SwapTables = @"IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}') AND s.type in (N'U'))
                                                          begin
																IF  EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}_old') AND s.type in (N'U'))
																	begin
																		DROP TABLE WhS_RouteSync_Ericsson_{0}.{1}_old
																	end
																EXEC sp_rename 'WhS_RouteSync_Ericsson_{0}.{1}', '{1}_old';
														  end

	                                        EXEC sp_rename 'WhS_RouteSync_Ericsson_{0}.{2}', '{1}';";

		const string query_SyncWithCustomerMappingSucceededTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                                    BEGIN
													BEGIN TRANSACTION
														BEGIN TRY
															DELETE WhS_RouteSync_Ericsson_{0}.{1}
															FROM WhS_RouteSync_Ericsson_{0}.{1} as cm join WhS_RouteSync_Ericsson_{0}.{2} as cms 
															ON cm.BO = cms.BO
															WHERE cms.Action = 2

															MERGE INTO WhS_RouteSync_Ericsson_{0}.{1}  as cm 
															USING WhS_RouteSync_Ericsson_{0}.{2} as cms
															ON cm.BO = cms.BO and cms.Action=1
															WHEN MATCHED THEN
															UPDATE 
															SET cm.CustomerMapping = cms.CustomerMapping;

															INSERT INTO  WhS_RouteSync_Ericsson_{0}.{1} (BO, CustomerMapping)
															SELECT BO, CustomerMapping FROM WhS_RouteSync_Ericsson_{0}.{2} as cms
															WHERE cms.Action = 0

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

		const string query_CreateSucceedCustomerMappingTable = @"CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[{1}](
                                                                BO varchar(255) NOT NULL,
	                                                            CustomerMapping varchar(max) NOT NULL,
																Action tinyint NOT NULL,

																CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.CustomerMapping_{1}{2}] PRIMARY KEY CLUSTERED 
																(
																	BO ASC
																)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
																ON [PRIMARY])";

		const string query_DeleteCustomerMappingTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}') AND s.type in (N'U'))
                                                    BEGIN
                                                        DROP TABLE WhS_RouteSync_Ericsson_{0}.{1}
                                                    END";

		const string query_RemoveFailedRecords = @"DELETE FROM [WhS_RouteSync_Ericsson_{0}].[{1}]
														#FILTER#";

		const string query_CopyRecordsFromBaseToTempTable = @"INSERT INTO  WhS_RouteSync_Ericsson_{0}.{2} (BO, CustomerMapping)
														SELECT BO, CustomerMapping FROM WhS_RouteSync_Ericsson_{0}.{1}
														#FILTER#";

		const string query_UpdateTempTableFromBaseTable = @"MERGE INTO WhS_RouteSync_Ericsson_{0}.{2}  as cm 
														USING WhS_RouteSync_Ericsson_{0}.{1} as cms
														ON cm.BO = cms.BO and cms.Action=1
														WHEN MATCHED AND cm.BO IN #Filter# THEN
														UPDATE 
														SET cm.CustomerMapping = cms.CustomerMapping;";

		const string query_UpdateTempTableFromBaseTable1 = @"UPDATE tempTable set tempTable.CustomerMapping = baseTable.CustomerMapping 
														   FROM tempTable JOIN baseTable on tempTable.BO = tempTable.BO 
														   #FILTER#";

		const string query_UpdateRecordsWithFailedTrunk = @"UPDATE  [WhS_RouteSync_Ericsson_{0}].[{1}] set [WhS_RouteSync_Ericsson_{0}].[{1}].CustomerMapping = updatedCustomerMappings.CustomerMapping
                                                    FROM [WhS_RouteSync_Ericsson_{0}].[{1}]
                                                    JOIN @UpdatedCustomerMappings updatedCustomerMappings on updatedCustomerMappings.BO = [WhS_RouteSync_Ericsson_{0}].[{1}].BO";

		const string query_EricssonCustomerMappingTableType = @"IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'LongIDType')
                                         CREATE TYPE [EricssonCustomerMappingTableType] AS TABLE(
	                                     [BO] [varchar](255) NOT NULL,
	                                     [CustomerMapping] [varchar](max) NOT NULL,
	                                     PRIMARY KEY CLUSTERED 
                                         (
                                             [BO] ASC
                                         )WITH (IGNORE_DUP_KEY = OFF)
                                         )";

		const string query_UpdateTempTable = @"UPDATE  tempCustomerMapping
														set tempCustomerMapping.CustomerMapping = customerMapping.CustomerMapping
                                                    FROM [WhS_RouteSync_Ericsson_{0}].[{1}] as tempRoutes
                                                    JOIN @UpdatedCustomerMapping as updatedCustomerMapping on tempCustomerMapping.BO = updatedCustomerMapping.BO
													JOIN [WhS_RouteSync_Ericsson_{0}].[{2}] as customerMapping on customerMapping.BO = updatedCustomerMapping.BO";

		#endregion
	}
}