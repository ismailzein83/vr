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
            double? doubleRate = GetReaderValue<double?>(arg, "Rate");
            decimal? rate = null;
            if (doubleRate.HasValue)
                rate = decimal.Round(decimal.Parse(doubleRate.Value.ToString()), 5, MidpointRounding.AwayFromZero);
            return new SourceCurrencyExchangeRate()
             {
                 SourceId = arg["CurrencyExchangeRateID"].ToString(),
                 ExchangeDate = GetReaderValue<DateTime?>(arg, "ExchangeDate"),
                 Rate = rate,
                 CurrencyId = arg["CurrencyId"].ToString(),
             };

        }

        const string query_getSourceCurrencyExchangeRates = @"SELECT [CurrencyExchangeRateID] ,[CurrencyID]  ,[Rate]   ,[ExchangeDate]  FROM [dbo].[CurrencyExchangeRate] WITH (NOLOCK)";
    }
}
