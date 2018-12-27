using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class CurrencyExchangeRateDataManager : ICurrencyExchangeRateDataManager
	{
        #region Local Variables
        static string TABLE_NAME = "common_CurrencyExchangeRate";
        static string TABLE_ALIAS = "vrCurrencyExchangeRate";
        const string COL_ID = "ID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_Rate = "Rate";
        const string COL_ExchangeDate = "ExchangeDate";
        const string COL_SourceID = "SourceID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        #endregion

        #region Constructors
        static CurrencyExchangeRateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Rate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 10 });
            columns.Add(COL_ExchangeDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "common",
                DBTableName = "CurrencyExchangeRate",
                Columns = columns,
                IdColumnName = COL_ID,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
		#endregion

		#region Public Methods
		public bool AreCurrenciesExchangeRateUpdated(ref object updateHandle)
		{
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

		public List<CurrencyExchangeRate> GetCurrenciesExchangeRate()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(CurrencyExchangeRateMapper);
		}

		public bool Insert(CurrencyExchangeRate currencyExchangeRate, out int insertedId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());

			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			insertQuery.Column(COL_Rate).Value(currencyExchangeRate.Rate);
			insertQuery.Column(COL_CurrencyID).Value(currencyExchangeRate.CurrencyId);
			insertQuery.Column(COL_ExchangeDate).Value(currencyExchangeRate.ExchangeDate);

			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_ExchangeDate).Value(currencyExchangeRate.ExchangeDate);
			ifNotExist.EqualsCondition(COL_CurrencyID).Value(currencyExchangeRate.CurrencyId);

			insertQuery.AddSelectGeneratedId();
			
			var insertedID = queryContext.ExecuteScalar().NullableIntValue;
            if (insertedID.HasValue)
            {
                insertedId = insertedID.Value;
                return true;
            }
            insertedId = -1;
            return false;
        }

		public bool Update(CurrencyExchangeRate currencyExchangeRate)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(currencyExchangeRate.CurrencyExchangeRateId);
			ifNotExist.EqualsCondition(COL_ExchangeDate).Value(currencyExchangeRate.ExchangeDate);
			ifNotExist.EqualsCondition(COL_CurrencyID).Value(currencyExchangeRate.CurrencyId);
			updateQuery.Column(COL_Rate).Value(currencyExchangeRate.Rate);
			updateQuery.Column(COL_CurrencyID).Value(currencyExchangeRate.CurrencyId);
			updateQuery.Column(COL_ExchangeDate).Value(currencyExchangeRate.ExchangeDate);
			updateQuery.Where().EqualsCondition(COL_ID).Value(currencyExchangeRate.CurrencyExchangeRateId);
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
		public CurrencyExchangeRate CurrencyExchangeRateMapper(IRDBDataReader reader)
		{
			return new CurrencyExchangeRate
			{
				CurrencyExchangeRateId = reader.GetLong(COL_ID),
				CurrencyId = reader.GetInt(COL_CurrencyID),
				Rate = reader.GetDecimal(COL_Rate),
				ExchangeDate = reader.GetDateTime(COL_ExchangeDate)
			};

		}
		#endregion
	}

}
