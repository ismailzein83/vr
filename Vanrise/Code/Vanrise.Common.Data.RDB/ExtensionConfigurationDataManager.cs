using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class ExtensionConfigurationDataManager : IExtensionConfigurationDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_ExtensionConfiguration";
		static string TABLE_ALIAS = "extensionConfiguration";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_Title = "Title";
		const string COL_ConfigType = "ConfigType";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		#endregion

		#region Contructors
		static ExtensionConfigurationDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_ConfigType, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "ExtensionConfiguration",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime
			});
		}
		#endregion

		#region Private Methods

		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common_ExtensionConfiguration", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}

		#endregion

		#region Public Methods
		public bool AreExtensionConfigurationUpdated(string parameter, ref object updateHandle)
		{
			throw new NotImplementedException();
		}

		public List<T> GetExtensionConfigurationsByType<T>(string type) where T : ExtensionConfiguration
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().Columns(COL_ID, COL_Title,COL_Settings,COL_Name);
			selectQuery.Where().EqualsCondition(COL_ConfigType).Value(type);
			return queryContext.GetItems(ExtensionConfigurationMapper<T>);


		}
		#endregion

		#region Mappers
		T ExtensionConfigurationMapper<T>(IRDBDataReader reader) where T : ExtensionConfiguration
		{
			var extensionConfigurationObj = Vanrise.Common.Serializer.Deserialize<T>(reader.GetString(COL_Settings));
			if (extensionConfigurationObj != null)
			{
				extensionConfigurationObj.ExtensionConfigurationId =reader.GetGuid(COL_ID);
				extensionConfigurationObj.Title = reader.GetString(COL_Title);
				extensionConfigurationObj.Name = reader.GetString(COL_Name);
			}
			return extensionConfigurationObj;
		}
		#endregion

	}
}
