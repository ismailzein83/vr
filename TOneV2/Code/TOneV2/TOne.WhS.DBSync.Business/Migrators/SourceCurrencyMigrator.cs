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



        public override void Migrate(List<DBTable> context)
        {
            base.Migrate(context);
        }

        protected override void AddItems(List<Currency> itemsToAdd, List<DBTable> context)
        {
            CurrencyDBSyncManager CurrencyManager = new CurrencyDBSyncManager();
            CurrencyManager.ApplyCurrenciesToTemp(itemsToAdd);

            //sourceCurrencyMigratorReader.Context.ta

        }

        protected override Currency BuildItemFromSource(SourceCurrency sourceItem)
        {
            return new Currency
            {
                Name = sourceItem.Name,
                Symbol = sourceItem.Symbol,
                SourceId = sourceItem.SourceId
            };
        }
    }
}
