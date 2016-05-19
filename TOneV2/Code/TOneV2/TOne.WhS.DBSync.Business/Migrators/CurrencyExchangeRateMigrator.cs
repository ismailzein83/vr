using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class CurrencyExchangeRateMigrator : Migrator<SourceCurrencyExchangeRate, CurrencyExchangeRate>
    {
        CurrencyExchangeRateDBSyncDataManager dbSyncDataManager;
        SourceCurrencyExchangeRateDataManager dataManager;

        public CurrencyExchangeRateMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CurrencyExchangeRateDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCurrencyExchangeRateDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<CurrencyExchangeRate> itemsToAdd)
        {
            dbSyncDataManager.ApplyCurrencyExchangeRatesToTemp(itemsToAdd);
        }

        public override IEnumerable<SourceCurrencyExchangeRate> GetSourceItems()
        {
            return dataManager.GetSourceCurrencyExchangeRates();
        }

        public override CurrencyExchangeRate BuildItemFromSource(SourceCurrencyExchangeRate sourceItem)
        {
            DBTable dbTableCurrency = Context.DBTables[DBTableName.Currency];
            if (dbTableCurrency != null)
            {
                Dictionary<string, Currency> allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
                Currency currency = null;
                if (allCurrencies != null)
                    allCurrencies.TryGetValue(sourceItem.CurrencyId, out currency);
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
