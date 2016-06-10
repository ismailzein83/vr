﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Entities.DataStorage.DataRecordStorage;

namespace Vanrise.GenericData.Business
{
    public class DataRecordStorageManager
    {
        #region Constructors/Fields

        DataStoreManager _dataStoreManager = new DataStoreManager();

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<DataRecordDetail> GetFilteredDataRecords(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input)
        {
            if (input.Query.DataRecordStorageIds == null)
                throw new NullReferenceException("input.Query.DataRecordStorageIds");

            var dataRecordStorage = GetDataRecordStorage(input.Query.DataRecordStorageIds.First());
            if (dataRecordStorage == null)
                throw new NullReferenceException(String.Format("dataRecordStorage. Id '{0}'", dataRecordStorage.DataRecordStorageId));

            var recordTypeManager = new DataRecordTypeManager();
            DataRecordType recordType = recordTypeManager.GetDataRecordType(dataRecordStorage.DataRecordTypeId);
            if (recordType == null)
                throw new NullReferenceException(String.Format("recordType ID '{0}'", dataRecordStorage.DataRecordTypeId));
            if (recordType.Fields == null)
                throw new NullReferenceException(String.Format("recordType.Fields ID '{0}'", dataRecordStorage.DataRecordTypeId));

            if (input.SortByColumnName.Contains("FieldValues"))
            {
                string[] fieldValueproperty = input.SortByColumnName.Split('.');
                input.SortByColumnName = string.Format(@"{0}[""{1}""].Description", fieldValueproperty[0], fieldValueproperty[1]);
            }
            return BigDataManager.Instance.RetrieveData(input, new DataRecordRequestHandler(recordType));
        }

        public IDataRecordDataManager GetStorageDataManager(int recordStorageId)
        {
            var dataRecordStorage = GetDataRecordStorage(recordStorageId);
            if (dataRecordStorage == null)
                throw new NullReferenceException(String.Format("dataRecordStorage. Id '{0}'", recordStorageId));
            if (dataRecordStorage.Settings == null)
                throw new NullReferenceException(String.Format("dataRecordStorage.Settings. Id '{0}'", recordStorageId));

            return GetStorageDataManager(dataRecordStorage);
        }

        public Vanrise.Entities.IDataRetrievalResult<DataRecordStorageDetail> GetFilteredDataRecordStorages(Vanrise.Entities.DataRetrievalInput<DataRecordStorageQuery> input)
        {
            var cachedDataRecordStorages = GetCachedDataRecordStorages();

            Func<DataRecordStorage, bool> filterExpression = (dataRecordStorage) =>
                (input.Query.Name == null || dataRecordStorage.Name.ToUpper().Contains(input.Query.Name.ToUpper()))
                && (input.Query.DataRecordTypeIds == null || input.Query.DataRecordTypeIds.Contains(dataRecordStorage.DataRecordTypeId))
                && (input.Query.DataStoreIds == null || input.Query.DataStoreIds.Contains(dataRecordStorage.DataStoreId));

            return DataRetrievalManager.Instance.ProcessResult(input, cachedDataRecordStorages.ToBigResult(input, filterExpression, DataRecordStorageMapper));
        }
        public IEnumerable<DataRecordStorageInfo> GetDataRecordsStorageInfo(DataRecordStorageFilter filter)
        {
            Func<DataRecordStorage, bool> filterExpression = null;
            if (filter != null)
            {
                if (filter.DataRecordTypeId.HasValue)
                    filterExpression = (x) => x.DataRecordTypeId == filter.DataRecordTypeId;
            }
            return this.GetCachedDataRecordStorages().MapRecords(DataRecordStorageInfoMapper, filterExpression).OrderBy(x => x.Name);
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

            UpdateStorage(dataRecordStorage);

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

            UpdateStorage(dataRecordStorage);

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

        void UpdateStorage(DataRecordStorage dataRecordStorage)
        {
            DataStore dataStore = _dataStoreManager.GeDataStore(dataRecordStorage.DataStoreId);
            DataRecordStorage existingDataRecordStorage = GetDataRecordStorage(dataRecordStorage.DataRecordStorageId);
            if (dataStore != null && dataStore.Settings != null)
            {
                UpdateRecordStorageContext context = new UpdateRecordStorageContext()
                {
                    DataStore = dataStore,
                    RecordStorage = dataRecordStorage,
                    ExistingRecordSettings = existingDataRecordStorage != null ? existingDataRecordStorage.Settings : null,
                    RecordStorageState = existingDataRecordStorage != null ? existingDataRecordStorage.State : null
                };
                dataStore.Settings.UpdateRecordStorage(context);
            }
            else
                throw new ArgumentNullException("dataStore.Settings");
        }

        private IDataRecordDataManager GetStorageDataManager(DataRecordStorage dataRecordStorage)
        {
            var dataStore = _dataStoreManager.GeDataStore(dataRecordStorage.DataStoreId);
            if (dataStore == null)
                throw new NullReferenceException(String.Format("dataStore. dataStore Id '{0}' dataRecordStorage Id '{1}'", dataRecordStorage.DataStoreId, dataRecordStorage.DataRecordStorageId));
            if (dataStore.Settings == null)
                throw new NullReferenceException(String.Format("dataStore.Settings. dataStore Id '{0}' dataRecordStorage Id '{1}'", dataRecordStorage.DataStoreId, dataRecordStorage.DataRecordStorageId));
            var getRecordStorageDataManagerContext = new GetRecordStorageDataManagerContext
            {
                DataStore = dataStore,
                DataRecordStorage = dataRecordStorage
            };
            return dataStore.Settings.GetDataRecordDataManager(getRecordStorageDataManagerContext);
        }


        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _dataRecordTypeCacheLastCheck;

            IDataRecordStorageDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordStorageDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataRecordStoragesUpdated(ref _updateHandle)
                    |
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataRecordTypeManager.CacheManager>().IsCacheExpired(ref _dataRecordTypeCacheLastCheck);
            }
        }

