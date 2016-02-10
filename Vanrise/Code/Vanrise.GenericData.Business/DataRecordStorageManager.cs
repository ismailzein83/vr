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
    public class DataRecordStorageManager
    {
        #region Constructors/Fields

        DataStoreManager _dataStoreManager = new DataStoreManager();
        
        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<DataRecordStorageDetail> GetFilteredDataRecordStorages(Vanrise.Entities.DataRetrievalInput<DataRecordStorageQuery> input)
        {
            var cachedDataRecordStorages = GetCachedDataRecordStorages();

            Func<DataRecordStorage, bool> filterExpression = (dataRecordStorage) =>
                (input.Query.Name == null || dataRecordStorage.Name.ToUpper().Contains(input.Query.Name.ToUpper()))
                && (input.Query.DataRecordTypeIds == null || input.Query.DataRecordTypeIds.Contains(dataRecordStorage.DataRecordTypeId))
                && (input.Query.DataStoreIds == null || input.Query.DataStoreIds.Contains(dataRecordStorage.DataStoreId));

            return DataRetrievalManager.Instance.ProcessResult(input, cachedDataRecordStorages.ToBigResult(input, filterExpression, DataRecordStorageMapper));
        }
        public IEnumerable<DataRecordStorageInfo> GetDataRecordsStorageInfo()
        {
            var cachedDataRecordsStorage = GetCachedDataRecordStorages();
            return cachedDataRecordsStorage.MapRecords(DataRecordStorageInfoMapper);
        }

        public DataRecordStorage GetDataRecordStorage(int dataRecordStorageId)
        {
            var cachedDataRecordStorages = GetCachedDataRecordStorages();
            return cachedDataRecordStorages.FindRecord(dataRecordStorage => dataRecordStorage.DataRecordStorageId == dataRecordStorageId);
        }

        public Vanrise.Entities.InsertOperationOutput<DataRecordStorageDetail> AddDataRecordStorage(DataRecordStorage dataRecordStorage)
        {
            Vanrise.Entities.InsertOperationOutput<DataRecordStorageDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataRecordStorageDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int dataRecordStorageId = -1;

            UpdateDataStore(dataRecordStorage);

            IDataRecordStorageDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordStorageDataManager>();

            if (dataManager.AddDataRecordStorage(dataRecordStorage, out dataRecordStorageId))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                dataRecordStorage.DataRecordStorageId = dataRecordStorageId;
                insertOperationOutput.InsertedObject = DataRecordStorageMapper(dataRecordStorage);

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<DataRecordStorageDetail> UpdateDataRecordStorage(DataRecordStorage dataRecordStorage)
        {
            Vanrise.Entities.UpdateOperationOutput<DataRecordStorageDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DataRecordStorageDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = DataRecordStorageMapper(dataRecordStorage);

            IDataRecordStorageDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordStorageDataManager>();

            if (dataManager.UpdateDataRecordStorage(dataRecordStorage))
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        Dictionary<int, DataRecordStorage> GetCachedDataRecordStorages()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataRecordStorages",
                () =>
                {
                    IDataRecordStorageDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordStorageDataManager>();
                    IEnumerable<DataRecordStorage> dataRecordStorages = dataManager.GetDataRecordStorages();
                    return dataRecordStorages.ToDictionary(dataRecordStorage => dataRecordStorage.DataRecordStorageId, dataRecordStorage => dataRecordStorage);
                });
        }

        void UpdateDataStore(DataRecordStorage dataRecordStorage)
        {
            DataStore dataStore = _dataStoreManager.GeDataStore(dataRecordStorage.DataStoreId);

            if (dataStore != null && dataStore.Settings != null)
            {
                UpdateRecordStorageContext context = new UpdateRecordStorageContext(dataStore, dataRecordStorage) { RecordStorageState = null };
                dataStore.Settings.UpdateRecordStorage(context);
            }
            else
            {
                throw new ArgumentNullException("dataStoreSettings");
            }
        }

        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataRecordStorageDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordStorageDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataRecordStoragesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        DataRecordStorageDetail DataRecordStorageMapper(DataRecordStorage dataRecordStorage)
        {
            return new DataRecordStorageDetail()
            {
                Entity = dataRecordStorage
            };
        }

        DataRecordStorageInfo DataRecordStorageInfoMapper(DataRecordStorage dataRecordStorage)
        {
            return new DataRecordStorageInfo()
            {
                DataRecordStorageId = dataRecordStorage.DataRecordStorageId,
                Name = dataRecordStorage.Name
            };
        }
        #endregion
    }
}
