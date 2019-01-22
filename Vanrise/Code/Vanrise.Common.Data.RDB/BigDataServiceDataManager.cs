using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class BigDataServiceDataManager : IBigDataServiceDataManager
	{

		#region Local Variables
		static string TABLE_NAME = "common_BigDataService";
		static string TABLE_ALIAS = "vrBigDataService";
		const string COL_ID = "ID";
		const string COL_ServiceURL = "ServiceURL";
		const string COL_RuntimeProcessID = "RuntimeProcessID";
		const string COL_TotalCachedRecordsCount = "TotalCachedRecordsCount";
		const string COL_CachedObjectIds = "CachedObjectIds";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static BigDataServiceDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
			columns.Add(COL_ServiceURL, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 1000 });
			columns.Add(COL_RuntimeProcessID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_TotalCachedRecordsCount, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
			columns.Add(COL_CachedObjectIds, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "BigDataService",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime
			});
		}
		#endregion

		#region Public Methods
		public bool AreBigDataServicesUpdated(ref object lastReceivedDataInfo)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
		}

		public void DeleteTimedOutServices(IEnumerable<int> notInRunningProcessIds)
		{
			var deleteQueryContext = new RDBQueryContext(GetDataProvider());
			var deleteQuery = deleteQueryContext.AddDeleteQuery();
			deleteQuery.FromTable(TABLE_NAME);
			var whereContext = deleteQuery.Where();
			whereContext.ListCondition(COL_RuntimeProcessID, RDBListConditionOperator.NotIN, notInRunningProcessIds);
			var effectedRows = deleteQueryContext.ExecuteNonQuery();
			if (effectedRows > 0)
			{
				var selectQueryContext = new RDBQueryContext(GetDataProvider());
				var selectQuery = selectQueryContext.AddSelectQuery();
				selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1);
				selectQuery.SelectColumns().Column(COL_ID);
				var firstId = selectQueryContext.ExecuteScalar().NullableLongValue;
				if (firstId.HasValue)
				{
					var updateQueryContext = new RDBQueryContext(GetDataProvider());
					var updateQuery = updateQueryContext.AddUpdateQuery();
					updateQuery.FromTable(TABLE_NAME);
					updateQuery.Column(COL_ServiceURL).Value(COL_ServiceURL);
					updateQuery.Where().EqualsCondition(COL_ID).Value(firstId.Value);
					updateQueryContext.ExecuteNonQuery();
				}
			}
		}

		public IEnumerable<BigDataService> GetAll()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(BigDataServiceMapper);
		}

		public bool Insert(string serviceUrl, int runtimeProcessId, out long id)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			insertQuery.Column(COL_ServiceURL).Value(serviceUrl);
			insertQuery.Column(COL_RuntimeProcessID).Value(runtimeProcessId);
			insertQuery.AddSelectGeneratedId();
			var insertedId = queryContext.ExecuteScalar().NullableLongValue;
			if (insertedId.HasValue)
			{
				id = insertedId.Value;
				return true;
			}
			id = -1;
			return true;
		}

		public void Update(long bigDataServiceId, long totalRecordsCount, IEnumerable<Guid> cachedObjectIds)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_TotalCachedRecordsCount).Value(totalRecordsCount);
			if (cachedObjectIds != null)
				updateQuery.Column(COL_CachedObjectIds).Value(String.Join(",", cachedObjectIds));
			else
				updateQuery.Column(COL_CachedObjectIds).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(bigDataServiceId);
			queryContext.ExecuteNonQuery();
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "BigDataServiceDBConnStringKey", "BigDataServiceDBConnString");
		}
		#endregion

		#region Mappers
		private BigDataService BigDataServiceMapper(IRDBDataReader reader)
		{
			BigDataService instance = new BigDataService
			{
				BigDataServiceId = reader.GetLong(COL_ID),
				URL = reader.GetString(COL_ServiceURL),
				TotalCachedRecordsCount = reader.GetLongWithNullHandling(COL_TotalCachedRecordsCount),
				CachedObjectIds = new HashSet<Guid>()
			};
			string serializedCachedObjectIds = reader.GetString(COL_CachedObjectIds);
			if (serializedCachedObjectIds != null)
			{
				foreach (var cachedObjectIdString in serializedCachedObjectIds.Split(','))
				{
					if (!String.IsNullOrEmpty(cachedObjectIdString))
						instance.CachedObjectIds.Add(Guid.Parse(cachedObjectIdString));
				}
			}
			return instance;
		}
		#endregion
	}
}
