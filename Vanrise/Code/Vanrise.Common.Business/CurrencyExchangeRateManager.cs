using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class CurrencyExchangeRateManager
    {
        public CurrencyExchangeRate GetEffectiveExchangeRate(int currencyId, DateTime effectiveOn)
        {
            var allCurrenciesExchangeRates = GetCachedCurrenciesExchangeRates();
            return allCurrenciesExchangeRates.FindRecord(x => x.CurrencyId == currencyId && x.ExchangeDate >= effectiveOn);
        }
        public Decimal ConvertValueToCurrency(decimal value, int currencyId, DateTime effectiveOn)
        {
            var exchangeRate = GetEffectiveExchangeRate(currencyId, effectiveOn);
            if (exchangeRate != null)
                return value * exchangeRate.Rate;
            else
                return value;

        }
        public Vanrise.Entities.IDataRetrievalResult<CurrencyExchangeRateDetail> GetFilteredCurrenciesExchangeRates(Vanrise.Entities.DataRetrievalInput<CurrencyExchangeRateQuery> input)
        {
            var allCurrenciesExchangeRates = GetCachedCurrenciesExchangeRates();



            var filteredExchangeRates = allCurrenciesExchangeRates.FindAllRecords((prod) =>
                 (input.Query.Currencies == null || input.Query.Currencies.Contains(prod.CurrencyId))
                  && (!input.Query.ExchangeDate.HasValue || input.Query.ExchangeDate >= prod.ExchangeDate));

            //if exchange rate is specified, retrieve only latest exchange rate for each currency
            if (filteredExchangeRates != null && input.Query.ExchangeDate.HasValue)
            {
                List<CurrencyExchangeRate> filteredExchangeRatesWithDistinctCurrencies = new List<CurrencyExchangeRate>();
                HashSet<int> addedCurrencies = new HashSet<int>();
                foreach (var ex in filteredExchangeRates.OrderByDescending(itm => itm.ExchangeDate))
                {
                    if (!addedCurrencies.Contains(ex.CurrencyId))
                    {
                        filteredExchangeRatesWithDistinctCurrencies.Add(ex);
                        addedCurrencies.Add(ex.CurrencyId);
                    }
                }
                filteredExchangeRates = filteredExchangeRatesWithDistinctCurrencies;
            }

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, filteredExchangeRates.ToBigResult(input, null, CurrencyExchangeRateDetailMapper));
        }
        public IEnumerable<CurrencyExchangeRate> GetAllCurrenciesExchangeRate()
        {
            var allExchangeRates = GetCachedCurrenciesExchangeRates();
            if (allExchangeRates == null)
                return null;

            return allExchangeRates.Values;
        }
        public Dictionary<long, CurrencyExchangeRate> GetCachedCurrenciesExchangeRates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCurrenciesExchangeRate",
               () =>
               {
                   ICurrencyExchangeRateDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyExchangeRateDataManager>();
                   IEnumerable<CurrencyExchangeRate> exchangeRates = dataManager.GetCurrenciesExchangeRate();
                   return exchangeRates.ToDictionary(ex => ex.CurrencyExchangeRateId, ex => ex);
               });
        }
        public CurrencyExchangeRate GetCurrencyExchangeRate(int currencyExchangeRateId)
        {
            var allCurrenciesExchangeRates = GetCachedCurrenciesExchangeRates();
            return allCurrenciesExchangeRates.GetRecord(currencyExchangeRateId);
        }
        public Vanrise.Entities.InsertOperationOutput<CurrencyExchangeRateDetail> AddCurrencyExchangeRate(CurrencyExchangeRate currencyExchangeRate)
        {
            Vanrise.Entities.InsertOperationOutput<CurrencyExchangeRateDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CurrencyExchangeRateDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int currencyExchangeRateId = -1;

            ICurrencyExchangeRateDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyExchangeRateDataManager>();
            bool insertActionSucc = dataManager.Insert(currencyExchangeRate, out currencyExchangeRateId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                currencyExchangeRate.CurrencyExchangeRateId = currencyExchangeRateId;
                insertOperationOutput.InsertedObject = CurrencyExchangeRateDetailMapper(currencyExchangeRate);
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<CurrencyExchangeRateDetail> UpdateCurrencyExchangeRate(CurrencyExchangeRate currencyExchangeRate)
        {
            ICurrencyExchangeRateDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyExchangeRateDataManager>();

            bool updateActionSucc = dataManager.Update(currencyExchangeRate);
            Vanrise.Entities.UpdateOperationOutput<CurrencyExchangeRateDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CurrencyExchangeRateDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CurrencyExchangeRateDetailMapper(currencyExchangeRate);
            }

            return updateOperationOutput;
        }

        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICurrencyExchangeRateDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyExchangeRateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCurrenciesExchangeRateUpdated(ref _updateHandle);
            }
        }

        private CurrencyExchangeRateDetail CurrencyExchangeRateDetailMapper(CurrencyExchangeRate currencyExchangeRate)
        {
            CurrencyExchangeRateDetail currencyExchangeRateDetail = new CurrencyExchangeRateDetail();

            currencyExchangeRateDetail.Entity = currencyExchangeRate;

            CurrencyManager manager = new CurrencyManager();
            if (currencyExchangeRate.CurrencyId != null)
            {
                int currencyId = (int)currencyExchangeRate.CurrencyId;
                currencyExchangeRateDetail.CurrencyName = manager.GetCurrency(currencyId).Name;
            }

            return currencyExchangeRateDetail;
        }

        #endregion
    }
}