        private class DataRecordRequestHandler : BigDataRequestHandler<DataRecordQuery, DataRecord, DataRecordDetail>
        {
            DataRecordType RecordType;
            public DataRecordRequestHandler(DataRecordType recordType)
            {
                RecordType = recordType;
            }
            public override DataRecordDetail EntityDetailMapper(DataRecord entity)
            {
                var dataRecordDetail = new DataRecordDetail() { RecordTime = entity.RecordTime, FieldValues = new Dictionary<string, DataRecordFieldValue>() };
                foreach (var fld in RecordType.Fields)
                {
                    DataRecordFieldValue fldValueDetail = new DataRecordFieldValue();
                    Object value;
                    if (entity.FieldValues.TryGetValue(fld.Name, out value))
                    {
                        fldValueDetail.Value = value;
                        fldValueDetail.Description = fld.Type.GetDescription(value);
                    }
                    dataRecordDetail.FieldValues.Add(fld.Name, fldValueDetail);
                }
                return dataRecordDetail;
            }

            public override IEnumerable<DataRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input)
            {
                List<DataRecord> records = new List<DataRecord>();
                foreach (int dataRecordStorageId in input.Query.DataRecordStorageIds)
                {
                    var result = GetDataRecords(input, dataRecordStorageId);
                    if (result != null)
                    {
                        records.AddRange(result);
                    }
                }
                return records.Count > 0 ? input.Query.Direction == OrderDirection.Ascending ? records.OrderBy(itm => itm.RecordTime).Take(input.Query.LimitResult) : records.OrderByDescending(itm => itm.RecordTime).Take(input.Query.LimitResult) : null;
            }

            private List<DataRecord> GetDataRecords(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input, int dataRecordStorageId)
            {
                DataRecordStorageManager manager = new DataRecordStorageManager();
                var dataRecordStorage = manager.GetDataRecordStorage(dataRecordStorageId);
                if (dataRecordStorage == null)
                    throw new NullReferenceException(String.Format("dataRecordStorage Id '{0}'", dataRecordStorageId));
                if (dataRecordStorage.Settings == null)
                    throw new NullReferenceException(String.Format("dataRecordStorage.Settings Id '{0}'", dataRecordStorageId));


                var dataManager = manager.GetStorageDataManager(dataRecordStorage);
                if (dataManager == null)
                    throw new NullReferenceException(String.Format("dataManager. ID '{0}'", dataRecordStorageId));

                return dataManager.GetFilteredDataRecords(input);
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
                Name = dataRecordStorage.Name,
                DataRecordTypeId = dataRecordStorage.DataRecordTypeId
            };
        }

        #endregion
    }
}
