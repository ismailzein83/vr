using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using TOne.WhS.RouteSync.Ericsson.Data;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
	class CustomerMappingSucceededDataManager : BaseSQLDataManager, ICustomerMappingSucceededDataManager
	{
		const string CustomerMappingSucceededTableName = "CustomerMapping_Succeeded";
		readonly string[] columns = { "BO", "CustomerMapping" };
		public string SwitchId { get; set; }

		public CustomerMappingSucceededDataManager()
			: base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
		{ }


		public void Finalize(ICustomerMappingSucceededFinalizeContext context)
		{
			string query = string.Format(query_DropCustomerMappingSucceededTable, SwitchId, CustomerMappingSucceededTableName);
			ExecuteNonQueryText(query, null);
		}

		public object FinishDBApplyStream(object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.Close();
			return new StreamBulkInsertInfo
			{
				TableName = string.Format("[WhS_RouteSync_Ericsson_{0}].[{1}]", SwitchId, CustomerMappingSucceededTableName),
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

		public void WriteRecordToStream(CustomerMappingSerialized record, object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.WriteRecord("{0}^{1}", record.BO, record.CustomerMappingAsString);
		}

		public void ApplyCustomerMappingSucceededForDB(object preparedCustomerMapping)
		{
			InsertBulkToTable(preparedCustomerMapping as BaseBulkInsertInfo);
		}

		CustomerMappingSerialized CustomerMappingSerializedMapper(IDataReader reader)
		{
			return new CustomerMappingSerialized()
			{
				BO = reader["BO"] as string,
				CustomerMappingAsString = reader["CustomerMapping"] as string
			};
		}

		#region Queries
		const string query_DropCustomerMappingSucceededTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}') AND s.type in (N'U'))
                                                    BEGIN
                                                        DROP TABLE WhS_RouteSync_Ericsson_{0}.{1}
                                                    END";
		#endregion
	}
}
