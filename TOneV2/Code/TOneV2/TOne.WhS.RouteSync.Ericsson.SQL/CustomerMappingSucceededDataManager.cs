using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
	public class CustomerMappingSucceededDataManager : BaseSQLDataManager, ICustomerMappingSucceededDataManager
	{
		const string CustomerMappingSucceededTableName = "CustomerMapping_Succeeded";
		readonly string[] columns = { "BO", "CustomerMapping" };
		public string SwitchId { get; set; }

		public CustomerMappingSucceededDataManager()
			: base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
		{ }

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

		public void WriteRecordToStream(CustomerMappingWithActionType record, object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.WriteRecord("{0}^{1}^{2}", record.CustomerMappingSerialized.BO, record.CustomerMappingSerialized.CustomerMappingAsString, record.ActionType);
		}

		public void ApplyCustomerMappingSucceededForDB(object preparedCustomerMapping)
		{
			InsertBulkToTable(preparedCustomerMapping as BaseBulkInsertInfo);
		}

		public void SaveCustomerMappingsSucceededToDB(IEnumerable<CustomerMappingWithActionType> customerMappingsSucceeded)
		{
			Object dbApplyStream = InitialiazeStreamForDBApply();
			foreach (var customerMappingSucceeded in customerMappingsSucceeded)
				WriteRecordToStream(customerMappingSucceeded, dbApplyStream);
			Object preparedInvalidCDRs = FinishDBApplyStream(dbApplyStream);
			ApplyCustomerMappingSucceededForDB(preparedInvalidCDRs);
		}
	}
}
