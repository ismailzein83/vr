using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class BusinessObjectDataRecordDataManager : GenericData.Entities.IDataRecordDataManager
    {
        #region Fields/Ctor

        static DataRecordTypeManager s_dataRecordTypeManager = new DataRecordTypeManager();
        static RecordFilterManager s_recordFilterManager = new RecordFilterManager();

        BusinessObjectDataRecordStorageSettings _businessObjectDataRecordStorageSettings;
        DataRecordStorage _dataRecordStorage;

        public BusinessObjectDataRecordDataManager(BusinessObjectDataRecordStorageSettings businessObjectDataRecordStorageSettings, DataRecordStorage dataRecordStorage)
        {
            _businessObjectDataRecordStorageSettings = businessObjectDataRecordStorageSettings;
            _dataRecordStorage = dataRecordStorage;
        }

        #endregion

        #region Public Methods

        public List<DataRecord> GetFilteredDataRecords(IDataRecordDataManagerGetFilteredDataRecordsContext context)
        {
            return GetFilteredDataRecords(context.FromTime, context.ToTime, context.FilterGroup, context.FieldNames, context.LimitResult, context.Direction);
        }

        public DataRecord GetDataRecord(object dataRecordId, List<string> fieldNames)
        {
            var dataRecordType = s_dataRecordTypeManager.GetDataRecordType(_dataRecordStorage.DataRecordTypeId);
            dataRecordType.ThrowIfNull("dataRecordType", _dataRecordStorage.DataRecordTypeId);
            dataRecordType.Settings.ThrowIfNull("dataRecordType.Settings", _dataRecordStorage.DataRecordTypeId);
            dataRecordType.Settings.IdField.ThrowIfNull("dataRecordType.Settings.IdField", _dataRecordStorage.DataRecordTypeId);
            var idField = s_dataRecordTypeManager.GetDataRecordField(_dataRecordStorage.DataRecordTypeId, dataRecordType.Settings.IdField);
            idField.ThrowIfNull("idField", _dataRecordStorage.DataRecordTypeId);
            var idRecordFilter = idField.Type.ConvertToRecordFilter(new DataRecordFieldTypeConvertToRecordFilterContext
            {
                FieldName = dataRecordType.Settings.IdField,
                FilterValues = new List<object> { dataRecordId },
                StrictEqual = true
            });
            idRecordFilter.ThrowIfNull("recordFilter", _dataRecordStorage.DataRecordTypeId);
            var idFilterGroup = new RecordFilterGroup
            {
                LogicalOperator = RecordQueryLogicalOperator.And,
                Filters = new List<RecordFilter> { idRecordFilter }
            };
            var records = GetFilteredDataRecords(null, null, idFilterGroup, fieldNames, null, null);
            return records != null && records.Count > 0 ? records[0] : null;
        }

        public void GetDataRecords(DateTime? from, DateTime? to, RecordFilterGroup filterGroup, Func<bool> shouldStop, Action<dynamic> onItemReady, string orderColumnName = null, bool isOrderAscending = false)
        {
            var recordType = s_dataRecordTypeManager.GetDataRecordType(_dataRecordStorage.DataRecordTypeId);
            recordType.ThrowIfNull("recordType", _dataRecordStorage.DataRecordTypeId);
            recordType.Fields.ThrowIfNull("recordType.Fields", _dataRecordStorage.DataRecordTypeId);

            var loadContext = new BusinessObjectDataProviderLoadRecordsContext
            {
                FromTime = from,
                ToTime = to,
                FilterGroup = filterGroup,
                Fields = recordType.Fields.Where(fld => fld.Formula == null).Select(fld => fld.Name).ToList()
            };

            bool doesSupportFilterOnAllFields = _businessObjectDataRecordStorageSettings.Settings.ExtendedSettings.DoesSupportFilterOnAllFields;
            loadContext.OnRecordLoadedAction = (record, recordTime) =>
            {
                if (doesSupportFilterOnAllFields
                    || filterGroup == null
                    || s_recordFilterManager.IsFilterGroupMatch(filterGroup, new DataRecordFilterGenericFieldMatchContext(record)))
                {
                    onItemReady(record.Object);
                }
            };
            _businessObjectDataRecordStorageSettings.Settings.ExtendedSettings.LoadRecords(loadContext);
        }

        public List<DataRecord> GetAllDataRecords(List<string> columns)
        {
            return GetFilteredDataRecords(null, null, null, columns, null, null);
        }

        public bool Insert(Dictionary<string, object> fieldValues, int? createdUserId, int? modifiedUserId, out object insertedId)
        {
            throw new NotImplementedException();
        }

        public void InsertRecords(IEnumerable<dynamic> records)
        {
            throw new NotImplementedException();
        }

        public bool Update(Dictionary<string, object> fieldValues, int? modifiedUserId, RecordFilterGroup filterGroup)
        {
            throw new NotImplementedException();
        }

        public void UpdateRecords(IEnumerable<dynamic> records, List<string> fieldsToJoin, List<string> fieldsToUpdate)
        {
            throw new NotImplementedException();
        }

        public bool Delete(List<object> recordFieldIds)
        {
            throw new NotImplementedException();
        }

        public void DeleteRecords(DateTime from, DateTime to, RecordFilterGroup recordFilterGroup)
        {
            throw new NotImplementedException();
        }

        public void DeleteRecords(DateTime dateTime, RecordFilterGroup recordFilterGroup)
        {
            throw new NotImplementedException();
        }

        public void DeleteRecords(DateTime fromDate, DateTime toDate, List<long> idsToDelete)
        {
            throw new NotImplementedException();
        }

        public bool AreDataRecordsUpdated(ref object updateHandle)
        {
            if (updateHandle is DateTime)
            {
                DateTime lastUpdateHandle = (DateTime)updateHandle;
                if ((DateTime.Now - lastUpdateHandle).TotalMinutes > 5)
                {
                    updateHandle = DateTime.Now;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                updateHandle = DateTime.Now;
                return true;
            }
        }

        public int GetDBQueryMaxParameterNumber()
        {
            throw new NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            throw new NotImplementedException();
        }

        public void WriteRecordToStream(object record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public void ApplyStreamToDB(object stream)
        {
            throw new NotImplementedException();
        }

        public DateTime? GetMinDateTimeWithMaxIdAfterId(long id, out long? maxId)
        {
            throw new NotImplementedException();
        }

        public long? GetMaxId(out DateTime? maxDate, out DateTime? minDate)
        {
            throw new NotImplementedException();
        }

        public DateTime? GetMinDateTimeWithMaxIdByFilter(RecordFilterGroup filterGroup, out long? maxId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private List<DataRecord> GetFilteredDataRecords(DateTime? fromTime, DateTime? toTime, RecordFilterGroup filterGroup, List<string> fieldNames, int? limitResult, OrderDirection? direction)
        {
            List<DataRecord> dataRecords = new List<DataRecord>();
            var loadContext = new BusinessObjectDataProviderLoadRecordsContext
            {
                FromTime = fromTime,
                ToTime = toTime,
                FilterGroup = filterGroup,
                Fields = fieldNames,
                LimitResult = limitResult,
                OrderDirection = direction
            };
            bool doesSupportFilterOnAllFields = _businessObjectDataRecordStorageSettings.Settings.ExtendedSettings.DoesSupportFilterOnAllFields;
            int loadedRecords = 0;
            loadContext.OnRecordLoadedAction = (dataRecordObject, recordTime) =>
            {
                bool shouldAddRecord = false;
                if (doesSupportFilterOnAllFields)
                {
                    shouldAddRecord = true;
                }
                else
                {
                    if (filterGroup == null || s_recordFilterManager.IsFilterGroupMatch(filterGroup, new DataRecordFilterGenericFieldMatchContext(dataRecordObject)))
                    {
                        shouldAddRecord = true;
                    }
                }
                if (shouldAddRecord)
                {
                    var dataRecord = new DataRecord { RecordTime = recordTime, FieldValues = new Dictionary<string, object>() };
                    if (fieldNames != null)
                    {
                        foreach (var fieldName in fieldNames)
                        {
                            dataRecord.FieldValues.Add(fieldName, dataRecordObject.GetFieldValue(fieldName));
                        }
                    }
                    dataRecords.Add(dataRecord);
                    loadedRecords++;
                    if (loadedRecords >= limitResult)
                        loadContext.StopLoadingRecords();
                }
            };
            _businessObjectDataRecordStorageSettings.Settings.ExtendedSettings.LoadRecords(loadContext);
            return dataRecords;
        }

        #endregion

        #region Private Classes

        private class BusinessObjectDataProviderLoadRecordsContext : IBusinessObjectDataProviderLoadRecordsContext
        {
            public Action<DataRecordObject, DateTime> OnRecordLoadedAction { get; set; }

            public DateTime? FromTime { get; set; }

            public DateTime? ToTime { get; set; }

            public RecordFilterGroup FilterGroup { get; set; }

            public int? LimitResult { get; set; }

            public OrderDirection? OrderDirection { get; set; }

            public bool IsLoadStopped { get; set; }

            public List<string> Fields { get; set; }

            public void OnRecordLoaded(DataRecordObject dataRecordObject, DateTime recordTime) { this.OnRecordLoadedAction(dataRecordObject, recordTime); }

            public void StopLoadingRecords() { this.IsLoadStopped = true; }
        }

        #endregion
    }
}