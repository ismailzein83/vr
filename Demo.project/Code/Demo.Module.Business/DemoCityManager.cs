using Demo.Module.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Demo.Module.Entities;
using Demo.Module.Entities.ProductInfo;

public class DemoCityManager
    {
      
       #region Public Methods
    public IDataRetrievalResult<DemoCityDetails> GetFilteredCities(DataRetrievalInput<DemoCityQuery> input)
        {
            var allCities = GetCachedCities();
            Func<DemoCity, bool> filterExpression = (demoCity) =>
            {
                if (input.Query.Name != null && !demoCity.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allCities.ToBigResult(input, filterExpression, DemoCityDetailMapper));

        }

    public string GetDemoCityName(int demoCityId)
    {
        var parent = GetDemoCityById(demoCityId);
        if (parent == null)
            return null;
        return parent.Name;
    }



    public InsertOperationOutput<DemoCityDetails> AddDemoCity(DemoCity demoCity)
        {
            IDemoCityDataManager demoCityDataManager = DemoModuleFactory.GetDataManager<IDemoCityDataManager>();
            InsertOperationOutput<DemoCityDetails> insertOperationOutput = new InsertOperationOutput<DemoCityDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int demoCityId = -1;

            bool insertActionSuccess = demoCityDataManager.Insert(demoCity, out demoCityId);
            if (insertActionSuccess)
            {
                demoCity.DemoCityId = demoCityId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = DemoCityDetailMapper(demoCity);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
    public DemoCity GetDemoCityById(int demoCityId)
        {
            var allCities = GetCachedCities();
            return allCities.GetRecord(demoCityId);
        }

    public IEnumerable<DemoCityInfo> GetDemoCitiesInfo(DemoCityInfoFilter demoCityInfoFilter)
    {
        var allCities = GetCachedCities();
        Func<DemoCity, bool> filterFunc = (demoCity) =>
        {
            if (demoCityInfoFilter != null)
            {
                var DemoCityId = demoCity.DemoCountryId;
                    if (DemoCityId!=demoCityInfoFilter.DemoCountryId)
                        {
                            return false;
                        }
                   

            }
            return true;
        };
        return allCities.MapRecords(DemoCityInfoMapper, filterFunc).OrderBy(demoCity => demoCity.Name);
    }

    public UpdateOperationOutput<DemoCityDetails> UpdateDemoCity(DemoCity demoCity)
        {
            IDemoCityDataManager demoCityDataManager = DemoModuleFactory.GetDataManager<IDemoCityDataManager>();
            UpdateOperationOutput<DemoCityDetails> updateOperationOutput = new UpdateOperationOutput<DemoCityDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = demoCityDataManager.Update(demoCity);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DemoCityDetailMapper(demoCity);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDemoCityDataManager demoCityDataManager = DemoModuleFactory.GetDataManager<IDemoCityDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return demoCityDataManager.AreDemoCitiesUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<int, DemoCity> GetCachedCities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedCities", () =>
               {
                   IDemoCityDataManager demoCityDataManager = DemoModuleFactory.GetDataManager<IDemoCityDataManager>();
                   List<DemoCity> demoCities = demoCityDataManager.GetDemoCities();
                   return demoCities.ToDictionary(demoCity => demoCity.DemoCityId, demoCity => demoCity);
               });
        }
        #endregion
    
        #region Mappers
        public DemoCityDetails DemoCityDetailMapper(DemoCity demoCity)
        {
            var demoCityDetails = new DemoCityDetails
            {
                Name = demoCity.Name,
                DemoCityId = demoCity.DemoCityId,
              
            };

            return demoCityDetails;
        }

        public DemoCityInfo DemoCityInfoMapper(DemoCity demoCity)
        {
            return new DemoCityInfo
            {
                Name = demoCity.Name,
                DemoCityId = demoCity.DemoCityId
            };
        }
        #endregion 
    
}
