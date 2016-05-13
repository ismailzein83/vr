using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Business.EntityMigrator;
using TOne.WhS.DBSync.Business.SourceMigratorsReaders;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;
using System.Linq;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCurrencyExchangeRateMigrator : SourceItemMigrator<SourceCurrencyExchangeRate, CurrencyExchangeRate, SourceCurrencyExchangeRateMigratorReader>
    {
        bool _UseTempTables;
        DBSyncLogger _Logger;
        public SourceCurrencyExchangeRateMigrator(SourceCurrencyExchangeRateMigratorReader sourceCurrencyExchangeRateMigratorReader, bool useTempTables, DBSyncLogger logger)
            : base(sourceCurrencyExchangeRateMigratorReader)
        {
            _UseTempTables = useTempTables;
            _Logger = logger;
        }

        public override void Migrate(List<DBTable> context)
        {
            _Logger.WriteInformation("Migrating table 'CurrencyExchangeRate' started");
            base.Migrate(context);
            _Logger.WriteInformation("Migrating table 'CurrencyExchangeRate' ended");
        }

        protected override void AddItems(List<CurrencyExchangeRate> itemsToAdd, List<DBTable> context)
        {
            CurrencyExchangeRateDBSyncManager CurrencyExchangeRateManager = new CurrencyExchangeRateDBSyncManager(_UseTempTables);
            CurrencyExchangeRateManager.ApplyCurrencyExchangeRatesToTemp(itemsToAdd);
        }

        protected override CurrencyExchangeRate BuildItemFromSource(SourceCurrencyExchangeRate sourceItem, List<DBTable> context)
        {
            DBTable dbTableCurrency = context.Where(x => x.Name == Constants.Table_Currency).FirstOrDefault();
            if (dbTableCurrency != null)
            {
                List<Currency> allCurrencies = (List<Currency>)dbTableCurrency.Records;
                Currency currency = allCurrencies.Where(x => x.SourceId == sourceItem.CurrencyId).FirstOrDefault();
                if (currency != null)
                    return new CurrencyExchangeRate
                    {
                        CurrencyId = currency.CurrencyId,
                        ExchangeDate = (sourceItem.ExchangeDate.HasValue ? sourceItem.ExchangeDate.Value : DateTime.Now),
                        Rate = (sourceItem.Rate.HasValue ? sourceItem.Rate.Value : Decimal.MinValue),
                        SourceId = sourceItem.SourceId
                    };
                else
                    return null;

            }
            else
                return null;
        }
    }
}
