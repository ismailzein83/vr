using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities.EntityMigrator;

namespace TOne.WhS.DBSync.Business.SourceMigratorsReaders
{
    public class SourceCurrencyExchangeRateMigratorReader : IMigrationSourceItemReader<SourceCurrencyExchangeRate>
    {
        private string _ConnectionString;

        public SourceCurrencyExchangeRateMigratorReader(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        public IEnumerable<SourceCurrencyExchangeRate> GetSourceItems()
        {
            SourceCurrencyExchangeRateDataManager dataManager = new SourceCurrencyExchangeRateDataManager(_ConnectionString);
            return dataManager.GetSourceCurrencyExchangeRates();
        }
    }
}
