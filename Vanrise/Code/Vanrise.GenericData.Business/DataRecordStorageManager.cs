using System;
using System.Collections.Generic;
using System.Linq;
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
            input.Query.DataRecordStorageIds.ThrowIfNull("input.Query.DataRecordStorageIds");

            var dataRecordStorageId = input.Query.DataRecordStorageIds.First();
            var dataRecordStorage = GetDataRecordStorage(dataRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);

            DataStore dataStore = new DataStoreManager().GetDataStore(dataRecordStorage.DataStoreId);
            dataStore.ThrowIfNull("dataStore", dataRecordStorage.DataStoreId);
            dataStore.Settings.ThrowIfNull("dataStore.Settings", dataRecordStorage.DataStoreId);

            var remoteRecordDataManager = dataStore.Settings.GetRemoteRecordDataManager(new GetRemoteRecordStorageDataManagerContext() { DataStore = dataStore, DataRecordStorage = dataRecordStorage });
            if (remoteRecordDataManager != null)
            {
                return remoteRecordDataManager.GetFilteredDataRecords(input);
            }
            else
            {
                if (input.Query.Filters != null)
                    input.Query.FilterGroup = new RecordFilterManager().BuildRecordFilterGroup(dataRecordStorage.DataRecordTypeId, input.Query.Filters, input.Query.FilterGroup);

                input.Query.Columns.ThrowIfNull("input.Query.Columns");
                input.Query.Columns = new HashSet<string>(input.Query.Columns).ToList();

                input.Query.ColumnTitles.ThrowIfNull("input.Query.ColumnTitles");
                input.Query.ColumnTitles = new HashSet<string>(input.Query.ColumnTitles).ToList();

                DataRecordType recordType = new DataRecordTypeManager().GetDataRecordType(dataRecordStorage.DataRecordTypeId);
                recordType.ThrowIfNull("recordType", dataRecordStorage.DataRecordTypeId);
                recordType.Fields.ThrowIfNull("recordType.Fields", dataRecordStorage.DataRecordTypeId);

                if (input.Query.FilterGroup != null)
                {
                    var filterGroup = ConvertFilterGroup(input.Query.FilterGroup, recordType);
                    input.Query.FilterGroup = filterGroup;
                }

                if (!string.IsNullOrEmpty(input.SortByColumnName) && input.SortByColumnName.Contains("FieldValues"))
                {
                    string[] fieldValueproperty = input.SortByColumnName.Split('.');
                    input.SortByColumnName = string.Format(@"{0}[""{1}""].{2}", fieldValueproperty[0], fieldValueproperty[1], fieldValueproperty[2]);
                }

                return BigDataManager.Instance.RetrieveData(input, new DataRecordRequestHandler() { DataRecordTypeId = dataRecordStorage.DataRecordTypeId });
            }
        }

        private RecordFilterGroup ConvertFilterGroup(RecordFilterGroup filterGroup, DataRecordType recordType)
        {
            List<RecordFilter> convertedFilters = new List<RecordFilter>();

            foreach (var filter in filterGroup.Filters)
            {
                ConvertChildFilterGroup(filter, recordType, convertedFilters);
            }

            if (convertedFilters.Count > 0)
            {
                RecordFilterGroup recordFilterGroup = new RecordFilterGroup() { };
                recordFilterGroup.LogicalOperator = filterGroup.LogicalOperator;
                recordFilterGroup.Filters = convertedFilters;

                return recordFilterGroup;
            }

            return null;
        }
        public void ConvertChildFilterGroup(RecordFilter recordFilter, DataRecordType recordType, List<RecordFilter> convertedFilters)
        {
            RecordFilterGroup childFilterGroup = recordFilter as RecordFilterGroup;
            if (childFilterGroup != null)
            {
                List<RecordFilter> convertedChildFilters = new List<RecordFilter>();

                foreach (var filter in childFilterGroup.Filters)
                {
                    ConvertChildFilterGroup(filter, recordType, convertedChildFilters);
                }

                if (convertedChildFilters.Count > 0)
                {
                    RecordFilterGroup childRecordFilterGroup = new RecordFilterGroup();
                    childRecordFilterGroup.LogicalOperator = childFilterGroup.LogicalOperator;
                    childRecordFilterGroup.Filters = convertedChildFilters;

                    convertedFilters.Add(childRecordFilterGroup);
                }
            }
            else
            {
                if (recordFilter.FieldName != null)
                {
                    var record = recordType.Fields.FindRecord(x => x.Name == recordFilter.FieldName);
                    if (record != null)
                    {
                        if (record.Formula != null)
                        {
                            var context = new DataRecordFieldFormulaConvertFilterContext(recordType.DataRecordTypeId, recordFilter.FieldName);
                            context.InitialFilter = recordFilter;

                            var recordFilterObj = record.Formula.ConvertFilter(context);
                            if (recordFilterObj != null)
                                ConvertChildFilterGroup(recordFilterObj, recordType, convertedFilters);
                        }
                        else
                        {
                            //recordFilter.FieldName = record.Name; 
                            convertedFilters.Add(recordFilter);
                        }
                    }
                }
                else //used in case of filter without FieldName like AlwaysFalseRecordFilter
                {
                    convertedFilters.Add(recordFilter);
                }
            }
        }

        public IDataRecordDataManager GetStorageDataManager(Guid recordStorageId, TempStorageInformation tempStorageInformation = null)
        {
            var dataRecordStorage = GetDataRecordStorage(recordStorageId);
            if (dataRecordStorage == null)
                throw new NullReferenceException(String.Format("dataRecordStorage. Id '{0}'", recordStorageId));
            if (dataRecordStorage.Settings == null)
                throw new NullReferenceException(String.Format("dataRecordStorage.Settings. Id '{0}'", recordStorageId));

            return GetStorageDataManager(dataRecordStorage, tempStorageInformation);
        }

        public Vanrise.Entities.IDataRetrievalResult<DataRecordStorageDetail> GetFilteredDataRecordStorages(Vanrise.Entities.DataRetrievalInput<DataRecordStorageQuery> input)
        {
            var cachedDataRecordStorages = GetCachedDataRecordStorages();

            Func<DataRecordStorage, bool> filterExpression = (dataRecordStorage) =>
                (input.Query.Name == null || dataRecordStorage.Name.ToUpper().Contains(input.Query.Name.ToUpper()))
                && (input.Query.DataRecordTypeIds == null || input.Query.DataRecordTypeIds.Contains(dataRecordStorage.DataRecordTypeId))
                && (input.Query.DataStoreIds == null || input.Query.DataStoreIds.Contains(dataRecordStorage.DataStoreId));
            VRActionLogger.Current.LogGetFilteredAction(DataRecordStorageLoggableEntity.Instance, input);
            return DataRetrievalManager.Instance.ProcessResult(input, cachedDataRecordStorages.ToBigResult(input, filterExpression, DataRecordStorageMapper));
        }

        public string GetDataRecordStorageName(DataRecordStorage dataRecordStorage)
        {
            if (dataRecordStorage != null)
                return dataRecordStorage.Name;
            return null;
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
            var dataRecordStorageItem = cachedDataRecordStorages.FindRecord(dataRecordStorage => dataRecordStorage.DataRecordStorageId == dataRecordStorageId);

            return dataRecordStorageItem;
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
                VRActionLogger.Current.TrackAndLogObjectAdded(DataRecordStorageLoggableEntity.Instance, dataRecordStorage);
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
                VRActionLogger.Current.TrackAndLogObjectUpdated(DataRecordStorageLoggableEntity.Instance, dataRecordStorage);
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

        public object CreateTempStorage(Guid dataRecordStorageId, long processId)
        {
            DataRecordStorage dataRecordStorage;
            DataStore dataStore;
            GetDataRecordData(dataRecordStorageId, out dataRecordStorage, out dataStore);

            CreateTempStorageContext createTempStorageContext = new CreateTempStorageContext()
            {
                DataStore = dataStore,
                DataRecordStorage = dataRecordStorage,
                ProcessId = processId
            };
            dataStore.Settings.CreateTempStorage(createTempStorageContext);

            return createTempStorageContext.TempStorageInformation;
        }

        public void FillDataRecordStorageFromTempStorage(Guid dataRecordStorageId, TempStorageInformation tempStorageInformation, DateTime from, DateTime to)
        {
            DataRecordStorage dataRecordStorage;
            DataStore dataStore;
            GetDataRecordData(dataRecordStorageId, out dataRecordStorage, out dataStore);

            FillDataRecordStorageFromTempStorageContext fillDataRecordStorageFromTempStorage = new FillDataRecordStorageFromTempStorageContext()
            {
                DataStore = dataStore,
                DataRecordStorage = dataRecordStorage,
                TempStorageInformation = tempStorageInformation,
                From = from,
                To = to
            };
            dataStore.Settings.FillDataRecordStorageFromTempStorage(fillDataRecordStorageFromTempStorage);
        }

        public void DropStorage(Guid dataRecordStorageId, TempStorageInformation tempStorageInformation = null)
        {
            DataRecordStorage dataRecordStorage;
            DataStore dataStore;
            GetDataRecordData(dataRecordStorageId, out dataRecordStorage, out dataStore);

            DropStorageContext dropTempStorageContext = new DropStorageContext()
            {
                DataStore = dataStore,
                DataRecordStorage = dataRecordStorage,
                TempStorageInformation = tempStorageInformation
            };
            dataStore.Settings.DropStorage(dropTempStorageContext);
        }

        public int GetStorageRowCount(Guid dataRecordStorageId, TempStorageInformation tempStorageInformation = null)
        {
            DataRecordStorage dataRecordStorage;
            DataStore dataStore;
            GetDataRecordData(dataRecordStorageId, out dataRecordStorage, out dataStore);

            GetStorageRowCountContext getStorageRowCountContext = new GetStorageRowCountContext()
            {
                DataStore = dataStore,
                DataRecordStorage = dataRecordStorage,
                TempStorageInformation = tempStorageInformation
            };
            return dataStore.Settings.GetStorageRowCount(getStorageRowCountContext);
        }
        #endregion

        #region Private Methods
        private void GetDataRecordData(Guid dataRecordStorageId, out DataRecordStorage dataRecordStorage, out DataStore dataStore)
        {
            dataRecordStorage = GetDataRecordStorage(dataRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);

            dataStore = new DataStoreManager().GetDataStore(dataRecordStorage.DataStoreId);
            dataStore.ThrowIfNull("dataStore", dataRecordStorage.DataStoreId);
            dataStore.Settings.ThrowIfNull("dataStore.Settings", dataRecordStorage.DataStoreId);
        }

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

        IDataRecordDataManager GetStorageDataManager(DataRecordStorage dataRecordStorage, TempStorageInformation tempStorageInformation = null)
        {
            var dataStore = _dataStoreManager.GetDataStore(dataRecordStorage.DataStoreId);
            if (dataStore == null)
                throw new NullReferenceException(String.Format("dataStore. dataStore Id '{0}' dataRecordStorage Id '{1}'", dataRecordStorage.DataStoreId, dataRecordStorage.DataRecordStorageId));
            if (dataStore.Settings == null)
                throw new NullReferenceException(String.Format("dataStore.Settings. dataStore Id '{0}' dataRecordStorage Id '{1}'", dataRecordStorage.DataStoreId, dataRecordStorage.DataRecordStorageId));

            var getRecordStorageDataManagerContext = new GetRecordStorageDataManagerContext { DataStore = dataStore, DataRecordStorage = dataRecordStorage, TempStorageInformation = tempStorageInformation };
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

        private class DataRecordStorageLoggableEntity : VRLoggableEntityBase
        {
            public static DataRecordStorageLoggableEntity Instance = new DataRecordStorageLoggableEntity();

            private DataRecordStorageLoggableEntity()
            {

            }

            static DataRecordStorageManager s_dataRecordStororageManager = new DataRecordStorageManager();

            public override string EntityUniqueName
            {
                get { return "VR_GenericData_DataRecordStorage"; }
            }

            public override string ModuleName
            {
                get { return "Generic Data"; }
            }

            public override string EntityDisplayName
            {
                get { return "Data Record Storage"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_GenericData_DataRecordStorage_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                DataRecordStorage dataRecordStorage = context.Object.CastWithValidate<DataRecordStorage>("context.Object");
                return dataRecordStorage.DataRecordStorageId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                DataRecordStorage dataRecordStorage = context.Object.CastWithValidate<DataRecordStorage>("context.Object");
                return s_dataRecordStororageManager.GetDataRecordStorageName(dataRecordStorage);
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

            private Dictionary<string, DataRecordField> _dataRecordFieldDict;
            private Dictionary<string, DataRecordField> DataRecordFieldDict
            {
                get
                {
                    if (_dataRecordFieldDict == null)
                    {
                        _dataRecordFieldDict = RecordType.Fields.ToDictionary(itm => itm.Name, itm => itm);
                    }
                    return _dataRecordFieldDict;
                }
            }

            private Dictionary<string, DataRecordFieldType> _dataRecordFieldTypeDict;
            private Dictionary<string, DataRecordFieldType> DataRecordFieldTypeDict
            {
                get
                {
                    if (_dataRecordFieldTypeDict == null)
                    {
                        _dataRecordFieldTypeDict = RecordType.Fields.ToDictionary(itm => itm.Name, itm => itm.Type);
                    }
                    return _dataRecordFieldTypeDict;
                }
            }

            #region Public and Protected Methods

            public override IEnumerable<DataRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input)
            {
                Vanrise.Entities.DataRetrievalInput<DataRecordQuery> clonedInput = Vanrise.Common.Utilities.CloneObject(input);
                List<DataRecord> records = new List<DataRecord>();

                HashSet<string> dependentDataRecordStorageFields = new HashSet<string>();
                Dictionary<string, List<string>> formulaFieldDirectDependencies = new Dictionary<string, List<string>>();

                Dictionary<string, DataRecordField> formulaDataRecordFieldsDict = GetFormulaDataRecordFields(RecordType, clonedInput);
                if (formulaDataRecordFieldsDict != null)
                {
                    Dictionary<string, DataRecordField> clonedFormulaDataRecordFieldsDict = Vanrise.Common.Utilities.CloneObject(formulaDataRecordFieldsDict);
                    foreach (var formulaField in clonedFormulaDataRecordFieldsDict.Values)
                    {
                        var currentDependentFields = formulaField.Formula.GetDependentFields(new DataRecordFieldFormulaGetDependentFieldsContext());
                        dependentDataRecordStorageFields.UnionWith(GetDependentDataRecordStorageFieldNames(currentDependentFields, formulaDataRecordFieldsDict, formulaFieldDirectDependencies));

                        if (!formulaFieldDirectDependencies.ContainsKey(formulaField.Name))
                            formulaFieldDirectDependencies.Add(formulaField.Name, currentDependentFields);
                    }
                }

                if (clonedInput.Query.Columns != null && clonedInput.Query.Columns.Count > 0)
                {
                    clonedInput.Query.Columns.RemoveAll(itm => DataRecordFieldDict.GetRecord(itm).Formula != null);

                    if (dependentDataRecordStorageFields.Count > 0)
                    {
                        clonedInput.Query.Columns.AddRange(dependentDataRecordStorageFields);
                        clonedInput.Query.Columns = clonedInput.Query.Columns.Distinct().ToList();
                    }
                }

                foreach (Guid dataRecordStorageId in input.Query.DataRecordStorageIds)
                {
                    var result = GetDataRecords(clonedInput, dataRecordStorageId);
                    if (result != null)
                        records.AddRange(result);
                }

                if (records.Count == 0)
                    return null;

                List<DataRecord> orderedDataRecordResults = GetTopOrderedResults(records, input);

                List<string> retrievedDataRecordFields = new List<string>(clonedInput.Query.Columns);
                Queue<string> formulaDataRecordFieldNames = GetFormulaDataRecordFieldNames(formulaFieldDirectDependencies, retrievedDataRecordFields);

                if (formulaDataRecordFieldNames != null && formulaDataRecordFieldNames.Count > 0)
                {
                    DataRecordField tempDataRecordField;

                    foreach (var dataRecordResult in orderedDataRecordResults)
                    {
                        foreach (string formulaFieldName in formulaDataRecordFieldNames)
                        {
                            if (formulaDataRecordFieldsDict.TryGetValue(formulaFieldName, out tempDataRecordField) && !dataRecordResult.FieldValues.ContainsKey(formulaFieldName))
                            {
                                Object value = tempDataRecordField.Formula.CalculateValue(new DataRecordFieldFormulaCalculateValueContext(DataRecordFieldTypeDict, dataRecordResult.FieldValues, tempDataRecordField.Type));
                                dataRecordResult.FieldValues.Add(formulaFieldName, value);
                            }
                        }
                    }
                }

                return orderedDataRecordResults;
            }

            public override DataRecordDetail EntityDetailMapper(DataRecord entity)
            {
                var dataRecordDetail = new DataRecordDetail() { RecordTime = entity.RecordTime, FieldValues = new Dictionary<string, DataRecordFieldValue>() };
                foreach (var fld in RecordType.Fields)
                {
                    Object value;
                    DataRecordFieldValue fldValueDetail = new DataRecordFieldValue();

                    if (entity.FieldValues.TryGetValue(fld.Name, out value))
                    {
                        fldValueDetail.Value = value;
                        fldValueDetail.Description = fld.Type.GetDescription(value);
                        dataRecordDetail.FieldValues.Add(fld.Name, fldValueDetail);
                    }
                }
                return dataRecordDetail;
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

                IOrderedEnumerable<DataRecordDetail> orderedRecords;
                if (!string.IsNullOrEmpty(input.SortByColumnName))
                    orderedRecords = allRecordsDetails.VROrderList(input);
                else
                    orderedRecords = input.Query.Direction == OrderDirection.Ascending ? allRecordsDetails.OrderBy(itm => itm.RecordTime) : allRecordsDetails.OrderByDescending(itm => itm.RecordTime);

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

            protected override ResultProcessingHandler<DataRecordDetail> GetResultProcessingHandler(DataRetrievalInput<DataRecordQuery> input, BigResult<DataRecordDetail> bigResult)
            {
                return new ResultProcessingHandler<DataRecordDetail>
                {
                    ExportExcelHandler = new DataRecordStorageExcelExportHandler(input.Query)
                };
            }

            #endregion

            #region Private Methods

            private Dictionary<string, DataRecordField> GetFormulaDataRecordFields(DataRecordType recordType, DataRetrievalInput<DataRecordQuery> input)
            {
                List<DataRecordField> formulaDataRecordFields = RecordType.Fields.FindAll(itm => itm.Formula != null && input.Query.Columns.Contains(itm.Name));
                return (formulaDataRecordFields != null && formulaDataRecordFields.Count > 0) ? formulaDataRecordFields.ToDictionary(itm => itm.Name, itm => itm) : null;
            }

            private HashSet<string> GetDependentDataRecordStorageFieldNames(List<string> directDependentFields, Dictionary<string, DataRecordField> formulaDataRecordFieldsDict, Dictionary<string, List<string>> formulaFieldDirectDependencies)
            {
                HashSet<string> results = new HashSet<string>();

                foreach (var directDependentField in directDependentFields)
                {
                    DataRecordField currentDataRecordField;
                    if (DataRecordFieldDict.TryGetValue(directDependentField, out currentDataRecordField))
                    {
                        if (currentDataRecordField.Formula == null)
                        {
                            results.Add(directDependentField);
                        }
                        else
                        {
                            var currentFieldDirectDependentFields = currentDataRecordField.Formula.GetDependentFields(new DataRecordFieldFormulaGetDependentFieldsContext());

                            if (!formulaFieldDirectDependencies.ContainsKey(directDependentField))
                                formulaFieldDirectDependencies.Add(directDependentField, currentFieldDirectDependentFields);

                            if (!formulaDataRecordFieldsDict.ContainsKey(directDependentField))
                                formulaDataRecordFieldsDict.Add(directDependentField, RecordType.Fields.FindRecord(itm => string.Compare(itm.Name, directDependentField, true) == 0));

                            results.UnionWith(GetDependentDataRecordStorageFieldNames(currentFieldDirectDependentFields, formulaDataRecordFieldsDict, formulaFieldDirectDependencies));
                        }
                    }
                }

                return results;
            }

            private List<DataRecord> GetTopOrderedResults(List<DataRecord> records, DataRetrievalInput<DataRecordQuery> input)
            {
                if (records.Count > 0)
                {
                    if (input.Query.Direction == OrderDirection.Ascending)
                        return records.OrderBy(itm => itm.RecordTime).Take(input.Query.LimitResult).ToList();
                    else
                        return records.OrderByDescending(itm => itm.RecordTime).Take(input.Query.LimitResult).ToList();
                }
                else
                {
                    return null;
                }
            }

            private Queue<string> GetFormulaDataRecordFieldNames(Dictionary<string, List<string>> dependentFieldsByFieldName, List<string> retrievedDataRecordFields)
            {
                Queue<string> formulaDataRecordFieldNames = GetFormulaDataRecordFieldNamesQueue(dependentFieldsByFieldName, retrievedDataRecordFields);
                return formulaDataRecordFieldNames.Count > 0 ? formulaDataRecordFieldNames : null;
            }

            private Queue<string> GetFormulaDataRecordFieldNamesQueue(Dictionary<string, List<string>> remainingDependentFieldsByFieldName, List<string> availableDataRecordFieldsValue, Queue<string> formulaDataRecordFieldNames = null)
            {
                if (formulaDataRecordFieldNames == null)
                    formulaDataRecordFieldNames = new Queue<string>();

                if (remainingDependentFieldsByFieldName.Count == 0)
                    return formulaDataRecordFieldNames;

                Dictionary<string, List<string>> clonedDependentFieldsByFieldName = Vanrise.Common.Utilities.CloneObject(remainingDependentFieldsByFieldName);

                foreach (var dependentFieldsKvp in remainingDependentFieldsByFieldName)
                {
                    List<string> dependentFieldNames = dependentFieldsKvp.Value;
                    if (dependentFieldNames.All(itm => availableDataRecordFieldsValue.Contains(itm)))
                    {
                        formulaDataRecordFieldNames.Enqueue(dependentFieldsKvp.Key);
                        availableDataRecordFieldsValue.Add(dependentFieldsKvp.Key);
                        clonedDependentFieldsByFieldName.Remove(dependentFieldsKvp.Key);
                    }
                }

                return GetFormulaDataRecordFieldNamesQueue(clonedDependentFieldsByFieldName, availableDataRecordFieldsValue, formulaDataRecordFieldNames);
            }

            private List<DataRecord> GetDataRecords(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input, Guid dataRecordStorageId)
            {
                DataRecordStorageManager manager = new DataRecordStorageManager();
                var dataRecordStorage = manager.GetDataRecordStorage(dataRecordStorageId);
                dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);
                dataRecordStorage.Settings.ThrowIfNull("dataRecordStorage.Settings", dataRecordStorageId);

                var dataManager = manager.GetStorageDataManager(dataRecordStorage);
                dataManager.ThrowIfNull("dataManager", dataRecordStorageId);

                return dataManager.GetFilteredDataRecords(input);
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

            #endregion
        }

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
                if (_query.Columns == null || _query.ColumnTitles == null || _query.Columns.Count != _query.ColumnTitles.Count)
                {
                    throw new ArgumentNullException("Count of columns titles different then count of columns");
                }

                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                foreach (var dimName in _query.ColumnTitles)
                {
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = dimName });
                }
                
                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
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
                                    throw new NullReferenceException(
                                        String.Format("dataRecordFieldValue. dimValue '{0}'", dataRecordFieldValue.Value));
                                row.Cells.Add(new ExportExcelCell { Value = dataRecordFieldValue.Description });
                            }
                        }
                    }
                }
                context.MainSheet = sheet;
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
    }
}
