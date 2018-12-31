using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class OverriddenConfigurationDataManager : IOverriddenConfigurationDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_OverriddenConfiguration";
		static string TABLE_ALIAS = "vrOverriddenConfiguration";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_GroupId = "GroupId";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static OverriddenConfigurationDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_GroupId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "OverriddenConfiguration",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool AreOverriddenConfigurationsUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public void GenerateScript(List<OverriddenConfiguration> overriddenConfigurations, Action<string, string> addEntityScript)
		{
			throw new NotImplementedException();
		}

		public List<OverriddenConfiguration> GetOverriddenConfigurations()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(OverriddenConfigurationItemMapper);
		}

		public bool Insert(OverriddenConfiguration overriddenConfiguration)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);

			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(overriddenConfiguration.Name);
			ifNotExist.EqualsCondition(COL_GroupId).Value(overriddenConfiguration.GroupId);

			insertQuery.Column(COL_ID).Value(overriddenConfiguration.OverriddenConfigurationId);
			insertQuery.Column(COL_Name).Value(overriddenConfiguration.Name);
			insertQuery.Column(COL_GroupId).Value(overriddenConfiguration.GroupId);
			if (overriddenConfiguration.Settings != null)
				insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(overriddenConfiguration.Settings));
			else
				insertQuery.Column(COL_Settings).Null();

			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(OverriddenConfiguration overriddenConfiguration)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);

			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(overriddenConfiguration.OverriddenConfigurationId);
			ifNotExist.EqualsCondition(COL_Name).Value(overriddenConfiguration.Name);
			ifNotExist.EqualsCondition(COL_GroupId).Value(overriddenConfiguration.GroupId);

			updateQuery.Column(COL_Name).Value(overriddenConfiguration.Name);
			if (overriddenConfiguration.Settings != null)
				updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(overriddenConfiguration.Settings));
			else
				updateQuery.Column(COL_Settings).Null();
			updateQuery.Column(COL_GroupId).Value(overriddenConfiguration.GroupId);
			updateQuery.Where().EqualsCondition(COL_ID).Value(overriddenConfiguration.OverriddenConfigurationId);
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
		private OverriddenConfiguration OverriddenConfigurationItemMapper(IRDBDataReader reader)
		{
			return new OverriddenConfiguration
			{
				OverriddenConfigurationId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				GroupId = reader.GetGuid(COL_GroupId),
				Settings = reader.GetString(COL_Settings)!= null ? Vanrise.Common.Serializer.Deserialize<OverriddenConfigurationSettings>(reader.GetString(COL_Settings)) : null,
			};
		}
		#endregion
	}
}
