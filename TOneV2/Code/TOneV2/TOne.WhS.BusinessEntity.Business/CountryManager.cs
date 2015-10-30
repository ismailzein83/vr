using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CountryManager
    {

        public Vanrise.Entities.IDataRetrievalResult<Country> GetFilteredCountries(Vanrise.Entities.DataRetrievalInput<CountryQuery> input)
        {
            var allCountries = GetCachedCountries();

            Func<Country, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCountries.ToBigResult(input, filterExpression));     
        }

        public IEnumerable<Country> GetAllCountries()
        {
            var allCountries = GetCachedCountries();
            if (allCountries == null)
                return null;

            return allCountries.Values;
        }
        public Country GetCountry(int countryId)
        {
            var countries = GetCachedCountries();
            return countries.GetRecord(countryId);
        }
        public TOne.Entities.InsertOperationOutput<Country> AddCountry(Country country)
        {
            TOne.Entities.InsertOperationOutput<Country> insertOperationOutput = new TOne.Entities.InsertOperationOutput<Country>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int countryId = -1;

            ICountrytDataManager dataManager = BEDataManagerFactory.GetDataManager<ICountrytDataManager>();
            bool insertActionSucc = dataManager.Insert(country, out countryId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                country.CountryId = countryId;
                insertOperationOutput.InsertedObject = country;
            }

            return insertOperationOutput;
        } 
        public TOne.Entities.UpdateOperationOutput<Country> UpdateCountry(Country country)
        {
            ICountrytDataManager dataManager = BEDataManagerFactory.GetDataManager<ICountrytDataManager>();

            bool updateActionSucc = dataManager.Update(country);
            TOne.Entities.UpdateOperationOutput<Country> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<Country>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = country;
            }

            return updateOperationOutput;
        }
        #region Private Members

        public Dictionary<int, Country> GetCachedCountries()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCountries",
               () =>
               {
                   ICountrytDataManager dataManager = BEDataManagerFactory.GetDataManager<ICountrytDataManager>();
                   IEnumerable<Country> countries = dataManager.GetCountries();
                   return countries.ToDictionary(cn => cn.CountryId, cn => cn);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICountrytDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICountrytDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCountriesUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
