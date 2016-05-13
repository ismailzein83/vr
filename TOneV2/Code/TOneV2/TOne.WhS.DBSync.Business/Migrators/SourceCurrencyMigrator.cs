using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Business.EntityMigrator;
using TOne.WhS.DBSync.Business.SourceMigratorsReaders;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;
using System.Linq;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCurrencyMigrator : SourceItemMigrator<SourceCurrency, Currency, SourceCurrencyMigratorReader>
    {
        bool _UseTempTables;
        DBSyncLogger _Logger;

        public SourceCurrencyMigrator(SourceCurrencyMigratorReader sourceCurrencyMigratorReader, bool useTempTables, DBSyncLogger logger)
            : base(sourceCurrencyMigratorReader)
        {
            _UseTempTables = useTempTables;
            _Logger = logger;
        }



        public override void Migrate(List<DBTable> context)
        {
            _Logger.WriteInformation("Migrating table 'Currency' started");
            base.Migrate(context);
            _Logger.WriteInformation("Migrating table 'Currency' ended");
        }

        protected override void AddItems(List<Currency> itemsToAdd, List<DBTable> context)
        {
            CurrencyDBSyncManager CurrencyManager = new CurrencyDBSyncManager(_UseTempTables);
            CurrencyManager.ApplyCurrenciesToTemp(itemsToAdd);
            DBTable dbTableCurrency = context.Where(x => x.Name == Constants.Table_Currency).FirstOrDefault();
            if (dbTableCurrency != null)
                dbTableCurrency.Records = CurrencyManager.GetCurrencies();
        }

        protected override Currency BuildItemFromSource(SourceCurrency sourceItem, List<DBTable> context)
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
