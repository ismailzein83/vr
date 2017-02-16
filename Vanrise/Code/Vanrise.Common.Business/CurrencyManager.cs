using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Caching;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
namespace Vanrise.Common.Business
{
    public class CurrencyManager : IBusinessEntityManager
    {
        #region Public Methods

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
                    return this.GetCachedCurrencies().MapRecords(x => x).OrderBy(x => x.Name);
                }

                public Currency GetCurrency(int currencyId)
                {
                    var currencies = GetCachedCurrencies();
                    return currencies.GetRecord(currencyId);
                }

                public Currency GetSystemCurrency()
                {
                    ConfigManager configManager = new ConfigManager();
                    var systemCurrencyId = configManager.GetSystemCurrencyId();
                    var systemCurrency = GetCurrency(systemCurrencyId);
                    if (systemCurrency == null)
                        throw new NullReferenceException(String.Format("systemCurrency ID: '{0}'", systemCurrencyId));

                    return systemCurrency;
                }

                public string GetCurrencyName(int currencyId)
                {
                    Currency currency = this.GetCurrency(currencyId);
                    if (currency == null)
                        return null;
                    return currency.Name;
                }

                public string GetCurrencySymbol(int currencyId)
                {
                    Currency currency = this.GetCurrency(currencyId);
                    if (currency == null)
                        return null;
                    return currency.Symbol;
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

        #endregion

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

            ConfigManager manager = new ConfigManager();
            if (currency.CurrencyId == manager.GetSystemCurrencyId())
                currencDetail.IsMain = true;

            return currencDetail;
        }
        #endregion

        #region IBusinessEntityManager

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetCurrencySymbol(Convert.ToInt32(context.EntityId));
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var currency = context.Entity as Currency;
            return currency.CurrencyId;
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetCurrency(context.EntityId);
        }

        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetAllCurrencies().Select(itm => itm as dynamic).ToList();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
