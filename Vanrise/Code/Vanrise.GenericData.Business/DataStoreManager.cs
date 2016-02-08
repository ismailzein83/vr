using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class DataStoreManager
    {
        #region Public Methods

        public IEnumerable<DataStoreInfo> GetDataStoresInfo()
        {
            var cachedDataStores = GetCachedDataStores();
            return cachedDataStores.MapRecords(DataStoreInfoMapper);
        }

        public IDataRecordDataManager GetDataRecordDataManager(int dataRecordStorageId)
        {
            return null;
        }

        #endregion

        #region Private Methods

        Dictionary<int, DataStore> GetCachedDataStores()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataStores",
                () =>
                {
                    IDataStoreDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataStoreDataManager>();
                    IEnumerable<DataStore> dataStores = dataManager.GetDataStores();
                    return dataStores.ToDictionary(dataStore => dataStore.DataStoreId, dataStore => dataStore);
                });
        }

        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataStoreDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IDataStoreDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataStoresUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        DataStoreInfo DataStoreInfoMapper(DataStore dataStore)
        {
            return new DataStoreInfo() {
                 DataStoreId = dataStore.DataStoreId,
                 Name = dataStore.Name
            };
        }

        #endregion
    }
}
