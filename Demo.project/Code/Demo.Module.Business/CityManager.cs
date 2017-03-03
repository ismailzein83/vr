using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;

namespace Demo.Module.Business
{
  public  class CityManager
    {
      public IDataRetrievalResult<CityDetails> GetFilteredCities(Vanrise.Entities.DataRetrievalInput<CityQ> input)
      {

          var cities = GetCachedCities();
          

          Func<Demo.Module.Entities.City, bool> filterExpression = (prod) =>
              {
                  if (input.Query.Name != null && !prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                      return false;
                  return true;
              };
              




          return DataRetrievalManager.Instance.ProcessResult(input, cities.ToBigResult(input, filterExpression, CityDetailMapper));
      }


      public Vanrise.Entities.InsertOperationOutput<CityDetails> AddCity(Demo.Module.Entities.City city)
      {
          Vanrise.Entities.InsertOperationOutput<CityDetails> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CityDetails>();

          insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
          insertOperationOutput.InsertedObject = null;

          int cityId = -1;

          ICityDataManager dataManager = DemoModuleFactory.GetDataManager<ICityDataManager>();
          bool insertActionSucc = dataManager.Insert(city, out cityId);
          if (insertActionSucc)
          {
              Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
              insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
              city.Id = cityId;
              insertOperationOutput.InsertedObject = CityDetailMapper(city);
          }
          else
          {
              insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
          }

          return insertOperationOutput;
      }

      public Demo.Module.Entities.City GetCity(int Id)
      {
          var cities = GetCachedCities();
          return cities.GetRecord(Id);
      }
      public Vanrise.Entities.UpdateOperationOutput<CityDetails> UpdateCity(Demo.Module.Entities.City city)
      {
          ICityDataManager dataManager = DemoModuleFactory.GetDataManager<ICityDataManager>();

          bool updateActionSucc = dataManager.Update(city);
          Vanrise.Entities.UpdateOperationOutput<CityDetails> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CityDetails>();

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

      public IEnumerable<Demo.Module.Entities.CityInfo> GetCitiesInfo(CityFilter filter)
      {
          ICityDataManager dataManager = DemoModuleFactory.GetDataManager<ICityDataManager>();
          IEnumerable<Demo.Module.Entities.City> cities = dataManager.GetCities();
          cities.ToDictionary(c => c.Id, c => c);
          Func<Demo.Module.Entities.City, bool> filterFunc = null;
          if (filter != null)
          {
              filterFunc = (city) =>
              {
                  if (filter.ExcludedCityIds != null && filter.ExcludedCityIds.Contains(city.Id))
                      return false;

                  if (filter.Filters != null)
                  {
                      var context = new CityFilterContext() { city = city };
                      if (filter.Filters.Any(x => x.IsExcluded(context)))
                          return false;
                  }

                  return true;
              };
          }
          IEnumerable<Demo.Module.Entities.City> filteredCities = (filterFunc != null) ? cities.FindAllRecords(filterFunc) : cities;
          return filteredCities.MapRecords(CityInfoMapper).OrderBy(x => x.Name);
      }

      private class CacheManager : Vanrise.Caching.BaseCacheManager
      {
          ICityDataManager _dataManager = DemoModuleFactory.GetDataManager<ICityDataManager>();
          object _updateHandle;

          protected override bool ShouldSetCacheExpired(object parameter)
          {
              return _dataManager.AreCitiesUpdated(ref _updateHandle);
          }
      }


      private Dictionary<int, Demo.Module.Entities.City> GetCachedCities()
      {
          return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCities",
            () =>
            {
                ICityDataManager dataManager = DemoModuleFactory.GetDataManager<ICityDataManager>();
                IEnumerable<Demo.Module.Entities.City> cities = dataManager.GetCities();
                return cities.ToDictionary(c => c.Id, c => c);
            });
      }



      public CityDetails CityDetailMapper(Demo.Module.Entities.City city)
      {
          CityDetails cityDetail = new CityDetails();

          
          

          cityDetail.Entity = city;
         
          return cityDetail;
      }

      private Demo.Module.Entities.CityInfo CityInfoMapper(Demo.Module.Entities.City city)
      {
          Demo.Module.Entities.CityInfo cityInfo = new Demo.Module.Entities.CityInfo();

          cityInfo.Id = city.Id;
          cityInfo.Name = city.Name;
          return cityInfo;
      }



    }
}
