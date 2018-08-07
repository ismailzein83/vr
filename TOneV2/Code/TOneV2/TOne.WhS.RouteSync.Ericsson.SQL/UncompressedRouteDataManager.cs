using System;
using Vanrise.Data.SQL;
using TOne.WhS.RouteSync.Ericsson.Data;
using System.Collections.Generic;
using System.Data;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
	public class UncompressedRouteDataManager : BaseSQLDataManager, IUncompressedRouteDataManager
	{
		public UncompressedRouteDataManager()
		: base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
		{


		}
		const string UncompressedRouteTableName = "Route_temp_uncompressed";

		readonly string[] columns = { "BO", "Code", "RCNumber" };

		public string SwitchId { get; set; }

		public object FinishDBApplyStream(object dbApplyStream)
		{
			StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
			streamForBulkInsert.Close();
			return new StreamBulkInsertInfo
			{
				TableName = string.Format("[WhS_RouteSync_Ericsson_{0}].[{1}]", SwitchId, UncompressedRouteTableName),
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

		public IEnumerable<EricssonConvertedRoute> GetUncompressedRoutes()
		{
			List<EricssonConvertedRoute> routes = new List<EricssonConvertedRoute>();
			string query = string.Format(query_SelectUncompressedRoutes, SwitchId, UncompressedRouteTableName);

			ExecuteReaderText(query, (reader) =>
			{
				while (reader.Read())
				{
					routes.Add(EricssonConvertedRouteMapper(reader));
				}
			}, null);

			return routes;
		}

		private EricssonConvertedRoute EricssonConvertedRouteMapper(IDataReader reader)
		{
			return new EricssonConvertedRoute()
			{
				BO = reader["BO"] as string,
				Code = reader["Code"] as string,
				RCNumber = (int)reader["RCNumber"]
			};
		}

		const string query_SelectUncompressedRoutes = @"SELECT[BO],[Code],[RCNumber] FROM[WhS_RouteSync_Ericsson_{0}].[{1}]";
	}
}
