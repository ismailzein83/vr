using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace Vanrise.Common.Data.RDB
{
	public class TypeDataManager : ITypeDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_Type";
		static string TABLE_ALIAS = "vrType";
		const string COL_ID = "ID";
		const string COL_Type = "Type";
		#endregion

		#region Constructors
		static TypeDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_Type, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 900 });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "Type",
				Columns = columns,
				IdColumnName = COL_ID
			});
		}
		#endregion

		#region Public Methods
		public int GetTypeId(string type)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);

			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Type).Value(type);
			insertQuery.Column(COL_Type).Value(type);

			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS);
			selectQuery.SelectColumns().Columns(COL_ID);
			selectQuery.Where().EqualsCondition(COL_Type).Value(type);
			return queryContext.ExecuteScalar().IntValue;
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		#endregion
	}
}
