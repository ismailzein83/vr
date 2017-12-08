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
        Dictionary<string, Currency> _allCurrencies;

        public CurrencyExchangeRateMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CurrencyExchangeRateDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCurrencyExchangeRateDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableCurrency = Context.DBTables[DBTableName.Currency];
            _allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
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
            if (_allCurrencies != null)
                _allCurrencies.TryGetValue(sourceItem.CurrencyId, out currency);
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
                Context.WriteWarning(string.Format("Failed migrating Currency Exchange Rate, Source Id: {0}", sourceItem.SourceId));
                TotalRowsFailed++;
                return null;
            }
        }

        public override List<CurrencyExchangeRate> BuildAllItemsFromSource(IEnumerable<SourceCurrencyExchangeRate> sourceItems)
        {
            List<CurrencyExchangeRate> result = new List<CurrencyExchangeRate>();

            Dictionary<string, List<SourceCurrencyExchangeRate>> exchangeRatesByCurrency = new Dictionary<string, List<SourceCurrencyExchangeRate>>();
            SourceCurrencyDataManager dataManager = new SourceCurrencyDataManager(Context.ConnectionString);
            List<SourceCurrency> sourceCurrencies = dataManager.GetSourceCurrencies();
            foreach (var exchangeRate in sourceItems)
            {
                List<SourceCurrencyExchangeRate> exchangeRates;

                if (!exchangeRatesByCurrency.TryGetValue(exchangeRate.CurrencyId, out exchangeRates))
                {
                    exchangeRates = new List<SourceCurrencyExchangeRate>();
                    exchangeRatesByCurrency.Add(exchangeRate.CurrencyId, exchangeRates);
                }
                exchangeRates.Add(exchangeRate);
            }

            foreach (var sourceCurrency in sourceCurrencies)
            {
                Currency currency = null;
                if (_allCurrencies != null)
                    _allCurrencies.TryGetValue(sourceCurrency.SourceId, out currency);
                if (currency == null)
                {
                    Context.WriteWarning(string.Format("Failed migrating Currency Exchange Rate, Source Id: {0}", sourceCurrency.SourceId));
                    TotalRowsFailed++;
                    continue;
                }
                List<SourceCurrencyExchangeRate> sourceExchangeRates;
                if (!exchangeRatesByCurrency.TryGetValue(sourceCurrency.SourceId, out sourceExchangeRates))
                {
                    result.Add(GetDefaultExchangeRateFromCurrency(sourceCurrency, currency));
                    Context.WriteWarning(string.Format("No Currency Exchange Rate found for Currency, Source Id: {0}. Generating Default Exchange Rate from Currency", sourceCurrency.SourceId));
                    continue;
                }
                foreach (var sourceItem in sourceExchangeRates)
                {
                    result.Add(new CurrencyExchangeRate
                    {
                        CurrencyId = currency.CurrencyId,
                        ExchangeDate = (sourceItem.ExchangeDate.HasValue ? sourceItem.ExchangeDate.Value : DateTime.Now),
                        Rate = sourceItem.Rate.HasValue ? sourceItem.Rate.Value : default(decimal),
                        SourceId = sourceItem.SourceId
                    });
                }
            }
            return result;
        }

        private CurrencyExchangeRate GetDefaultExchangeRateFromCurrency(SourceCurrency sourceCurrency, Currency currency)
        {
            return new CurrencyExchangeRate
            {
                CurrencyId = currency.CurrencyId,
                Rate = sourceCurrency.LastRate,
                SourceId = "Currency_" + sourceCurrency.SourceId,
                ExchangeDate = sourceCurrency.LastUpdated
            };
        }

        public override bool IsBuildAllItemsOnce
        {
            get
            {
                return true;
            }
        }
        public override void FillTableInfo(bool useTempTables)
        {

        }
    }
}
