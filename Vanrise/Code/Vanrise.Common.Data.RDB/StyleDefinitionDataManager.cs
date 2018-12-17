using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class StyleDefinitionDataManager : IStyleDefinitionDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_StyleDefinition";
		static string TABLE_ALIAS = "styleDefinition";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		#endregion

		#region Constructors
		static StyleDefinitionDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "StyleDefinition",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime
			});
		}
		#endregion

		#region Public Methods
		public bool AreStyleDefinitionUpdated(ref object updateHandle)
		{
			throw new NotImplementedException();
		}

		public List<StyleDefinition> GetStyleDefinitions()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().Columns(COL_ID, COL_Name, COL_Settings);
			selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
			return queryContext.GetItems(StyleDefinitionMapper);
		}

		public bool Insert(StyleDefinition styleDefinitionItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(styleDefinitionItem.Name);
			insertQuery.Column(COL_ID).Value(styleDefinitionItem.StyleDefinitionId);
			insertQuery.Column(COL_Name).Value(styleDefinitionItem.Name);
			insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(styleDefinitionItem.StyleDefinitionSettings));
			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(StyleDefinition styleDefinitionItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
			ifNotExist.NotEqualsCondition(COL_ID).Value(styleDefinitionItem.StyleDefinitionId);
			ifNotExist.EqualsCondition(COL_Name).Value(styleDefinitionItem.Name);
			updateQuery.Column(COL_Name).Value(styleDefinitionItem.Name);
			updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(styleDefinitionItem.StyleDefinitionSettings));
			updateQuery.Where().EqualsCondition(COL_ID).Value(styleDefinitionItem.StyleDefinitionId);
			return queryContext.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Private Methods
		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common_StyleDefinition", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}

		#endregion

		#region Mappers
		StyleDefinition StyleDefinitionMapper(IRDBDataReader reader)
		{
			return new StyleDefinition
			{
				StyleDefinitionId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				StyleDefinitionSettings = Vanrise.Common.Serializer.Deserialize<StyleDefinitionSettings>(reader.GetString(COL_Settings))
			};
		}
		#endregion

	}
}
