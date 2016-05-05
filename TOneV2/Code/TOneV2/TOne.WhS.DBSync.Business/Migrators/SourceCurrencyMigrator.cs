using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Business.EntityMigrator;
using TOne.WhS.DBSync.Business.SourceMigratorsReaders;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCurrencyMigrator : SourceItemMigrator<SourceCurrency, Currency, SourceCurrencyMigratorReader>
    {
        public SourceCurrencyMigrator(SourceCurrencyMigratorReader sourceCurrencyMigratorReader)
            : base(sourceCurrencyMigratorReader)
        {

        }

        public override void Migrate()
        {
            base.Migrate();
        }

        protected override void AddItems(List<Currency> itemsToAdd)
        {
            CurrencyManager CurrencyManager = new CurrencyManager();
            CurrencyManager.AddCurrenciesFromSource(itemsToAdd);
        }

        protected override Currency BuildItemFromSource(SourceCurrency sourceItem)
        {
            return new Currency
            {
                Name = sourceItem.Name,
                Symbol = sourceItem.Symbol
            };
        }
    }
}
