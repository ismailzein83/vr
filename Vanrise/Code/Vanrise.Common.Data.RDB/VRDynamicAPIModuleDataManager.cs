using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRDynamicAPIModuleDataManager : IVRDynamicAPIModuleDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRDynamicAPIModule";
		static string TABLE_ALIAS = "vrDynamicAPIModule";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_CreatedBy = "CreatedBy";
		const string COL_LastModifiedTime = "LastModifiedTime";
		const string COL_LastModifiedBy = "LastModifiedBy";
		#endregion

		#region Constructors
		static VRDynamicAPIModuleDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRDynamicAPIModule",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public List<VRDynamicAPIModule> GetVRDynamicAPIModules()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
			return queryContext.GetItems(VRDynamicAPIModuleMapper);
		}

		public bool Insert(VRDynamicAPIModule vrDynamicAPIModule, out int insertedId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(vrDynamicAPIModule.Name);
			insertQuery.AddSelectGeneratedId();
			insertQuery.Column(COL_Name).Value(vrDynamicAPIModule.Name);
			insertQuery.Column(COL_CreatedBy).Value(vrDynamicAPIModule.CreatedBy);
			insertQuery.Column(COL_LastModifiedBy).Value(vrDynamicAPIModule.LastModifiedBy);

			int? id = queryContext.ExecuteScalar().NullableIntValue;
			if (id.HasValue)
			{
				insertedId = id.Value;
				return true;
			}
			insertedId = 0;
			return false;
		}

		public bool Update(VRDynamicAPIModule vrDynamicAPIModule)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(vrDynamicAPIModule.VRDynamicAPIModuleId);
			ifNotExist.EqualsCondition(COL_Name).Value(vrDynamicAPIModule.Name);
			updateQuery.Column(COL_Name).Value(vrDynamicAPIModule.Name);
			updateQuery.Column(COL_LastModifiedBy).Value(vrDynamicAPIModule.LastModifiedBy);
			updateQuery.Where().EqualsCondition(COL_ID).Value(vrDynamicAPIModule.VRDynamicAPIModuleId);
			return queryContext.ExecuteNonQuery() > 0;
		}
		public bool AreVRDynamicAPIModulesUpdated(ref object updateHandle)
		{
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		VRDynamicAPIModule VRDynamicAPIModuleMapper(IRDBDataReader reader)
		{
			return new VRDynamicAPIModule
			{
				VRDynamicAPIModuleId = reader.GetInt(COL_ID),
				Name = reader.GetString(COL_Name),
				CreatedTime = reader.GetDateTime(COL_CreatedTime),
				CreatedBy = reader.GetInt(COL_CreatedBy),
				LastModifiedTime = reader.GetDateTime(COL_LastModifiedTime),
				LastModifiedBy = reader.GetInt(COL_LastModifiedBy)
			};
		}
		#endregion
	}
}
