using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class DataRecordFieldTypeConfigManager
    {

        //#region Public Methods
        //public IEnumerable<DataRecordFieldTypeConfig> GetDataRecordFieldTypes()
        //{
        //    var cachedDataRecordFieldTypes = GetCachedDataRecordFieldTypes();
        //    return cachedDataRecordFieldTypes.Values;
        //}

        //public DataRecordFieldTypeConfig GetDataRecordFieldTypeConfig(int configId)
        //{
        //    var cachedDataRecordFieldTypes = GetCachedDataRecordFieldTypes();
        //    return cachedDataRecordFieldTypes.GetRecord(configId);
        //}

        //#endregion

        //#region Private Methods
        //private Dictionary<Guid, DataRecordFieldTypeConfig> GetCachedDataRecordFieldTypes()
        //{
        //    return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataRecordFieldTypes",
        //       () =>
        //       {
        //           IDataRecordFieldTypeConfigDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordFieldTypeConfigDataManager>();
        //           IEnumerable<DataRecordFieldTypeConfig> dataRecordFieldTypes = dataManager.GetDataRecordFieldTypes();
        //           return dataRecordFieldTypes.ToDictionary(kvp => kvp.DataRecordFieldTypeConfigId, kvp => kvp);
        //       });
        //}

        //#endregion

        //#region Private Classes
        //private class CacheManager : Vanrise.Caching.BaseCacheManager
        //{
        //    IDataRecordFieldTypeConfigDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordFieldTypeConfigDataManager>();
        //    object _updateHandle;

        //    protected override bool ShouldSetCacheExpired(object parameter)
        //    {
        //        return _dataManager.AreDataRecordFieldTypeConfigUpdated(ref _updateHandle);
        //    }
        //}
        //#endregion
    }
}
