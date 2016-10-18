using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class DataStoreConfigManager
    {
        //#region Public Methods
        //public IEnumerable<DataStoreConfig> GetDataStoreConfigs()
        //{
        //    return this.GetCachedDataStoreConfigs().MapRecords(x => x).OrderBy(x => x.Name);
        //}

        //public DataStoreConfig GeDataStoreConfigById(int dataStoreConfigId)
        //{
        //    var cachedGenericRuleTypes = GetCachedDataStoreConfigs();
        //    return cachedGenericRuleTypes.FindRecord(x => x.DataStoreConfigId == dataStoreConfigId);
        //}

        //#endregion

        //#region Private Methods
        //private Dictionary<int, DataStoreConfig> GetCachedDataStoreConfigs()
        //{
        //    return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataStoreConfigs",
        //       () =>
        //       {
        //           IDataStoreConfigDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataStoreConfigDataManager>();
        //           IEnumerable<DataStoreConfig> denericRuleTypes = dataManager.GetDataStoreConfigs();
        //           return denericRuleTypes.ToDictionary(c => c.DataStoreConfigId , c => c);
        //       });
        //}

        //#endregion

        //#region Private Classes
        //private class CacheManager : Vanrise.Caching.BaseCacheManager
        //{
        //    IDataStoreConfigDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IDataStoreConfigDataManager>();
        //    object _updateHandle;

        //    protected override bool ShouldSetCacheExpired(object parameter)
        //    {
        //        return _dataManager.AreDataStoreConfigUpdated(ref _updateHandle);
        //    }
        //}
        //#endregion
    }
}
