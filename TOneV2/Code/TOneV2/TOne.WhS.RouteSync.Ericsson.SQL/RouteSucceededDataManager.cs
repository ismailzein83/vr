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
		const string RouteAddedTableName = "Route_Added";
		const string RouteUpdatedTableName = "Route_Updated";
		const string RouteDeletedTableName = "Route_Deleted";

		readonly string[] columns = { "BO", "Code", "RCNumber" };
		public string SwitchId { get; set; }
		public string RouteSucceededTableName { get; set; }
		public RouteSucceededDataManager()
			: base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
		{ }


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

		public void ApplyRoutesSucceededForDB(object preparedRoute)
		{
			InsertBulkToTable(preparedRoute as BaseBulkInsertInfo);
		}

		public void SaveRoutesSucceededToDB(IEnumerable<EricssonConvertedRoute> routes, RouteActionType actionType)
		{
			switch (actionType)
			{
				case RouteActionType.Add:
					RouteSucceededTableName = RouteAddedTableName;
					break;
				case RouteActionType.Update:
					RouteSucceededTableName = RouteUpdatedTableName;
					break;
				case RouteActionType.Delete:
					RouteSucceededTableName = RouteDeletedTableName;
					break;
				default: break;
			}
			Object dbApplyStream = InitialiazeStreamForDBApply();
			foreach (var route in routes)
				WriteRecordToStream(route, dbApplyStream);
			Object preparedInvalidCDRs = FinishDBApplyStream(dbApplyStream);
			ApplyRoutesSucceededForDB(preparedInvalidCDRs);
		}
	}
}
