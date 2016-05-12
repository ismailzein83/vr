using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities.EntityMigrator;

namespace TOne.WhS.DBSync.Business.SourceMigratorsReaders
{
    public class SourceCurrencyMigratorReader : IMigrationSourceItemReader<SourceCurrency>
    {
        private string _ConnectionString;

        public SourceCurrencyMigratorReader(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        public IEnumerable<SourceCurrency> GetSourceItems()
        {
            SourceCurrencyDataManager dataManager = new SourceCurrencyDataManager(_ConnectionString);
            return dataManager.GetSourceCurrencies();
        }


    }
}
