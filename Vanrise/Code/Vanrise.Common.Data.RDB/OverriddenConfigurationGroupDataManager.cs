using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class OverriddenConfigurationGroupDataManager : IOverriddenConfigurationGroupDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_OverriddenConfigurationGroup";
		static string TABLE_ALIAS = "vrOverriddenConfigurationGroup";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static OverriddenConfigurationGroupDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "OverriddenConfigurationGroup",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool AreOverriddenConfigurationGroupUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public void GenerateScript(List<OverriddenConfigurationGroup> groups, Action<string, string> addEntityScript)
		{
			throw new NotImplementedException();
		}

		public List<OverriddenConfigurationGroup> GetOverriddenConfigurationGroup()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(OverriddenConfigurationGroupMapper);
		}

		public bool Insert(OverriddenConfigurationGroup overriddenConfigurationGroup)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);

			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(overriddenConfigurationGroup.Name);

			insertQuery.Column(COL_ID).Value(overriddenConfigurationGroup.OverriddenConfigurationGroupId);
			insertQuery.Column(COL_Name).Value(overriddenConfigurationGroup.Name);

			return queryContext.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		private OverriddenConfigurationGroup OverriddenConfigurationGroupMapper(IRDBDataReader reader)
		{
			return new OverriddenConfigurationGroup
			{
				OverriddenConfigurationGroupId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name)
			};
		}
		#endregion

	}
}
