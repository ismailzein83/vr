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

        public Vanrise.Entities.IDataRetrievalResult<CurrencyDetail> GetFilteredCurrencies(Vanrise.Entities.DataRetrievalInput<CurrencyQuery> input)
        {
            var allCurrencies = GetCachedCurrencies();

            Func<Currency, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                  &&
                 (input.Query.Symbol == null || prod.Symbol.ToLower().Contains(input.Query.Symbol.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCurrencies.ToBigResult(input, filterExpression, CurrencyDetailMapper));
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

        public Currency GetSystemCurrency()
        {
            SettingManager settingManager = new SettingManager();
            CurrencySettingData currencyData = settingManager.GetSetting<CurrencySettingData>(Constants.BaseCurrencySettingType);
            if (currencyData == null)
                throw new NullReferenceException("CurrencySettingData");

            var systemCurrency = GetCurrency(currencyData.CurrencyId);
            if (systemCurrency == null)
                throw new NullReferenceException(String.Format("systemCurrency ID: '{0}'", currencyData.CurrencyId));

            return systemCurrency;
        }

        public string GetCurrencyName(int currencyId)
        {
            Currency currency = this.GetCurrency(currencyId);

            if (currency != null)
                return currency.Name;

            return "Currency Not Found";
        }

        public string GetCurrencySymbol(int currencyId)
        {
            Currency currency = this.GetCurrency(currencyId);

            if (currency != null)
                return currency.Symbol;

            return "Currency Not Found";
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

        public Vanrise.Entities.InsertOperationOutput<CurrencyDetail> AddCurrency(Currency currency)
        {
            Vanrise.Entities.InsertOperationOutput<CurrencyDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CurrencyDetail>();

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
                insertOperationOutput.InsertedObject = CurrencyDetailMapper(currency);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<CurrencyDetail> UpdateCurrency(Currency currency)
        {
            ICurrencyDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICurrencyDataManager>();

            bool updateActionSucc = dataManager.Update(currency);
            Vanrise.Entities.UpdateOperationOutput<CurrencyDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CurrencyDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CurrencyDetailMapper(currency);
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
        private CurrencyDetail CurrencyDetailMapper(Currency currency)
        {
            CurrencyDetail currencDetail = new CurrencyDetail();

            currencDetail.Entity = currency;

            CurrencyManager manager = new CurrencyManager();
            if (currency.Symbol == manager.GetSystemCurrency().Symbol)
                currencDetail.IsMain = true;

            return currencDetail;
        }
        #endregion
    }
}
