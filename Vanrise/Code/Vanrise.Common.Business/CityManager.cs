using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class CityManager
    {
        #region Public Methods

        public IDataRetrievalResult<CityDetail> GetFilteredCities(Vanrise.Entities.DataRetrievalInput<CityQuery> input)
        {
            var allCities = GetCachedCities();

            Func<City, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.CountryIds == null || input.Query.CountryIds.Contains(prod.CountryId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCities.ToBigResult(input, filterExpression, CityDetailMapper));
        }

        public IEnumerable<CityInfo> GetCitiesInfo(int countryId)
        {
            return this.GetCachedCities().MapRecords(CityInfoMapper, city => city.CountryId == countryId).OrderBy(city => city.Name);
        }

        public IEnumerable<int> GetDistinctCountryIdsByCityIds(IEnumerable<int> cityIds)
        {
            return this.GetCachedCities().MapRecords(city => city.CountryId, city => cityIds.Contains(city.CityId)).Distinct();
        }

    
        public City GetCity(int cityId)
        {
            var cities = GetCachedCities();
            return cities.GetRecord(cityId);
        }

        public Vanrise.Entities.InsertOperationOutput<CityDetail> AddCity(City city)
        {
            Vanrise.Entities.InsertOperationOutput<CityDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CityDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int cityId = -1;

            ICityDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICityDataManager>();
            bool insertActionSucc = dataManager.Insert(city, out cityId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                city.CityId = cityId;
                insertOperationOutput.InsertedObject = CityDetailMapper(city);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<CityDetail> UpdateCity(City city)
        {
            ICityDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICityDataManager>();

            bool updateActionSucc = dataManager.Update(city);
            Vanrise.Entities.UpdateOperationOutput<CityDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CityDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CityDetailMapper(city);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public string GetCityName(int cityId)
        {
            var city = this.GetCity(cityId);
            if (city != null)
                return city.Name;
            else
                return null;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICityDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ICityDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCitiesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, City> GetCachedCities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCities",
              () =>
              {
                  ICityDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICityDataManager>();
                  IEnumerable<City> cities = dataManager.GetCities();
                  return cities.ToDictionary(c => c.CityId, c => c);
              });
        }

        #endregion

        #region Mappers

        public CityDetail CityDetailMapper(City city)
        {
            CityDetail cityDetail = new CityDetail();

            CountryManager countryManager = new CountryManager();
            Country country = countryManager.GetCountry(city.CountryId);

            cityDetail.Entity = city;
            cityDetail.CountryName = (country != null ? country.Name : string.Empty);
            return cityDetail;
        }

        public CityInfo CityInfoMapper(City city)
        {
            CityInfo cityInfo = new CityInfo();
            cityInfo.CityId = city.CityId;
            cityInfo.Name = city.Name;
            cityInfo.CountryId = city.CountryId;
            return cityInfo;
        }

        #endregion
    }
}
