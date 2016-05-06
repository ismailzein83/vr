using System;
using System.Collections.Generic;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities.EntityMigrator;

namespace TOne.WhS.DBSync.Business.SourceMigratorsReaders
{
    public class SourceCurrencyExchangeRateMigratorReader : IMigrationSourceItemReader<SourceCurrencyExchangeRate>
    {
        public string ConnectionString { get; set; }

        public IEnumerable<SourceCurrencyExchangeRate> GetSourceItems()
        {
            DataManager dataManager = new DataManager(this.ConnectionString);
            return dataManager.GetSourceCurrencyExchangeRates();
        }

        private class DataManager : Vanrise.Data.SQL.BaseSQLDataManager
        {
            public DataManager(string connectionString)
                : base(connectionString, false)
            {
            }

            public List<SourceCurrencyExchangeRate> GetSourceCurrencyExchangeRates()
            {
                return GetItemsText(query_getSourceCurrencyExchangeRates, SourceCurrencyExchangeRateMapper, null);
            }

            private SourceCurrencyExchangeRate SourceCurrencyExchangeRateMapper(System.Data.IDataReader arg)
            {
                double? doubleRate = GetReaderValue<double?>(arg, "Rate");
                decimal? rate = null;
                if (doubleRate.HasValue)
                    rate = decimal.Round(decimal.Parse(doubleRate.Value.ToString()), 5, MidpointRounding.AwayFromZero);
                SourceCurrencyExchangeRate sourceCurrencyExchangeRate = new SourceCurrencyExchangeRate()
                {
                    SourceId = arg["CurrencyExchangeRateID"].ToString(),
                    ExchangeDate = GetReaderValue<DateTime?>(arg, "ExchangeDate"),
                    Rate = rate,
                    CurrencyId = arg["CurrencyId"].ToString()
                };
                return sourceCurrencyExchangeRate;
            }

            const string query_getSourceCurrencyExchangeRates = @"SELECT [CurrencyExchangeRateID] ,[CurrencyID]  ,[Rate]   ,[ExchangeDate]  ,[UserID]  ,[timestamp] FROM [dbo].[CurrencyExchangeRate]";
        }
    }
}
