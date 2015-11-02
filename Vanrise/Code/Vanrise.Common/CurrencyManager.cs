using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Caching;
using Vanrise.Entities;
namespace Vanrise.Common
{
    public class CurrencyManager
    {

        public Vanrise.Entities.IDataRetrievalResult<Currency> GetFilteredCurrencies(Vanrise.Entities.DataRetrievalInput<CurrencyQuery> input)
        {
            var allCurrencies = GetCachedCurrencies();

            Func<Currency, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                  &&
                 (input.Query.Symbol == null || prod.Symbol.ToLower().Contains(input.Query.Symbol.ToLower())) ;

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCurrencies.ToBigResult(input, filterExpression));     
        }

        public IEnumerable<Currency> GetAllCurrencies()
        {
            var allCountries = GetCachedCurrencies();
            if (allCountries == null)
                return null;

            return allCountries.Values;
        }
      
        #region Private Members

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
