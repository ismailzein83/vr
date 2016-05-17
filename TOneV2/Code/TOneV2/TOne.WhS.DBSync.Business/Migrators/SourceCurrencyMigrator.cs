using System.Collections.Generic;
using System.Linq;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCurrencyMigrator : Migrator<SourceCurrency, Currency>
    {
        CurrencyDBSyncDataManager dbSyncDataManager;
        SourceCurrencyDataManager dataManager;

        public SourceCurrencyMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CurrencyDBSyncDataManager(context.UseTempTables);
            dataManager = new SourceCurrencyDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<Currency> itemsToAdd)
        {
            dbSyncDataManager.ApplyCurrenciesToTemp(itemsToAdd);
            DBTable dbTableCurrency = Context.DBTables[DBTableName.Currency];
            if (dbTableCurrency != null)
                dbTableCurrency.Records = dbSyncDataManager.GetCurrencies();
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
    }
}
