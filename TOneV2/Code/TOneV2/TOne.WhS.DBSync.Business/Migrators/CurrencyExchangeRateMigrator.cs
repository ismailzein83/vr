using System;
using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class CurrencyExchangeRateMigrator : Migrator<SourceCurrencyExchangeRate, CurrencyExchangeRate>
    {
        CurrencyExchangeRateDBSyncDataManager dbSyncDataManager;
        SourceCurrencyExchangeRateDataManager dataManager;
        Dictionary<string, Currency> allCurrencies;

        public CurrencyExchangeRateMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CurrencyExchangeRateDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCurrencyExchangeRateDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableCurrency = Context.DBTables[DBTableName.Currency];
            allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            base.Migrate(context);
        }

        public override void AddItems(List<CurrencyExchangeRate> itemsToAdd)
        {
            dbSyncDataManager.ApplyCurrencyExchangeRatesToTemp(itemsToAdd);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceCurrencyExchangeRate> GetSourceItems()
        {
            return dataManager.GetSourceCurrencyExchangeRates();
        }

        public override CurrencyExchangeRate BuildItemFromSource(SourceCurrencyExchangeRate sourceItem)
        {

            Currency currency = null;
            if (allCurrencies != null)
                allCurrencies.TryGetValue(sourceItem.CurrencyId, out currency);
            if (currency != null)
                return new CurrencyExchangeRate
                {
                    CurrencyId = currency.CurrencyId,
                    ExchangeDate = (sourceItem.ExchangeDate.HasValue ? sourceItem.ExchangeDate.Value : DateTime.Now),
                    Rate = sourceItem.Rate.HasValue ? sourceItem.Rate.Value : default(decimal),
                    SourceId = sourceItem.SourceId
                };
            else
            {
                TotalRowsFailed++;
                return null;
            }
        }
        public override void FillTableInfo(bool useTempTables)
        {

        }
    }
}
