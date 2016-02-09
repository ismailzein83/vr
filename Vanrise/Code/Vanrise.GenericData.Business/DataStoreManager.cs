using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
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

        public Vanrise.Entities.IDataRetrievalResult<DataStoreDetail> GetFilteredDataStores(Vanrise.Entities.DataRetrievalInput<DataStoreQuery> input)
        {
            var cachedDataStore = GetCachedDataStores();
            Func<DataStore, bool> filterExpression = (dataStore) => (input.Query.Name == null || dataStore.Name.ToUpper().Contains(input.Query.Name.ToUpper()));
            return DataRetrievalManager.Instance.ProcessResult(input, cachedDataStore.ToBigResult(input, filterExpression, DataStoreDetailMapper));
        }

        public DataStore GeDataStore(int dataStoreId)
        {
            var cachedDataStore = GetCachedDataStores();
            return cachedDataStore.FindRecord((dataStore) => dataStore.DataStoreId == dataStoreId);
        }

        public Vanrise.Entities.InsertOperationOutput<DataStoreDetail> AddDataStore(DataStore dataStore)
        {
            InsertOperationOutput<DataStoreDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataStoreDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int dataStoreId = -1;

            IDataStoreDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataStoreDataManager>();
            bool added = dataManager.AddDataStore(dataStore, out dataStoreId);

            if (added)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                dataStore.DataStoreId = dataStoreId;
                insertOperationOutput.InsertedObject = DataStoreDetailMapper(dataStore);

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetDataStores");
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<DataStoreDetail> UpdateDataStore(DataStore dataStore)
        {
            UpdateOperationOutput<DataStoreDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DataStoreDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IDataStoreDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataStoreDataManager>();
            bool updated = dataManager.UpdateDataStore(dataStore);

            if (updated)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetDataStores");
                updateOperationOutput.UpdatedObject = DataStoreDetailMapper(dataStore);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
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

        DataStoreDetail DataStoreDetailMapper(DataStore dataStore)
        {
            return new DataStoreDetail()
            {
                Entity = dataStore
            };
        }

        #endregion
    }
}
