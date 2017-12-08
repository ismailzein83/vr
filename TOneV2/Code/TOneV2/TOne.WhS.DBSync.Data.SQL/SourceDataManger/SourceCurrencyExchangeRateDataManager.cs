using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCurrencyExchangeRateDataManager : BaseSQLDataManager
    {
        public SourceCurrencyExchangeRateDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCurrencyExchangeRate> GetSourceCurrencyExchangeRates()
        {
            return GetItemsText(query_getSourceCurrencyExchangeRates, SourceCurrencyExchangeRateMapper, null);
        }

        private SourceCurrencyExchangeRate SourceCurrencyExchangeRateMapper(IDataReader arg)
        {
            double? rate = GetReaderValue<double?>(arg, "Rate");
            return new SourceCurrencyExchangeRate()
             {
                 SourceId = arg["CurrencyExchangeRateID"].ToString(),
                 ExchangeDate = GetReaderValue<DateTime?>(arg, "ExchangeDate"),
                 Rate = rate.HasValue ? Convert.ToDecimal(rate.Value) : default(decimal?),
                 CurrencyId = arg["CurrencyId"].ToString(),
             };

        }

        const string query_getSourceCurrencyExchangeRates = @"SELECT [CurrencyExchangeRateID] ,[CurrencyID]  ,[Rate]   ,[ExchangeDate]  FROM [dbo].[CurrencyExchangeRate] WITH (NOLOCK) order by CurrencyID";
    }
}
