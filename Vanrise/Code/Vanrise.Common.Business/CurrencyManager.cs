using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Caching;
using Vanrise.Entities;
namespace Vanrise.Common.Business
{
    public class CurrencyManager
    {

        public Vanrise.Entities.IDataRetrievalResult<Currency> GetFilteredCurrencies(Vanrise.Entities.DataRetrievalInput<CurrencyQuery> input)
        {
            var allCurrencies = GetCachedCurrencies();

            Func<Currency, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                  &&
                 (input.Query.Symbol == null || prod.Symbol.ToLower().Contains(input.Query.Symbol.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCurrencies.ToBigResult(input, filterExpression));
        }

        public IEnumerable<Currency> GetAllCurrencies()
        {
            var allCountries = GetCachedCurrencies();
            if (allCountries == null)
                return null;

            return allCountries.Values;
        }
        public Currency GetCurrency(int currencyId)
        {
            var currencies = GetCachedCurrencies();
            return currencies.GetRecord(currencyId);
        }

        public Dictionary<int, Currency> GetCachedCurrencies()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCurrencies",
               () =>
               {
                   ICurrencyDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyDataManager>();
                   IEnumerable<Currency> currencies = dataManager.GetCurrencies();
                   return currencies.ToDictionary(cu => cu.CurrencyId, cu => cu);
               });
        }

        public Vanrise.Entities.InsertOperationOutput<Currency> AddCurrency(Currency currency)
        {
            Vanrise.Entities.InsertOperationOutput<Currency> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<Currency>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int currencyId = -1;

            ICurrencyDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyDataManager>();
            bool insertActionSucc = dataManager.Insert(currency, out currencyId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                currency.CurrencyId = currencyId;
                insertOperationOutput.InsertedObject = currency;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<Currency> UpdateCurrency(Currency currency)
        {
            ICurrencyDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyDataManager>();

            bool updateActionSucc = dataManager.Update(currency);
            Vanrise.Entities.UpdateOperationOutput<Currency> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<Currency>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = currency;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICurrencyDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCurrenciesUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
