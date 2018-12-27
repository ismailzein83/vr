using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	class CurrencyDataManager : ICurrencyDataManager
	{

		static string TABLE_NAME = "common_Currency";
		static string TABLE_ALIAS = "currency";
		const string COL_ID = "ID";
		const string COL_Symbol = "Symbol";
		const string COL_Name = "Name";
		const string COL_SourceID = "SourceID";
		const string COL_CreatedBy = "CreatedBy";
		const string COL_LastModifiedBy = "LastModifiedBy";
		const string COL_LastModifiedTime = "LastModifiedTime";
		const string COL_CreatedTime = "CreatedTime";


		static CurrencyDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_Symbol, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 10 });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
			columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "Currency",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}

		#region Private Methods
		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common_Currency", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion


		public List<Currency> GetCurrencies()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
			return queryContext.GetItems(CurrencyMapper);
		}

		public bool Insert(Currency currency, out int insertedId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Symbol).Value(currency.Symbol);
			insertQuery.AddSelectGeneratedId();
			insertQuery.Column(COL_Name).Value(currency.Name);
			insertQuery.Column(COL_Symbol).Value(currency.Symbol);
			insertQuery.Column(COL_SourceID).Value(currency.SourceId);
			if (currency.CreatedBy.HasValue)
				insertQuery.Column(COL_CreatedBy).Value(currency.CreatedBy.Value);
			else
				insertQuery.Column(COL_CreatedBy).Null();
			if (currency.LastModifiedBy.HasValue)
				insertQuery.Column(COL_LastModifiedBy).Value(currency.LastModifiedBy.Value);
			else
				insertQuery.Column(COL_LastModifiedBy).Null();
			var insertedID = queryContext.ExecuteScalar().NullableIntValue;
            if (insertedID.HasValue)
            {
                insertedId = insertedID.Value;
                return true;
            }
            insertedId = -1;
            return false;
        }

		public bool Update(Currency currency)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(currency.CurrencyId);
			ifNotExist.EqualsCondition(COL_Symbol).Value(currency.Symbol);
			updateQuery.Column(COL_Name).Value(currency.Name);
			updateQuery.Column(COL_Symbol).Value(currency.Symbol);
			if (currency.LastModifiedBy.HasValue)
				updateQuery.Column(COL_LastModifiedBy).Value(currency.LastModifiedBy.Value);
			else
				updateQuery.Column(COL_LastModifiedBy).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(currency.CurrencyId);
			return queryContext.ExecuteNonQuery() > 0;

		}

		public bool AreCurrenciesUpdated(ref object updateHandle)
		{
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        } 
		#region Mappers
		public Currency CurrencyMapper(IRDBDataReader reader)
		{
			Currency currency = new Currency
			{
				CurrencyId = reader.GetInt(COL_ID),
				Name = reader.GetString(COL_Name),
				Symbol = reader.GetString(COL_Symbol),
				SourceId = reader.GetString(COL_SourceID),
				CreatedTime = reader.GetNullableDateTime(COL_CreatedTime),
				CreatedBy = reader.GetNullableInt(COL_CreatedBy),
				LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
				LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime)
			};

			return currency;
		}
		#endregion
	}
}
