using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCurrencyExchangeRateMigrator : Migrator
    {
        public SourceCurrencyExchangeRateMigrator(string connectionString, bool useTempTables, DBSyncLogger logger)
            : base(connectionString, useTempTables, logger)
        {
        }

        public override void Migrate(List<DBTable> context)
        {
            Logger.WriteInformation("Migrating table 'CurrencyExchangeRate' started");
            var sourceItems = GetSourceItems();
            if (sourceItems != null)
            {
                List<CurrencyExchangeRate> itemsToAdd = new List<CurrencyExchangeRate>();
                foreach (var sourceItem in sourceItems)
                {
                    var item = BuildItemFromSource(sourceItem, context);
                    if (item != null)
                        itemsToAdd.Add(item);
                }
                AddItems(itemsToAdd, context);
            }
            Logger.WriteInformation("Migrating table 'CurrencyExchangeRate' ended");
        }

        private  void AddItems(List<CurrencyExchangeRate> itemsToAdd, List<DBTable> context)
        {
            CurrencyExchangeRateDBSyncManager CurrencyExchangeRateManager = new CurrencyExchangeRateDBSyncManager(UseTempTables);
            CurrencyExchangeRateManager.ApplyCurrencyExchangeRatesToTemp(itemsToAdd);
        }

        private IEnumerable<SourceCurrencyExchangeRate> GetSourceItems()
        {
            SourceCurrencyExchangeRateDataManager dataManager = new SourceCurrencyExchangeRateDataManager(ConnectionString);
            return dataManager.GetSourceCurrencyExchangeRates();
        }

        private  CurrencyExchangeRate BuildItemFromSource(SourceCurrencyExchangeRate sourceItem, List<DBTable> context)
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
