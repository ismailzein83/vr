using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class DataEncryptionKeyDataManager : IDataEncryptionKeyDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_DataEncryptionKey";
		static string TABLE_ALIAS = "vrDataEncryptionKey";
		const string COL_ID = "ID";
		const string COL_EncryptionKey = "EncryptionKey";
		#endregion

		#region Constructors
		static DataEncryptionKeyDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_EncryptionKey, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "DataEncryptionKey",
				Columns = columns,
				IdColumnName = COL_ID
			});
		}
		#endregion

		#region Public Methods
		public string InsertIfNotExistsAndGetDataEncryptionKey(string keyToInsertIfNotExists)
		{
			var queryContextToInsert = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContextToInsert.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			queryContextToInsert.ExecuteNonQuery();

			var queryContextToSelect = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContextToSelect.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1);
			selectQuery.Sort().ByColumn(TABLE_ALIAS,COL_ID,RDBSortDirection.ASC);
			
			return queryContextToSelect.ExecuteScalar().StringValue;
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
