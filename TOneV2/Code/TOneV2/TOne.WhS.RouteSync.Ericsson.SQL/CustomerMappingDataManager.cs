﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Data;
using Vanrise.Data.SQL;
using Vanrise.Common;

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

		public void Swap(ICustomerMappingFinalizeContext context)
		{
			string query = string.Format(query_SwapTables, SwitchId, CustomerMappingTableName, CustomerMappingTempTableName);
			ExecuteNonQueryText(query, null);
		}

		public void Finalize(ICustomerMappingFinalizeContext context)
		{
			string syncWithSucceededTableQuery = string.Format(query_SyncWithCustomerMappingSucceededTable, SwitchId, CustomerMappingTableName, CustomerMappingSucceededTableName, Guid.NewGuid());
			ExecuteNonQueryText(syncWithSucceededTableQuery, null);

			string query = string.Format(query_DeleteCustomerMappingTable, SwitchId, CustomerMappingTempTableName);
			ExecuteNonQueryText(query, null);
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
						var customerMappingToUpdate = customerMappingDifferences.FindRecord(item => (string.Compare(item.TableName, CustomerMappingTempTableName, true) == 0));
						if (customerMappingToUpdate != null)
							customerMappingsToUpdate.Add(customerMappingToUpdate.CustomerMapping);
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
                                                              DROP TABLE WhS_RouteSync_Ericsson_{0}.{1}
                                                          end

	                                        EXEC sp_rename '[WhS_RouteSync_Ericsson_{0}].[{2}]', '{1}';";

		const string query_SyncWithCustomerMappingSucceededTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                                    BEGIN
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
		#endregion
	}
}