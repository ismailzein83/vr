using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class SettingDataManager : ISettingDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_Setting";
		static string TABLE_ALIAS = "vrSetting";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_Type = "Type";
		const string COL_Category = "Category";
		const string COL_Settings = "Settings";
		const string COL_Data = "Data";
		const string COL_IsTechnical = "IsTechnical";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static SettingDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_Type, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_Category, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_Data, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_IsTechnical, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "Setting",
				Columns = columns,
				IdColumnName = COL_ID,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool AreSettingsUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public void GenerateScript(List<Setting> settings, Action<string, string> addEntityScript)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Setting> GetSettings()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
			return queryContext.GetItems(SettingMapper);
		}

		public bool UpdateSetting(SettingToEdit setting)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);

			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(setting.SettingId);
			ifNotExist.EqualsCondition(COL_Name).Value(setting.Name);

			updateQuery.Column(COL_Name).Value(setting.Name);

			if (setting.Data != null)
				updateQuery.Column(COL_Data).Value(Vanrise.Common.Serializer.Serialize(setting.Data));
			else
				updateQuery.Column(COL_Data).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(setting.SettingId);
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
		private Setting SettingMapper(IRDBDataReader reader)
		{
			var settings = reader.GetString(COL_Settings)  ;
			var data = reader.GetString(COL_Data)  ;
			return new Setting
			{
				SettingId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				Type = reader.GetString(COL_Type) ,
				Category = reader.GetString(COL_Category)  ,
				Settings = !string.IsNullOrEmpty(settings) ? Vanrise.Common.Serializer.Deserialize<SettingConfiguration>(settings) : null,
				Data = !string.IsNullOrEmpty(data) ? Vanrise.Common.Serializer.Deserialize<SettingData>(data) : null,
				IsTechnical =reader.GetBoolean(COL_IsTechnical)
			};
		}
		#endregion


	}
}
