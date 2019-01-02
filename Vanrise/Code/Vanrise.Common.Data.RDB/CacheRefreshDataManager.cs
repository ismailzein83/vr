using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class CacheRefreshDataManager : ICacheRefreshDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_CacheRefreshHandle";
		static string TABLE_ALIAS = "vrCacheRefreshHandle";
		const string COL_CacheTypeName = "CacheTypeName";
		const string COL_LastUpdateTime = "LastUpdateTime";
		const string COL_CreatedTime = "CreatedTime";
		#endregion

		#region Constructors
		static CacheRefreshDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_CacheTypeName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_LastUpdateTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "CacheRefreshHandle",
				Columns = columns,
				IdColumnName = COL_CacheTypeName,
				CreatedTimeColumnName = COL_CreatedTime
			});
		}
		#endregion

		#region Public Methods
		public bool AreUpdateHandlesEqual(ref object lastReceivedDataInfo, object newReceivedDataInfo)
		{
			if (newReceivedDataInfo == null)
			{
				return false;
			}
			else
			{
				if (lastReceivedDataInfo == null || newReceivedDataInfo != lastReceivedDataInfo)
				{
					lastReceivedDataInfo = newReceivedDataInfo;
					return true;
				}
				else
					return false;
			}
		}

		public List<CacheRefreshHandle> GetAll()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQyery = queryContext.AddSelectQuery();
			selectQyery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQyery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(CacheRefreshHandleMapper);
		}

		public CacheRefreshHandle GetByCacheTypeName(string cacheTypeName)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQyery = queryContext.AddSelectQuery();
			selectQyery.From(TABLE_NAME, TABLE_ALIAS);
			selectQyery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQyery.Where().EqualsCondition(COL_CacheTypeName).Value(cacheTypeName);
			return queryContext.GetItem(CacheRefreshHandleMapper);
		}

		public void UpdateCacheTypeHandle(string cacheTypeName)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_LastUpdateTime).DateNow();
			updateQuery.Where().EqualsCondition(COL_CacheTypeName).Value(cacheTypeName);
			var result = queryContext.ExecuteNonQuery();

			if (result == 0)
			{
				queryContext = new RDBQueryContext(GetDataProvider());
				var insertQuery = queryContext.AddInsertQuery();
				insertQuery.IntoTable(TABLE_NAME);
				var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
				ifNotExist.EqualsCondition(COL_CacheTypeName).Value(cacheTypeName);
				insertQuery.Column(COL_CacheTypeName).Value(cacheTypeName);
				queryContext.ExecuteNonQuery();
			}

		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		private CacheRefreshHandle CacheRefreshHandleMapper(IRDBDataReader reader)
		{
			return new CacheRefreshHandle
			{
				TypeName = reader.GetString(COL_CacheTypeName),
				LastUpdateTime = reader.GetNullableDateTime(COL_LastUpdateTime),
				LastUpdateInfo = reader.GetNullableDateTime(COL_LastUpdateTime)
			};
		}
		#endregion

	}
}
