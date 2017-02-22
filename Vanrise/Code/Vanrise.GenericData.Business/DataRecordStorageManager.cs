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
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Entities.DataStorage.DataRecordStorage;
using Vanrise.Security.Business;

namespace Vanrise.GenericData.Business
{
    public class DataRecordStorageManager : IDataRecordStorageManager
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

            DataStore dataStore = new DataStoreManager().GetDataStore(dataRecordStorage.DataStoreId);
            if (dataStore == null)
                throw new NullReferenceException(String.Format("dataStore. Id '{0}'", dataRecordStorage.DataStoreId));

            if (dataStore.Settings == null)
                throw new NullReferenceException(String.Format("dataStore.Settings. Id '{0}'", dataRecordStorage.DataStoreId));

            var remoteRecordDataManager = dataStore.Settings.GetRemoteRecordDataManager(new GetRemoteRecordStorageDataManagerContext() { DataStore = dataStore, DataRecordStorage = dataRecordStorage });
            
            if (remoteRecordDataManager != null)
                return remoteRecordDataManager.GetFilteredDataRecords(input);
            else
            {
                if (input.Query.Columns == null)
                    throw new NullReferenceException("input.Query.Columns");

                input.Query.Columns = new HashSet<string>(input.Query.Columns).ToList();

                var recordTypeManager = new DataRecordTypeManager();
                DataRecordType recordType = recordTypeManager.GetDataRecordType(dataRecordStorage.DataRecordTypeId);
                if (recordType == null)
                    throw new NullReferenceException(String.Format("recordType ID '{0}'", dataRecordStorage.DataRecordTypeId));
                if (recordType.Fields == null)
                    throw new NullReferenceException(String.Format("recordType.Fields ID '{0}'", dataRecordStorage.DataRecordTypeId));

                if (input.Query.FilterGroup != null)
                {
                    var filterGroup = ConvertFilterGroup(input.Query.FilterGroup, recordType);
                    input.Query.FilterGroup = filterGroup;
                }

                if (input.SortByColumnName.Contains("FieldValues"))
                {
                    string[] fieldValueproperty = input.SortByColumnName.Split('.');
                    input.SortByColumnName = string.Format(@"{0}[""{1}""].{2}", fieldValueproperty[0], fieldValueproperty[1], fieldValueproperty[2]);
                }
                return BigDataManager.Instance.RetrieveData(input, new DataRecordRequestHandler() { DataRecordTypeId = dataRecordStorage.DataRecordTypeId });
            }
        }

        private RecordFilterGroup ConvertFilterGroup(RecordFilterGroup filterGroup, DataRecordType recordType)
        {
            RecordFilterGroup recordFilterGroup = new RecordFilterGroup();
            recordFilterGroup.LogicalOperator = filterGroup.LogicalOperator;
            foreach (var filter in filterGroup.Filters)
            {
                ConvertChildFilterGroup(recordFilterGroup, filter, recordType);
            }
            return recordFilterGroup;

        }
        public void ConvertChildFilterGroup(RecordFilterGroup recordFilterGroup, RecordFilter recordFilter, DataRecordType recordType)
        {
            RecordFilterGroup childFilterGroup = recordFilter as RecordFilterGroup;
            if (recordFilterGroup.Filters == null)
                recordFilterGroup.Filters = new List<RecordFilter>();
            if (childFilterGroup != null)
            {
                RecordFilterGroup childRecordFilterGroup = new RecordFilterGroup();
                childRecordFilterGroup.LogicalOperator = childFilterGroup.LogicalOperator;
                recordFilterGroup.Filters.Add(childRecordFilterGroup);
                foreach (var filter in childFilterGroup.Filters)
                {
                    ConvertChildFilterGroup(childRecordFilterGroup, filter, recordType);
                }
            }
            else if (recordFilter.FieldName != null)
            {
                var record = recordType.Fields.FindRecord(x => x.Name == recordFilter.FieldName);
                if (record != null)
                {
                    if (record.Formula != null)
                    {
                        DataRecordFieldFormulaConvertFilterContext context = new DataRecordFieldFormulaConvertFilterContext(recordType.DataRecordTypeId, recordFilter.FieldName);
                        context.InitialFilter = recordFilter;
                        var recordFilterObj = record.Formula.ConvertFilter(context);
                        recordFilterGroup.Filters.Add(recordFilterObj);
                    }
                    else
                    {
                        recordFilter.FieldName = record.Name;
                        recordFilterGroup.Filters.Add(recordFilter);
                    }

                }
            }
        }

        public IDataRecordDataManager GetStorageDataManager(Guid recordStorageId)
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
            Func<DataRecordStorage, bool> filterExpression = (dataRecordStorage) =>
            {
                if (filter == null)
                    return true;

                if (filter.DataRecordTypeId.HasValue && dataRecordStorage.DataRecordTypeId != filter.DataRecordTypeId)
                    return false;

                if (filter.Filters != null && filter.Filters.Count > 0)
                {
                    foreach (IDataRecordStorageFilter dataRecordStorageFilter in filter.Filters)
                    {
                        if (!dataRecordStorageFilter.IsMatched(dataRecordStorage))
                            return false;
                    }
                }

                return true;
            };
            return this.GetCachedDataRecordStorages().MapRecords(DataRecordStorageInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public IEnumerable<DataRecordStorageInfo> GetRemoteDataRecordsStorageInfo(Guid connectionId, string serializedFilter)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            return connectionSettings.Get<IEnumerable<DataRecordStorageInfo>>(string.Format("/api/VR_GenericData/DataRecordStorage/GetDataRecordsStorageInfo?filter={0}", serializedFilter));
        }

        public DataRecordStorage GetDataRecordStorage(Guid dataRecordStorageId)
        {
            var cachedDataRecordStorages = GetCachedDataRecordStorages();
            return cachedDataRecordStorages.FindRecord(dataRecordStorage => dataRecordStorage.DataRecordStorageId == dataRecordStorageId);
        }

        public Vanrise.Entities.InsertOperationOutput<DataRecordStorageDetail> AddDataRecordStorage(DataRecordStorage dataRecordStorage)
        {
            Vanrise.Entities.InsertOperationOutput<DataRecordStorageDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataRecordStorageDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            dataRecordStorage.DataRecordStorageId = Guid.NewGuid();
            UpdateStorage(dataRecordStorage);

            IDataRecordStorageDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordStorageDataManager>();

            if (dataManager.AddDataRecordStorage(dataRecordStorage))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
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

        public void GetDataRecords(Guid dataRecordStorageId, DateTime from, DateTime to, Action<dynamic> onItemReady)
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

            dataManager.GetDataRecords(from, to, onItemReady);
        }
        public List<Guid> CheckRecordStoragesAccess(List<Guid> dataRecordStorages)
        {
            var allRecordStorages = GetCachedDataRecordStorages().Where(k => dataRecordStorages.Contains(k.Key)).Select(v => v.Value).ToList();
            List<Guid> filterdRecrodsIds = new List<Guid>();

            foreach (var r in allRecordStorages)
            {
                if (DoesUserHaveAccessOnRecordStorage(SecurityContext.Current.GetLoggedInUserId(), r))
                    filterdRecrodsIds.Add(r.DataRecordStorageId);
            }
            return filterdRecrodsIds;
        }
        public bool DoesUserHaveAccess(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input)
        {

            return this.DoesUserHaveAccess(SecurityContext.Current.GetLoggedInUserId(), input.Query.DataRecordStorageIds);
        }
        public bool DoesUserHaveAccess(int userId, List<Guid> dataRecordStorages)
        {
            var allRecordStorages = GetCachedDataRecordStorages().Where(k => dataRecordStorages.Contains(k.Key)).Select(v => v.Value).ToList();
            foreach (var r in allRecordStorages)
            {
                if (!DoesUserHaveAccessOnRecordStorage(userId, r))
                    return false;
            }
            return true;
        }

        public IEnumerable<VRRestAPIRecordQueryInterceptorConfig> GetVRRestAPIRecordQueryInterceptorConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<VRRestAPIRecordQueryInterceptorConfig>(VRRestAPIRecordQueryInterceptorConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Private Methods

        bool DoesUserHaveAccessOnRecordStorage(int userId, DataRecordStorage dataRecordStorage)
        {
            SecurityManager secManager = new SecurityManager();
            if (dataRecordStorage.Settings.RequiredPermission != null && !secManager.IsAllowed(dataRecordStorage.Settings.RequiredPermission, userId))
                return false;
            return true;
        }
        Dictionary<Guid, DataRecordStorage> GetCachedDataRecordStorages()
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
            DataStore dataStore = _dataStoreManager.GetDataStore(dataRecordStorage.DataStoreId);
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

        IDataRecordDataManager GetStorageDataManager(DataRecordStorage dataRecordStorage)
        {
            var dataStore = _dataStoreManager.GetDataStore(dataRecordStorage.DataStoreId);
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
            public Guid DataRecordTypeId { get; set; }

            private DataRecordType _recordType;

            private DataRecordType RecordType
            {
                get
                {
                    if (_recordType == null)
                    {
                        var recordTypeManager = new DataRecordTypeManager();
                        _recordType = recordTypeManager.GetDataRecordType(DataRecordTypeId);
                    }
                    return _recordType;
                }
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

            private List<DataRecordDetail> DataRecordDetailMapperList(IEnumerable<DataRecord> dataRecords)
            {
                if (dataRecords == null)
                    return null;

                List<DataRecordDetail> result = new List<DataRecordDetail>();
                foreach (DataRecord dataRecord in dataRecords)
                    result.Add(EntityDetailMapper(dataRecord));

                return result;
            }
            public override IEnumerable<DataRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input)
            {
                List<DataRecord> records = new List<DataRecord>();
                foreach (Guid dataRecordStorageId in input.Query.DataRecordStorageIds)
                {
                    var result = GetDataRecords(input, dataRecordStorageId);
                    if (result != null)
                    {
                        records.AddRange(result);
                    }
                }
                return records.Count > 0 ? input.Query.Direction == OrderDirection.Ascending ? records.OrderBy(itm => itm.RecordTime).Take(input.Query.LimitResult) : records.OrderByDescending(itm => itm.RecordTime).Take(input.Query.LimitResult) : null;
            }
            protected override Vanrise.Entities.BigResult<DataRecordDetail> AllRecordsToBigResult(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input, IEnumerable<DataRecord> allRecords)
            {
                if (allRecords == null)
                    return new Vanrise.Entities.BigResult<DataRecordDetail>()
                {
                    ResultKey = input.ResultKey,
                    Data = null,
                    TotalCount = 0
                };


                IEnumerable<DataRecordDetail> allRecordsDetails = DataRecordDetailMapperList(allRecords);

                IOrderedEnumerable<DataRecordDetail> orderedRecords = allRecordsDetails.VROrderList(input);

                if (input.Query.SortColumns != null && input.Query.SortColumns.Count > 0)
                    orderedRecords = GetOrderedByFields(input.Query.SortColumns, orderedRecords);

                IEnumerable<DataRecordDetail> pagedRecords = orderedRecords.VRGetPage(input);

                var dataRecordBigResult = new Vanrise.Entities.BigResult<DataRecordDetail>()
                {
                    ResultKey = input.ResultKey,
                    Data = pagedRecords,
                    TotalCount = allRecordsDetails.Count()
                };

                return dataRecordBigResult;
            }

            private IOrderedEnumerable<DataRecordDetail> GetOrderedByFields(List<SortColumn> sortColumns, IOrderedEnumerable<DataRecordDetail> orderedRecords)
            {
                foreach (SortColumn sortColumn in sortColumns)
                {
                    DataRecordField field = RecordType.Fields.FirstOrDefault(itm => itm.Name == sortColumn.FieldName);
                    if (field == null)
                        continue;

                    switch (field.Type.OrderType)
                    {
                        case DataRecordFieldOrderType.ByFieldDescription: orderedRecords = sortColumn.IsDescending ? orderedRecords.ThenByDescending(itm => itm.FieldValues[sortColumn.FieldName].Description) : orderedRecords.ThenBy(itm => itm.FieldValues[sortColumn.FieldName].Description); break;
                        case DataRecordFieldOrderType.ByFieldValue: orderedRecords = sortColumn.IsDescending ? orderedRecords.ThenByDescending(itm => itm.FieldValues[sortColumn.FieldName].Value) : orderedRecords.ThenBy(itm => itm.FieldValues[sortColumn.FieldName].Value); break;
                        default: break;
                    }
                }
                return orderedRecords;
            }

            private List<DataRecord> GetDataRecords(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input, Guid dataRecordStorageId)
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

            protected override ResultProcessingHandler<DataRecordDetail> GetResultProcessingHandler(DataRetrievalInput<DataRecordQuery> input, BigResult<DataRecordDetail> bigResult)
            {
                return new ResultProcessingHandler<DataRecordDetail>
                {
                    ExportExcelHandler = new DataRecordStorageExcelExportHandler(input.Query)
                };
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
            DataStore dataStore = _dataStoreManager.GetDataStore(dataRecordStorage.DataStoreId);

            return new DataRecordStorageInfo()
            {
                DataRecordStorageId = dataRecordStorage.DataRecordStorageId,
                Name = dataRecordStorage.Name,
                DataRecordTypeId = dataRecordStorage.DataRecordTypeId,
                IsRemoteRecordStorage = dataStore != null && dataStore.Settings != null ? dataStore.Settings.IsRemoteDataStore : false
            };
        }

        #endregion

        private class DataRecordStorageExcelExportHandler : ExcelExportHandler<DataRecordDetail>
        {
            DataRecordQuery _query;
            public DataRecordStorageExcelExportHandler(DataRecordQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }

            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<DataRecordDetail> context)
            {
                if (context.BigResult == null)
                    throw new ArgumentNullException("context.BigResult");
                if (context.BigResult.Data == null)
                    throw new ArgumentNullException("context.BigResult.Data");
                ExportExcelSheet sheet = new ExportExcelSheet();
                sheet.Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() };

                if (_query.Columns != null)
                {
                    foreach (var dimName in _query.Columns)
                    {
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = dimName });
                    }
                }
                sheet.Rows = new List<ExportExcelRow>();
                foreach (var record in context.BigResult.Data)
                {
                    var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                    sheet.Rows.Add(row);
                    if (record.FieldValues != null)
                    {
                        foreach (var dimValue in _query.Columns)
                        {
                            DataRecordFieldValue dataRecordFieldValue;
                            if (!record.FieldValues.TryGetValue(dimValue, out dataRecordFieldValue))
                                throw new NullReferenceException(String.Format("dataRecordFieldValue. dimValue '{0}'", dataRecordFieldValue.Value));
                            row.Cells.Add(new ExportExcelCell { Value = dataRecordFieldValue.Description });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
    }
}
