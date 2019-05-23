﻿using System;
using System;
using System.Collections.Concurrent;
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
        DataRecordTypeManager _recordTypeManager = new DataRecordTypeManager();

        #endregion

        #region Public Methods Data Record
        public List<DataRecord> GetDataRecords(DateTime fromTime, DateTime toTime, RecordFilterGroup filterGroup, List<string> fieldNames, int? limitResult, OrderDirection direction, Guid dataRecordStorageId)
        {
            var storageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorageId);
            storageDataManager.ThrowIfNull("storageDataManager", dataRecordStorageId);
            var context = new DataRecordDataManagerGetFilteredDataRecordsContext
            {
                FromTime = fromTime,
                ToTime = toTime,
                FilterGroup = MergeRecordFilterGroup(dataRecordStorageId, filterGroup),
                LimitResult = limitResult,
                FieldNames = fieldNames,
                Direction = direction
            };
            return storageDataManager.GetFilteredDataRecords(context);
        }

        public DataRecord GetDataRecord(Guid dataRecordStorageId, Object dataRecordId, List<string> fieldNames)
        {
            var dataRecordStorage = GetDataRecordStorage(dataRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);

            var recordTypeIdField = _recordTypeManager.GetDataRecordTypeIdField(dataRecordStorage.DataRecordTypeId);
            recordTypeIdField.ThrowIfNull("recordTypeIdField", dataRecordStorage.DataRecordTypeId);
            var parsedDataRecordId = recordTypeIdField.Type.ParseValueToFieldType(new DataRecordFieldTypeParseValueToFieldTypeContext(dataRecordId));



            if (dataRecordStorage.Settings.EnableUseCaching)
            {
                return GetCachedDataRecordsDict(dataRecordStorageId).GetRecord((Object)parsedDataRecordId);
            }
            else
            {
                fieldNames.ThrowIfNull("fieldNames");
                fieldNames = new HashSet<string>(fieldNames).ToList();

                var dataRecordFieldDict = new DataRecordTypeManager().GetDataRecordTypeFields(dataRecordStorage.DataRecordTypeId);
                dataRecordFieldDict.ThrowIfNull("dataRecordFieldDict", dataRecordStorage.DataRecordTypeId);

                var storageDataManager = GetStorageDataManager(dataRecordStorageId);
                storageDataManager.ThrowIfNull("storageDataManager", dataRecordStorageId);


                Dictionary<string, DataRecordField> formulaDataRecordFieldsDict;
                PrepareDependentAndFormulaFields(fieldNames, dataRecordFieldDict, out formulaDataRecordFieldsDict);

                var dataRecord = storageDataManager.GetDataRecord(parsedDataRecordId, fieldNames);

                FillDataRecordFormulaFields(dataRecordStorage.DataRecordTypeId, dataRecord, formulaDataRecordFieldsDict);
                return dataRecord;
            }
        }

        public Vanrise.Entities.IDataRetrievalResult<DataRecordDetail> GetFilteredDataRecords(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> dataRetrievalInput)
        {
            var input = dataRetrievalInput.VRDeepCopy();
            if (input.Query.SortColumns != null && input.Query.SortColumns.Count > 0)
            {
                if (input.Query.Columns == null)
                    input.Query.Columns = new List<string>();
                foreach (var sortColumn in input.Query.SortColumns)
                {
                    if (!input.Query.Columns.Contains(sortColumn.FieldName))
                    {
                        input.Query.Columns.Add(sortColumn.FieldName);
                    }
                }
            }
            input.Query.DataRecordStorageIds.ThrowIfNull("input.Query.DataRecordStorageIds");

            var dataRecordStorageId = input.Query.DataRecordStorageIds.First();
            var dataRecordStorage = GetDataRecordStorage(dataRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);

            DataStore dataStore = new DataStoreManager().GetDataStore(dataRecordStorage.DataStoreId);
            dataStore.ThrowIfNull("dataStore", dataRecordStorage.DataStoreId);
            dataStore.Settings.ThrowIfNull("dataStore.Settings", dataRecordStorage.DataStoreId);

            if (dataRecordStorage.Settings.RequiredLimitResult && !input.Query.LimitResult.HasValue)
                throw new Exception("Limit result should not be null.");

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

                input.Query.FilterGroup = MergeRecordFilterGroup(dataRecordStorageId, input.Query.FilterGroup);

                if (!string.IsNullOrEmpty(input.SortByColumnName) && input.SortByColumnName.Contains("FieldValues"))
                {
                    string[] fieldValueproperty = input.SortByColumnName.Split('.');
                    input.SortByColumnName = string.Format(@"{0}[""{1}""].{2}", fieldValueproperty[0], fieldValueproperty[1], fieldValueproperty[2]);
                }

                if (dataRecordStorage.Settings.EnableUseCaching)
                {
                    if (input.Query.DataRecordStorageIds.Count > 1)
                        throw new Exception("Only one DataRecordStorage is supported in Cache Mode");
                    var allDataRecords = GetCachedDataRecords(dataRecordStorage.DataRecordStorageId);
                    var dataRecords = GetTopOrderedResults(allDataRecords, input.Query.Direction, input.Query.LimitResult);
                    RecordFilterManager recordFilterManager = new RecordFilterManager();

                    Func<DataRecord, bool> filterExpression = (dataRecord) =>
                    {
                        if (input.Query.FilterGroup != null)
                        {
                            var context = new DataRecordDictFilterGenericFieldMatchContext(dataRecord.FieldValues, recordType.DataRecordTypeId);
                            if (!recordFilterManager.IsFilterGroupMatch(input.Query.FilterGroup, context))
                                return false;

                        }
                        return true;
                    };

                    var dataRecordBigResult = dataRecords.FindAllRecords(filterExpression);
                    var handler = new ResultProcessingHandler<DataRecordDetail>()
                    {
                        ExportExcelHandler = new DataRecordStorageExcelExportHandler(input.Query)
                    };

                    var cachedAccountsWithSelectionHandling = dataRecordBigResult;
                    if (input.Query.BulkActionState != null)
                    {
                        string resultKey = input.ResultKey;
                        VRBulkActionDraftManager bulkActionDraftManager = new VRBulkActionDraftManager();
                        cachedAccountsWithSelectionHandling = bulkActionDraftManager.GetOrCreateCachedWithSelectionHandling<DataRecord, DataRecordStorageManager.RecordCacheManager, Guid>(ref resultKey, input.Query.BulkActionState, () =>
                        {
                            return dataRecordBigResult;
                        }, (records) =>
                        {

                            List<BulkActionItem> bulkActionItems = new List<BulkActionItem>();
                            var idField = _recordTypeManager.GetDataRecordTypeIdField(dataRecordStorage.DataRecordTypeId);
                            foreach (var record in records)
                            {
                                bulkActionItems.Add(new BulkActionItem
                                {
                                    ItemId = record.FieldValues.GetRecord(idField.Name).ToString()
                                });
                            }
                            return bulkActionItems;

                        }, dataRecordStorageId);
                        input.ResultKey = resultKey;
                    }
                    var orderType = input.Query.OrderType;
                    if (orderType.HasValue)
                        cachedAccountsWithSelectionHandling = GetOrderedDataRecords(orderType.Value, input.Query.Columns, input.Query.AdvancedOrderOptions, cachedAccountsWithSelectionHandling);
                    return DataRetrievalManager.Instance.ProcessResult(input, AllRecordsToBigResult(input, cachedAccountsWithSelectionHandling, recordType), handler);

                }
                else
                {

                    return BigDataManager.Instance.RetrieveData(input, new DataRecordRequestHandler() { DataRecordTypeId = recordType.DataRecordTypeId });
                }
            }
        }

        public void GetDataRecords(Guid dataRecordStorageId, DateTime? from, DateTime? to, RecordFilterGroup recordFilterGroup, Func<bool> shouldStop, Action<dynamic> onItemReady, string orderColumnName = null, bool isOrderAscending = false)
        {
            var storageDataManager = GetStorageDataManager(dataRecordStorageId);
            storageDataManager.ThrowIfNull("storageDataManager", dataRecordStorageId);
            storageDataManager.GetDataRecords(from, to, MergeRecordFilterGroup(dataRecordStorageId, recordFilterGroup), shouldStop, onItemReady, orderColumnName, isOrderAscending);
        }

        public IEnumerable<DataRecord> GetAllDataRecords(Guid dataRecordStorageId, List<string> columnsNeeded = null, RecordFilterGroup filterGroup = null, List<DataRecordFilter> filters = null)
        {
            var dataRecordStorage = GetDataRecordStorage(dataRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);

            DataRecordType recordType = new DataRecordTypeManager().GetDataRecordType(dataRecordStorage.DataRecordTypeId);
            recordType.ThrowIfNull("recordType", dataRecordStorage.DataRecordTypeId);
            recordType.Fields.ThrowIfNull("recordType.Fields", dataRecordStorage.DataRecordTypeId);
            RecordFilterManager recordFilterManager = new RecordFilterManager();

            RecordFilterGroup filterGroupObj = null;
            if (filterGroup != null || filters != null)
            {
                var mergedFilterGroup = recordFilterManager.BuildRecordFilterGroup(recordType.DataRecordTypeId, filters, filterGroup);
                filterGroupObj = ConvertFilterGroup(mergedFilterGroup, recordType);
            }
            filterGroupObj = MergeRecordFilterGroup(dataRecordStorageId, filterGroupObj);
            Func<DataRecord, bool> filterExpression = (dataRecord) =>
            {
                if (filterGroupObj != null)
                {
                    var context = new DataRecordDictFilterGenericFieldMatchContext(dataRecord.FieldValues, recordType.DataRecordTypeId);
                    if (!recordFilterManager.IsFilterGroupMatch(filterGroupObj, context))
                        return false;

                }
                return true;
            };

            if (!dataRecordStorage.Settings.EnableUseCaching)
            {
                return GetDataRecords(dataRecordStorageId, columnsNeeded).FindAllRecords(filterExpression);
            }
            //     throw new NotSupportedException("This method only available for cached record storages");
            return GetCachedDataRecords(dataRecordStorageId).FindAllRecords(filterExpression);
        }

        public IEnumerable<DataRecord> GetDataRecordsFinalResult(DataRecordType recordType, DataRetrievalInput<DataRecordQuery> input, Func<Guid, DataRetrievalInput<DataRecordQuery>, IEnumerable<DataRecord>> retrieveDataRecordFunction)
        {
            Vanrise.Entities.DataRetrievalInput<DataRecordQuery> clonedInput = Vanrise.Common.Utilities.CloneObject(input);

            var dataRecordFieldDict = recordType.Fields.ToDictionary(itm => itm.Name, itm => itm);
            Dictionary<string, DataRecordField> formulaDataRecordFieldsDict;
            PrepareDependentAndFormulaFields(clonedInput.Query.Columns, dataRecordFieldDict, out formulaDataRecordFieldsDict);

            List<DataRecord> records = new List<DataRecord>();

            foreach (Guid item in input.Query.DataRecordStorageIds)
            {
                var result = retrieveDataRecordFunction(item, clonedInput);
                if (result != null)
                    records.AddRange(result);
            }
            return GetOrderedDataRecordResults(recordType.DataRecordTypeId, clonedInput.Query.Direction, input.Query.LimitResult, input.Query.Columns, records, formulaDataRecordFieldsDict);
        }

        private void FillDataRecordFormulaFields(Guid dataRecordTypeId, DataRecord record, Dictionary<string, DataRecordField> formulaDataRecordFieldsDict)
        {
            if (formulaDataRecordFieldsDict != null && formulaDataRecordFieldsDict.Count > 0)
            {
                var dataRecordObject = new DataRecordObject(dataRecordTypeId, record.FieldValues);
                foreach (var formulaFieldName in formulaDataRecordFieldsDict)
                {
                    if (!record.FieldValues.ContainsKey(formulaFieldName.Key))
                        record.FieldValues.Add(formulaFieldName.Key, dataRecordObject.GetFieldValue(formulaFieldName.Key));
                }
            }
        }


        public bool AddDataRecord(Guid dataRecordStorageId, Dictionary<string, Object> fieldValues, int? userId, out Object insertedId, out bool hasInsertedId, TempStorageInformation tempStorageInformation = null)
        {
            var dataRecordStorage = GetDataRecordStorage(dataRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);

            var storageDataManager = GetStorageDataManager(dataRecordStorageId, tempStorageInformation);
            storageDataManager.ThrowIfNull("storageDataManager");

            var dataRecordType = _recordTypeManager.GetDataRecordType(dataRecordStorage.DataRecordTypeId);
            dataRecordType.ThrowIfNull("dataRecordType", dataRecordStorage.DataRecordTypeId);

            var idFieldType = dataRecordType.Fields.FindRecord(x => x.Name == dataRecordType.Settings.IdField);
            dataRecordType.ThrowIfNull("idFieldType");

            hasInsertedId = true;
            Guid? idField;
            if (idFieldType.Type.TryGenerateUniqueIdentifier(out idField))
            {
                hasInsertedId = false;
                fieldValues.Add(idFieldType.Name, idField);
            }
            if (!userId.HasValue)
                SecurityContext.Current.TryGetLoggedInUserId(out userId);
            bool insertActionSucc = storageDataManager.Insert(fieldValues, userId, userId, out insertedId);

            if (insertActionSucc && dataRecordStorage.Settings.EnableUseCaching)
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<RecordCacheManager>().SetCacheExpired(dataRecordStorageId);

            return insertActionSucc;
        }

        public bool AddDataRecord(Guid dataRecordStorageId, Dictionary<string, Object> fieldValues, out Object insertedId, out bool hasInsertedId, TempStorageInformation tempStorageInformation = null)
        {
            return AddDataRecord(dataRecordStorageId, fieldValues, null, out insertedId, out hasInsertedId,  tempStorageInformation);
        }
        public void AddDataRecords(Guid dataRecordStorageId, IEnumerable<dynamic> records, TempStorageInformation tempStorageInformation = null)
        {
            var storageDataManager = GetStorageDataManager(dataRecordStorageId, tempStorageInformation);
            storageDataManager.ThrowIfNull("storageDataManager", dataRecordStorageId);
            storageDataManager.InsertRecords(records);
        }
        public bool UpdateDataRecord(Guid dataRecordStorageId, Object recordFieldId, Dictionary<string, Object> fieldValues, RecordFilterGroup filterGroup, int? userId, TempStorageInformation tempStorageInformation = null)
        {
            var storageDataManager = GetStorageDataManager(dataRecordStorageId, tempStorageInformation);
            storageDataManager.ThrowIfNull("storageDataManager");

            var dataRecordStorage = GetDataRecordStorage(dataRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);

            if (fieldValues != null)
            {
                var dataRecordType = _recordTypeManager.GetDataRecordType(dataRecordStorage.DataRecordTypeId);
                var idFieldType = dataRecordType.Fields.FindRecord(x => x.Name == dataRecordType.Settings.IdField);
                fieldValues.Add(idFieldType.Name, recordFieldId);
            }
            if (!userId.HasValue)
                SecurityContext.Current.TryGetLoggedInUserId(out userId);

            bool updateActionSucc = storageDataManager.Update(fieldValues, userId, filterGroup);

            if (updateActionSucc && dataRecordStorage.Settings.EnableUseCaching)
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<RecordCacheManager>().SetCacheExpired(dataRecordStorageId);

            return updateActionSucc;
        }
        public bool UpdateDataRecord(Guid dataRecordStorageId, Object recordFieldId, Dictionary<string, Object> fieldValues, RecordFilterGroup filterGroup, TempStorageInformation tempStorageInformation = null)
        {
            return UpdateDataRecord(dataRecordStorageId, recordFieldId, fieldValues, filterGroup, null, tempStorageInformation);
        }

        public bool DeleteDataRecord(Guid dataRecordStorageId, List<object> recordFieldIds)
        {
            var storageDataManager = GetStorageDataManager(dataRecordStorageId);
            storageDataManager.ThrowIfNull("storageDataManager");

            var dataRecordStorage = GetDataRecordStorage(dataRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);

            bool deleteOperationSucc = storageDataManager.Delete(recordFieldIds);

            if (deleteOperationSucc && dataRecordStorage.Settings.EnableUseCaching)
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<RecordCacheManager>().SetCacheExpired(dataRecordStorageId);

            return deleteOperationSucc;
        }

        public void UpdateDataRecords(Guid dataRecordStorageId, IEnumerable<dynamic> records, List<string> fieldsToJoin, List<string> fieldsToUpdate, TempStorageInformation tempStorageInformation = null)
        {
            var storageDataManager = GetStorageDataManager(dataRecordStorageId, tempStorageInformation);
            storageDataManager.ThrowIfNull("storageDataManager", dataRecordStorageId);
            storageDataManager.UpdateRecords(records, fieldsToJoin, fieldsToUpdate);
        }

        public DateTime? GetMinDateTimeWithMaxIdAfterId(Guid dataRecordStorageId, long id, out long? maxId)
        {
            var storageDataManager = GetStorageDataManager(dataRecordStorageId);
            storageDataManager.ThrowIfNull("storageDataManager", dataRecordStorageId);
            return storageDataManager.GetMinDateTimeWithMaxIdAfterId(id, out maxId);
        }

        public DateTime? GetMinDateTimeWithMaxIdByFilter(Guid dataRecordStorageId, RecordFilterGroup filterGroup, out long? maxId)
        {
            var storageDataManager = GetStorageDataManager(dataRecordStorageId);
            storageDataManager.ThrowIfNull("storageDataManager", dataRecordStorageId);
            return storageDataManager.GetMinDateTimeWithMaxIdByFilter(filterGroup, out maxId);
        }

        public long? GetMaxId(Guid dataRecordStorageId, out DateTime? maxDate, out DateTime? minDate)
        {
            var storageDataManager = GetStorageDataManager(dataRecordStorageId);
            storageDataManager.ThrowIfNull("storageDataManager", dataRecordStorageId);
            return storageDataManager.GetMaxId(out maxDate, out minDate);
        }

        #endregion

        #region Public Methods

        public RecordFilterGroup ConvertFilterGroup(RecordFilterGroup filterGroup, Guid recordTypeId)
        {
            DataRecordType dataRecordType = new DataRecordTypeManager().GetDataRecordType(recordTypeId);
            return ConvertFilterGroup(filterGroup, dataRecordType);
        }

        public RecordFilterGroup ConvertFilterGroup(RecordFilterGroup filterGroup, DataRecordType recordType)
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
                    record.ThrowIfNull("record", recordFilter.FieldName);

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
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(dataRecordStorage.DevProjectId))
                    return false;

                if (input.Query.Name != null && !dataRecordStorage.Name.ToUpper().Contains(input.Query.Name.ToUpper()))
                    return false;

                if (input.Query.DataRecordTypeIds != null && !input.Query.DataRecordTypeIds.Contains(dataRecordStorage.DataRecordTypeId))
                    return false;

                if (input.Query.DataStoreIds != null && !input.Query.DataStoreIds.Contains(dataRecordStorage.DataStoreId))
                    return false;

                return true;
            };
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
                if (Utilities.ShouldHideItemHavingDevProjectId(dataRecordStorage.DevProjectId))
                    return false;

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
            dataRecordStorage.Settings.ThrowIfNull("dataRecordStorage.Settings");
            if (!dataRecordStorage.Settings.DontReflectToDB)
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
            dataRecordStorage.Settings.ThrowIfNull("dataRecordStorage.Settings", dataRecordStorage.DataRecordStorageId);
            if (!dataRecordStorage.Settings.DontReflectToDB)
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

        public List<DataRecordStorage> GetDataRecordStorageList(List<Guid> DataRecordStorageIdsList)
        {
            var allRecordStorages = GetCachedDataRecordStorages().Where(k => DataRecordStorageIdsList.Contains(k.Key)).Select(v => v.Value).ToList();
            return allRecordStorages;
        }

        public IEnumerable<VRRestAPIRecordQueryInterceptorConfig> GetVRRestAPIRecordQueryInterceptorConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<VRRestAPIRecordQueryInterceptorConfig>(VRRestAPIRecordQueryInterceptorConfig.EXTENSION_TYPE);
        }

        public IEnumerable<DataRecordStoragePermanentFilterSettingsConfig> GetPermanentFilterSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<DataRecordStoragePermanentFilterSettingsConfig>(DataRecordStoragePermanentFilterSettingsConfig.EXTENSION_TYPE);
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

        public void FillDataRecordStorageFromTempStorage(Guid dataRecordStorageId, TempStorageInformation tempStorageInformation, DateTime from, DateTime to, RecordFilterGroup recordFilterGroup)
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
                To = to,
                RecordFilterGroup = recordFilterGroup
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

        public int GetDBQueryMaxParameterNumber(Guid dataRecordStorageId)
        {
            var storageDataManager = GetStorageDataManager(dataRecordStorageId);
            storageDataManager.ThrowIfNull("storageDataManager", dataRecordStorageId);
            return storageDataManager.GetDBQueryMaxParameterNumber();
        }

        public bool DoesUserHaveAccess(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input)
        {
            var userID = SecurityContext.Current.GetLoggedInUserId();
            return this.DoesUserHaveAccess(userID, input.Query.DataRecordStorageIds) && DoesUserHaveFieldsAccess(userID, input.Query.DataRecordStorageIds, input.Query.Columns);
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

        public bool DoesUserHaveFieldsAccess(int userId, List<Guid> dataRecordStorages, IEnumerable<string> fieldNames)
        {
            var allRecordStorages = GetCachedDataRecordStorages().Where(k => dataRecordStorages.Contains(k.Key)).Select(v => v.Value).ToList();
            foreach (var r in allRecordStorages)
            {
                if (!DoesUserHaveAccessForAllRecordStorageFields(userId, r.Settings.FieldsPermissions, fieldNames))
                    return false;
            }
            return true;
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

        bool DoesUserHaveAccessForAllRecordStorageFields(int userId, List<DataRecordStorageFieldsPermission> FieldsPermissions, IEnumerable<string> fieldNames)
        {
            SecurityManager secManager = new SecurityManager();
            if (FieldsPermissions != null && FieldsPermissions.Count > 0)
            {
                if (fieldNames != null)
                {
                    foreach (var fieldName in fieldNames)
                    {
                        var item = FieldsPermissions.FindRecord(x => x.FieldNames.Contains(fieldName));
                        if (item != null)
                        {
                            if (!secManager.IsAllowed(item.RequiredPermission, userId))
                                return false;
                        }
                    }
                }
            }
            return true;
        }
        private RecordFilterGroup MergeRecordFilterGroup(Guid dataRecordStorageId, RecordFilterGroup oldRecordFilter)
        {
            var dataRecord = GetDataRecordStorage(dataRecordStorageId);

            if (dataRecord != null && dataRecord.Settings != null && dataRecord.Settings.PermanentFilter != null && dataRecord.Settings.PermanentFilter.Settings != null)
            {
                var permanentFilter = dataRecord.Settings.PermanentFilter.Settings.ConvertToRecordFilter(new DataRecordStoragePermanentFilterContext());
                if (permanentFilter != null)
                {
                    if (oldRecordFilter != null)
                    {
                        if (oldRecordFilter.LogicalOperator == RecordQueryLogicalOperator.And)
                        {
                            if (oldRecordFilter.Filters == null)
                                oldRecordFilter.Filters = new List<RecordFilter>();
                            oldRecordFilter.Filters.Add(permanentFilter);
                            return oldRecordFilter;
                        }
                        else
                        {
                            var recordFilterGroup = new RecordFilterGroup()
                            {
                                LogicalOperator = RecordQueryLogicalOperator.And,
                                Filters = new List<RecordFilter>()
                            };
                            recordFilterGroup.Filters.Add(permanentFilter);
                            recordFilterGroup.Filters.Add(oldRecordFilter);
                            return recordFilterGroup;
                        }
                    }
                    else
                    {
                        return permanentFilter;
                    }

                }
            }
            return oldRecordFilter;
        }

        public Dictionary<Guid, DataRecordStorage> GetCachedDataRecordStorages()
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

        #region Private Methods Data Record

        private List<DataRecord> GetCachedDataRecords(Guid dataRecordStorageId)
        {
            return CacheManagerFactory.GetCacheManager<RecordCacheManager>().GetOrCreateObject("GetCachedDataRecords", dataRecordStorageId,
                () =>
                {
                    return GetDataRecords(dataRecordStorageId);
                });
        }
        private Dictionary<Object, DataRecord> GetCachedDataRecordsDict(Guid dataRecordStorageId)
        {
            return CacheManagerFactory.GetCacheManager<RecordCacheManager>().GetOrCreateObject("GetCachedDataRecordsDict", dataRecordStorageId,
                () =>
                {
                    var dataRecords = GetCachedDataRecords(dataRecordStorageId);
                    var dataRecordStorage = GetDataRecordStorage(dataRecordStorageId);
                    dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);
                    var recordTyeIdField = _recordTypeManager.GetDataRecordTypeIdField(dataRecordStorage.DataRecordTypeId);
                    recordTyeIdField.ThrowIfNull("recordTyeIdField", dataRecordStorage.DataRecordTypeId);
                    recordTyeIdField.Name.ThrowIfNull("recordTyeIdField.Name", dataRecordStorage.DataRecordTypeId);
                    return dataRecords.ToDictionary(key => key.FieldValues[recordTyeIdField.Name], value => value);
                });
        }
        private List<DataRecord> GetDataRecords(Guid dataRecordStorageId, List<string> columnsNeeded = null)
        {
            var dataManager = GetStorageDataManager(dataRecordStorageId);
            dataManager.ThrowIfNull("dataManager", dataRecordStorageId);

            var dataRecordStorage = GetDataRecordStorage(dataRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);

            var dataRecordTypeFields = _recordTypeManager.GetDataRecordTypeFields(dataRecordStorage.DataRecordTypeId);
            dataRecordTypeFields.ThrowIfNull("dataRecordTypeFields", dataRecordStorage.DataRecordTypeId);
            List<string> columns;
            if (columnsNeeded != null)
            {
                foreach (var item in columnsNeeded)
                {
                    if (!dataRecordTypeFields.ContainsKey(item))
                    {
                        throw new NullReferenceException(string.Format("'{0}' field not found.", item));
                    }
                }
                columns = columnsNeeded;
            }
            else
            {
                columns = dataRecordTypeFields.FindAllRecords(itm => itm.Formula == null).Select(x => x.Name).ToList();
            }
            Dictionary<string, DataRecordField> formulaDataRecordFieldsDict;
            PrepareDependentAndFormulaFields(columns, dataRecordTypeFields, out formulaDataRecordFieldsDict);
            var dataRecords = dataManager.GetAllDataRecords(columns);
            FillDataRecordsFormulaFields(dataRecordStorage.DataRecordTypeId, formulaDataRecordFieldsDict, dataRecords);
            return dataRecords;
        }

        private void FillDataRecordsFormulaFields(Guid dataRecordTypeId, Dictionary<string, DataRecordField> formulaDataRecordFieldsDict, List<DataRecord> dataRecords)
        {
            if (formulaDataRecordFieldsDict != null && dataRecords != null)
            {
                foreach (var dataRecord in dataRecords)
                {
                    FillDataRecordFormulaFields(dataRecordTypeId, dataRecord, formulaDataRecordFieldsDict);
                }
            }
        }
        private Dictionary<string, DataRecordField> GetFormulaDataRecordFields(Dictionary<string, DataRecordField> dataRecordFieldsDict, List<string> fieldNames)
        {
            IEnumerable<DataRecordField> formulaDataRecordFields = dataRecordFieldsDict.Values.FindAllRecords(itm => itm.Formula != null && fieldNames.Contains(itm.Name));
            return (formulaDataRecordFields != null && formulaDataRecordFields.Count() > 0) ? formulaDataRecordFields.ToDictionary(itm => itm.Name, itm => itm) : null;
        }

        public bool DoesDataRecordStorageDenyAPICall(Guid dataRecordStorageId)
        {
            var dataRecordStorage = GetDataRecordStorage(dataRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);
            dataRecordStorage.Settings.ThrowIfNull("dataRecordStorage.Settings", dataRecordStorage.DataRecordStorageId);
            return dataRecordStorage.Settings.DenyAPICall;
        }

        private IOrderedEnumerable<DataRecordDetail> GetOrderedByFields(List<SortColumn> sortColumns, IOrderedEnumerable<DataRecordDetail> orderedRecords, DataRecordType recordType)
        {
            foreach (SortColumn sortColumn in sortColumns)
            {
                DataRecordField field = recordType.Fields.FirstOrDefault(itm => itm.Name == sortColumn.FieldName);
                field.ThrowIfNull("field", sortColumn.FieldName);

                switch (field.Type.OrderType)
                {
                    case DataRecordFieldOrderType.ByFieldDescription: orderedRecords = sortColumn.IsDescending ? orderedRecords.ThenByDescending(itm => itm.FieldValues[sortColumn.FieldName].Description) : orderedRecords.ThenBy(itm => itm.FieldValues[sortColumn.FieldName].Description); break;
                    case DataRecordFieldOrderType.ByFieldValue: orderedRecords = sortColumn.IsDescending ? orderedRecords.ThenByDescending(itm => itm.FieldValues[sortColumn.FieldName].Value) : orderedRecords.ThenBy(itm => itm.FieldValues[sortColumn.FieldName].Value); break;
                    default: break;
                }
            }
            return orderedRecords;
        }

        private static IEnumerable<DataRecord> GetOrderedDataRecords(OrderType orderType, List<string> fieldNames, AdvancedOrderOptionsBase advancedOrderOptions,
            IEnumerable<DataRecord> allRecords)
        {
            IEnumerable<DataRecord> orderedRecords;
            switch (orderType)
            {
                case OrderType.ByAllFields: orderedRecords = GetOrderedByAllFields(fieldNames, allRecords, false); break;
                case OrderType.ByAllFieldsDescending: orderedRecords = GetOrderedByAllFields(fieldNames, allRecords, true); break;
                case OrderType.AdvancedFieldOrder: orderedRecords = GetOrderedByAdvancedFieldOrder(fieldNames, advancedOrderOptions, allRecords); break;
                default: orderedRecords = null; break;
            }
            return orderedRecords;
        }
        private static IEnumerable<DataRecord> GetOrderedByAllFields(List<string> fieldNames, IEnumerable<DataRecord> allRecords, bool descOrder)
        {
            List<string> orderByFields = fieldNames;
            if (orderByFields == null || orderByFields.Count == 0)
                throw new NullReferenceException($"orderByFields '{orderByFields}'");

            var firstField = orderByFields[0];
            IOrderedEnumerable<DataRecord> orderedRecords;
            Func<DataRecord, Object> firstOrderByFunction = record => record.FieldValues[firstField];
            orderedRecords = descOrder ? allRecords.OrderByDescending(firstOrderByFunction) : allRecords.OrderBy(firstOrderByFunction);

            if (orderByFields.Count > 1)
            {
                for (int i = 1; i < orderByFields.Count; i++)
                {
                    var field = orderByFields[i];
                    Func<DataRecord, Object> orderByFunction = record => record.FieldValues[field];
                    orderedRecords = descOrder ? orderedRecords.ThenByDescending(orderByFunction) : orderedRecords.ThenBy(orderByFunction);
                }
            }
            return orderedRecords;
        }

        private static IEnumerable<DataRecord> GetOrderedByAdvancedFieldOrder(List<string> fieldNames, AdvancedOrderOptionsBase advancedOrderOptions, IEnumerable<DataRecord> allRecords)
        {
            if (fieldNames == null)
                throw new NullReferenceException($"fieldNames '{fieldNames}'");

            AdvancedFieldOrderOptions advancedFieldOrderOptions = advancedOrderOptions.CastWithValidate<AdvancedFieldOrderOptions>("advancedOrderOptions");

            if (advancedFieldOrderOptions.Fields == null || advancedFieldOrderOptions.Fields.Count == 0)
                throw new NullReferenceException($"fields '{advancedFieldOrderOptions.Fields}'");

            var fieldOrders = advancedFieldOrderOptions.Fields;
            var firstFieldOrder = fieldOrders[0];
            if (!fieldNames.Contains(firstFieldOrder.FieldName))
                throw new Exception(String.Format("Field Order '{0}' is not available in the query field names", firstFieldOrder.FieldName));
            Func<DataRecord, Object> firstOrderByFunction = record => record.FieldValues[firstFieldOrder.FieldName];

            IOrderedEnumerable<DataRecord> orderedRecords = firstFieldOrder.OrderDirection == OrderDirection.Ascending ?
                allRecords.OrderBy(firstOrderByFunction) :
                allRecords.OrderByDescending(firstOrderByFunction);
            if (fieldOrders.Count > 1)
            {
                for (int i = 1; i < fieldOrders.Count; i++)
                {
                    var fieldOrder = fieldOrders[i];
                    if (!fieldNames.Contains(fieldOrder.FieldName))
                        throw new Exception(String.Format("Field Order '{0}' is not available in the query fields", fieldOrder.FieldName));
                    Func<DataRecord, Object> orderByFunction = record => record.FieldValues[fieldOrder.FieldName];

                    orderedRecords = fieldOrder.OrderDirection == OrderDirection.Ascending ?
                        orderedRecords.ThenBy(orderByFunction) :
                        orderedRecords.ThenByDescending(orderByFunction);
                }
            }
            return orderedRecords;
        }

        private void PrepareDependentAndFormulaFields(List<string> fieldNames, Dictionary<string, DataRecordField> dataRecordFieldDict, out Dictionary<string, DataRecordField> formulaDataRecordFieldsDict)
        {
            var dependentDataRecordStorageFields = new HashSet<string>();
            var formulaFieldDirectDependencies = new Dictionary<string, List<string>>();

            formulaDataRecordFieldsDict = GetFormulaDataRecordFields(dataRecordFieldDict, fieldNames);

            if (formulaDataRecordFieldsDict != null)
            {
                Dictionary<string, DataRecordField> clonedFormulaDataRecordFieldsDict = Vanrise.Common.Utilities.CloneObject(formulaDataRecordFieldsDict);
                foreach (var formulaField in clonedFormulaDataRecordFieldsDict.Values)
                {
                    var currentDependentFields = formulaField.Formula.GetDependentFields(new DataRecordFieldFormulaGetDependentFieldsContext());
                    dependentDataRecordStorageFields.UnionWith(GetDependentDataRecordStorageFieldNames(currentDependentFields, formulaDataRecordFieldsDict, formulaFieldDirectDependencies, dataRecordFieldDict));

                    if (!formulaFieldDirectDependencies.ContainsKey(formulaField.Name))
                        formulaFieldDirectDependencies.Add(formulaField.Name, currentDependentFields);
                }
            }

            if (fieldNames != null && fieldNames.Count > 0)
            {
                fieldNames.RemoveAll(itm => dataRecordFieldDict.GetRecord(itm).Formula != null);

                if (dependentDataRecordStorageFields.Count > 0)
                {
                    fieldNames.AddRange(dependentDataRecordStorageFields);

                    HashSet<string> existingColumnNames = new HashSet<string>();
                    for (int i = fieldNames.Count - 1; i >= 0; i--)
                    {
                        string currentFieldName = fieldNames[i];
                        if (existingColumnNames.Contains(fieldNames[i]))
                            fieldNames.RemoveAt(i);

                        existingColumnNames.Add(currentFieldName);
                    }
                }
            }
        }

        public BigResult<DataRecordDetail> AllRecordsToBigResult(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input, IEnumerable<DataRecord> allRecords, DataRecordType recordType)
        {
            if (allRecords == null)
                return new Vanrise.Entities.BigResult<DataRecordDetail>()
                {
                    ResultKey = input.ResultKey,
                    Data = null,
                    TotalCount = 0
                };

            HashSet<string> fieldsThatDescriptionUsedForOrdering = GetFieldsThatDescriptionUsedForOrdering(input, recordType);
            IEnumerable<DataRecordDetail> allRecordsDetails = DataRecordDetailMapperList(allRecords, recordType, fieldsThatDescriptionUsedForOrdering);

            IOrderedEnumerable<DataRecordDetail> orderedRecords;
            if (!string.IsNullOrEmpty(input.SortByColumnName))
                orderedRecords = allRecordsDetails.VROrderList(input);
            else
                orderedRecords = input.Query.Direction == OrderDirection.Ascending ? allRecordsDetails.OrderBy(itm => itm.RecordTime) : allRecordsDetails.OrderByDescending(itm => itm.RecordTime);

            if (input.Query.SortColumns != null && input.Query.SortColumns.Count > 0)
                orderedRecords = GetOrderedByFields(input.Query.SortColumns, orderedRecords, recordType);

            IEnumerable<DataRecordDetail> pagedRecords = orderedRecords.VRGetPage(input);

            CompleteDataRecordDetailMapper(pagedRecords, recordType, fieldsThatDescriptionUsedForOrdering);

            var dataRecordBigResult = new Vanrise.Entities.BigResult<DataRecordDetail>()
            {
                ResultKey = input.ResultKey,
                Data = pagedRecords,
                TotalCount = allRecordsDetails.Count()
            };

            return dataRecordBigResult;
        }

        private HashSet<string> GetFieldsThatDescriptionUsedForOrdering(DataRetrievalInput<DataRecordQuery> input, DataRecordType recordType)
        {
            HashSet<string> fieldNames = new HashSet<string>();
            if (input.SortByColumnName != null)
            {
                if (input.SortByColumnName.EndsWith(".Description"))
                {
                    string sortFieldName = input.SortByColumnName.Replace("FieldValues[\"", "").Replace("\"].Description", "");
                    fieldNames.Add(sortFieldName);
                }
            }

            if (input.Query.SortColumns != null)
            {
                foreach (var sortColumn in input.Query.SortColumns)
                {
                    DataRecordField field = recordType.Fields.FirstOrDefault(itm => itm.Name == sortColumn.FieldName);
                    field.ThrowIfNull("field", sortColumn.FieldName);
                    if (field.Type.OrderType == DataRecordFieldOrderType.ByFieldDescription)
                        fieldNames.Add(sortColumn.FieldName);
                }
            }
            return fieldNames;
        }

        public List<DataRecordDetail> DataRecordDetailMapperList(IEnumerable<DataRecord> dataRecords, DataRecordType recordType, HashSet<string> fieldsToGetDescription)
        {
            if (dataRecords == null)
                return null;

            List<DataRecordDetail> result = new List<DataRecordDetail>();
            foreach (DataRecord dataRecord in dataRecords)
                result.Add(DataRecordDetail(dataRecord, recordType, fieldsToGetDescription));

            return result;
        }

        private DataRecordDetail DataRecordDetail(DataRecord entity, DataRecordType recordType, HashSet<string> fieldsToGetDescription)
        {
            var dataRecordDetail = new DataRecordDetail() { RecordTime = entity.RecordTime, FieldValues = new Dictionary<string, DataRecordFieldValue>() };
            foreach (var fld in recordType.Fields)
            {
                Object value;
                DataRecordFieldValue fldValueDetail = new DataRecordFieldValue();

                if (entity.FieldValues.TryGetValue(fld.Name, out value))
                {
                    fldValueDetail.Value = value;
                    if (fieldsToGetDescription.Contains(fld.Name))
                        fldValueDetail.Description = fld.Type.GetDescription(value);
                    dataRecordDetail.FieldValues.Add(fld.Name, fldValueDetail);
                }
            }
            return dataRecordDetail;
        }

        public void CompleteDataRecordDetailMapper(IEnumerable<DataRecordDetail> dataRecordDetails, DataRecordType recordType, HashSet<string> fieldsHavingDescriptionAlreadySet)
        {
            if (dataRecordDetails == null)
                return;

            foreach (var dataRecordDetail in dataRecordDetails)
            {
                foreach (var fld in recordType.Fields)
                {
                    if (fieldsHavingDescriptionAlreadySet.Contains(fld.Name))
                        continue;

                    DataRecordFieldValue fldValueDetail;

                    if (dataRecordDetail.FieldValues.TryGetValue(fld.Name, out fldValueDetail))
                    {
                        fldValueDetail.Description = fld.Type.GetDescription(fldValueDetail.Value);
                    }
                }
            }

        }

        private List<DataRecord> GetTopOrderedResults(List<DataRecord> records, OrderDirection direction, int? limitResult)
        {
            if (records.Count > 0)
            {
                IOrderedEnumerable<DataRecord> orderedDataRecord = null;
                if (direction == OrderDirection.Ascending)
                {
                    orderedDataRecord = records.OrderBy(itm => itm.RecordTime);
                }
                else
                {
                    orderedDataRecord = records.OrderByDescending(itm => itm.RecordTime);
                }

                if (limitResult.HasValue)
                {
                    return orderedDataRecord.Take(limitResult.Value).ToList();
                }
                else
                {
                    return orderedDataRecord.ToList();
                }
            }
            else
            {
                return null;
            }
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

        private HashSet<string> GetDependentDataRecordStorageFieldNames(List<string> directDependentFields, Dictionary<string, DataRecordField> formulaDataRecordFieldsDict, Dictionary<string, List<string>> formulaFieldDirectDependencies, Dictionary<string, DataRecordField> dataRecordFieldDict)
        {
            HashSet<string> results = new HashSet<string>();

            foreach (var directDependentField in directDependentFields)
            {
                DataRecordField currentDataRecordField;
                if (dataRecordFieldDict.TryGetValue(directDependentField, out currentDataRecordField))
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
                            formulaDataRecordFieldsDict.Add(directDependentField, dataRecordFieldDict.Values.FindRecord(itm => string.Compare(itm.Name, directDependentField, true) == 0));

                        results.UnionWith(GetDependentDataRecordStorageFieldNames(currentFieldDirectDependentFields, formulaDataRecordFieldsDict, formulaFieldDirectDependencies, dataRecordFieldDict));
                    }
                }
            }

            return results;
        }

        private IEnumerable<DataRecord> GetOrderedDataRecordResults(Guid dataRecordTypeId, OrderDirection direction, int? limitresult, List<string> fieldNames, List<DataRecord> records, Dictionary<string, DataRecordField> formulaDataRecordFieldsDict)
        {
            if (records == null || records.Count == 0)
                return records;
            List<DataRecord> orderedDataRecordResults = GetTopOrderedResults(records, direction, limitresult);

            FillDataRecordsFormulaFields(dataRecordTypeId, formulaDataRecordFieldsDict, orderedDataRecordResults);

            return orderedDataRecordResults;
        }

        #endregion

        #region Private Classes Data Record

        public class RecordCacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        {

            DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();


            ConcurrentDictionary<Guid, Object> _updateHandlesByDataStorage = new ConcurrentDictionary<Guid, Object>();

            protected override bool ShouldSetCacheExpired(Guid dataRecordStorageId)
            {
                Object updateHandle;

                _updateHandlesByDataStorage.TryGetValue(dataRecordStorageId, out updateHandle);
                var dataManager = dataRecordStorageManager.GetStorageDataManager(dataRecordStorageId);
                bool isCacheExpired = dataManager.AreDataRecordsUpdated(ref updateHandle);
                _updateHandlesByDataStorage.AddOrUpdate(dataRecordStorageId, updateHandle, (key, existingHandle) => updateHandle);

                return isCacheExpired;
            }
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
            private DataRecordType DataRecordType
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

            #region Public and Protected Methods

            public override IEnumerable<DataRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input)
            {
                DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
                var dataRecordsFinalResult = dataRecordStorageManager.GetDataRecordsFinalResult(DataRecordType, input, (dataRecordStorageId, cloneInput) =>
                {
                    return dataRecordStorageManager.GetDataRecords(cloneInput.Query.FromTime, cloneInput.Query.ToTime, cloneInput.Query.FilterGroup, cloneInput.Query.Columns, cloneInput.Query.LimitResult, cloneInput.Query.Direction, dataRecordStorageId);
                });
                if (input.Query.BulkActionState != null)
                {
                    VRBulkActionDraftManager bulkActionDraftManager = new VRBulkActionDraftManager();
                    GenericBusinessEntityDefinitionManager _genericBEDefinitionManager = new GenericBusinessEntityDefinitionManager();
                    bulkActionDraftManager.CreateWithClearVRBulkActionDraft(input.Query.BulkActionState, dataRecordsFinalResult, (records) =>
                    {
                        DataRecordStorageManager manager = new DataRecordStorageManager();
                        DataRecordTypeManager _recordTypeManager = new DataRecordTypeManager();
                        List<BulkActionItem> bulkActionItems = new List<BulkActionItem>();
                        var dataRecordStorageId = input.Query.DataRecordStorageIds.First();
                        var dataRecordStorage = manager.GetDataRecordStorage(dataRecordStorageId);
                        dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);
                        var idField = _recordTypeManager.GetDataRecordTypeIdField(dataRecordStorage.DataRecordTypeId);
                        foreach (var record in records)
                        {
                            bulkActionItems.Add(new BulkActionItem
                            {
                                ItemId = record.FieldValues.GetRecord(idField.Name).ToString()
                            });
                        }
                        return bulkActionItems;

                    });
                }
                var orderType = input.Query.OrderType;
                if (orderType.HasValue)
                    dataRecordsFinalResult = GetOrderedDataRecords(orderType.Value, input.Query.Columns, input.Query.AdvancedOrderOptions, dataRecordsFinalResult);
                return dataRecordsFinalResult;

            }

            protected override Vanrise.Entities.BigResult<DataRecordDetail> AllRecordsToBigResult(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input, IEnumerable<DataRecord> allRecords)
            {
                return new DataRecordStorageManager().AllRecordsToBigResult(input, allRecords, DataRecordType);
            }

            protected override ResultProcessingHandler<DataRecordDetail> GetResultProcessingHandler(DataRetrievalInput<DataRecordQuery> input, BigResult<DataRecordDetail> bigResult)
            {
                return new ResultProcessingHandler<DataRecordDetail>
                {
                    ExportExcelHandler = new DataRecordStorageExcelExportHandler(input.Query)
                };
            }

            public override DataRecordDetail EntityDetailMapper(DataRecord entity)
            {
                throw new NotImplementedException();
            }


            #endregion

            #region Private Methods



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

        private class DataRecordDataManagerGetFilteredDataRecordsContext : IDataRecordDataManagerGetFilteredDataRecordsContext
        {
            public DateTime FromTime
            {
                get;
                set;
            }

            public DateTime ToTime
            {
                get;
                set;
            }

            public RecordFilterGroup FilterGroup
            {
                get;
                set;
            }


            public List<string> FieldNames
            {
                get;
                set;
            }

            public int? LimitResult
            {
                get;
                set;
            }

            public OrderDirection Direction
            {
                get;
                set;
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