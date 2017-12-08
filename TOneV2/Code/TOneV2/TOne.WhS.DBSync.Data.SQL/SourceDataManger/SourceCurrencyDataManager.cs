using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCurrencyDataManager : BaseSQLDataManager
    {
        public SourceCurrencyDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCurrency> GetSourceCurrencies()
        {
            return GetItemsText(query_getSourceCurrencies, SourceCurrencyMapper, null);
        }

        private SourceCurrency SourceCurrencyMapper(IDataReader reader)
        {
            SourceCurrency sourceCurrency = new SourceCurrency()
            {
                SourceId = reader["CurrencyID"].ToString(),
                Name = reader["Name"] as string,
                IsMainCurrency = reader["IsMainCurrency"].ToString().Equals("Y", System.StringComparison.InvariantCultureIgnoreCase) ? true : false,
                Symbol = reader["CurrencyID"].ToString(),
                LastRate = (decimal)GetReaderValue<double>(reader, "LastRate"),
                LastUpdated = reader["LastUpdated"] == DBNull.Value ? new DateTime(2000, 01, 01) : GetReaderValue<DateTime>(reader, "LastUpdated")
            };
            return sourceCurrency;
        }

        const string query_getSourceCurrencies = @"SELECT [CurrencyID] ,[IsMainCurrency] ,[Name], LastRate,LastUpdated  FROM [dbo].[Currency] WITH (NOLOCK)";
    }
}
