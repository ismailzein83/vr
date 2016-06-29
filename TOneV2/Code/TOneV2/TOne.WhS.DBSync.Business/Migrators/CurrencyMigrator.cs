using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class CurrencyMigrator : Migrator<SourceCurrency, Currency>
    {
        CurrencyDBSyncDataManager dbSyncDataManager;
        SourceCurrencyDataManager dataManager;

        public CurrencyMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CurrencyDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCurrencyDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate(MigrationInfoContext context)
        {
            base.Migrate(context);
        }

        public override void AddItems(List<Currency> itemsToAdd)
        {
            dbSyncDataManager.ApplyCurrenciesToTemp(itemsToAdd);
            TotalRows = itemsToAdd.Count;
        }

        public override IEnumerable<SourceCurrency> GetSourceItems()
        {
            return dataManager.GetSourceCurrencies();
        }

        public override Currency BuildItemFromSource(SourceCurrency sourceItem)
        {
            return new Currency
            {
                Name = sourceItem.Name,
                Symbol = sourceItem.Symbol,
                SourceId = sourceItem.SourceId
            };
        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableCurrency = Context.DBTables[DBTableName.Currency];
            if (dbTableCurrency != null)
                dbTableCurrency.Records = dbSyncDataManager.GetCurrencies(useTempTables);
        }
    }
}
