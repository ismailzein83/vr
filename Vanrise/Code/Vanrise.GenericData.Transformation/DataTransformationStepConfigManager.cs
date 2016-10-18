using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.GenericData.Transformation.Data;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation
{
    public class DataTransformationStepConfigManager
    {
     
        //#region Public Methods
        //public IEnumerable<DataTransformationStepConfig> GetDataTransformationSteps()
        //{
        //    var cachedDataTransformationSteps = GetCachedDataTransformationSteps();
        //    return cachedDataTransformationSteps.Values;
        //}
        //#endregion

        //#region Private Methods
        //private Dictionary<int, DataTransformationStepConfig> GetCachedDataTransformationSteps()
        //{
        //    return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataTransformationSteps",
        //       () =>
        //       {
        //           IDataTransformationStepConfigDataManager dataManager = DataTransformationDefinitionDataManagerFactory.GetDataManager<IDataTransformationStepConfigDataManager>();
        //           IEnumerable<DataTransformationStepConfig> dataTransformationSteps = dataManager.GetDataTransformationSteps();
        //           return dataTransformationSteps.ToDictionary(kvp => kvp.DataTransformationStepConfigId, kvp => kvp);
        //       });
        //}

        //#endregion

        //#region Private Classes
        //private class CacheManager : Vanrise.Caching.BaseCacheManager
        //{
        //    IDataTransformationStepConfigDataManager _dataManager = DataTransformationDefinitionDataManagerFactory.GetDataManager<IDataTransformationStepConfigDataManager>();
        //    object _updateHandle;

        //    protected override bool ShouldSetCacheExpired(object parameter)
        //    {
        //        return _dataManager.AreDataTransformationStepConfigUpdated(ref _updateHandle);
        //    }
        //}
        //#endregion
    }
}
