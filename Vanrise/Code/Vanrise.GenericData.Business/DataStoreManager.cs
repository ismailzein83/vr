using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class DataStoreManager
    {
        #region Public Methods
        public IEnumerable<DataStoreConfig> GetDataStoreConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<DataStoreConfig>(DataStoreConfig.EXTENSION_TYPE);
        }
        public IEnumerable<DataStoreInfo> GetDataStoresInfo()
        {
            return this.GetCachedDataStores().MapRecords(DataStoreInfoMapper).OrderBy(x => x.Name);
        }

        public Vanrise.Entities.IDataRetrievalResult<DataStoreDetail> GetFilteredDataStores(Vanrise.Entities.DataRetrievalInput<DataStoreQuery> input)
        {
            var cachedDataStore = GetCachedDataStores();
            Func<DataStore, bool> filterExpression = (dataStore) => (input.Query.Name == null || dataStore.Name.ToUpper().Contains(input.Query.Name.ToUpper()));
            return DataRetrievalManager.Instance.ProcessResult(input, cachedDataStore.ToBigResult(input, filterExpression, DataStoreDetailMapper));
        }

        public DataStore GeDataStore(Guid dataStoreId)
        {
            var cachedDataStore = GetCachedDataStores();
            return cachedDataStore.FindRecord((dataStore) => dataStore.DataStoreId == dataStoreId);
        }

        public Vanrise.Entities.InsertOperationOutput<DataStoreDetail> AddDataStore(DataStore dataStore)
        {
            InsertOperationOutput<DataStoreDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataStoreDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            dataStore.DataStoreId = Guid.NewGuid();

            IDataStoreDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataStoreDataManager>();
            bool insertActionSucc = dataManager.AddDataStore(dataStore);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = DataStoreDetailMapper(dataStore);

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
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
            bool updateActionSucc = dataManager.UpdateDataStore(dataStore);

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
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

        Dictionary<Guid, DataStore> GetCachedDataStores()
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
