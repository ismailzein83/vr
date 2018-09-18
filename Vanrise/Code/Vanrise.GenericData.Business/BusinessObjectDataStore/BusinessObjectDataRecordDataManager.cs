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
        static DataRecordTypeManager s_dataRecordTypeManager = new DataRecordTypeManager();
        static RecordFilterManager s_recordFilterManager = new RecordFilterManager();

        BusinessObjectDataRecordStorageSettings _businessObjectDataRecordStorageSettings;
        DataRecordStorage _dataRecordStorage;

        public BusinessObjectDataRecordDataManager(BusinessObjectDataRecordStorageSettings businessObjectDataRecordStorageSettings, DataRecordStorage dataRecordStorage)
        {
            _businessObjectDataRecordStorageSettings = businessObjectDataRecordStorageSettings;
            _dataRecordStorage = dataRecordStorage;
        }

        public List<DataRecord> GetFilteredDataRecords(IDataRecordDataManagerGetFilteredDataRecordsContext context)
        {
            List<DataRecord> dataRecords = new List<DataRecord>();
            var loadContext = new BusinessObjectDataProviderLoadRecordsContext
            {
                FromTime = context.FromTime,
                ToTime = context.ToTime,
                FilterGroup = context.FilterGroup,
                Fields = context.FieldNames,
                LimitResult = context.LimitResult,
                OrderDirection = context.Direction
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
                    if (context.FilterGroup == null || s_recordFilterManager.IsFilterGroupMatch(context.FilterGroup, new DataRecordFilterGenericFieldMatchContext(dataRecordObject)))
                    {
                        shouldAddRecord = true;
                    }
                }
                if (shouldAddRecord)
                {
                    var dataRecord = new DataRecord { RecordTime = recordTime, FieldValues = new Dictionary<string, object>() };
                    if (context.FieldNames != null)
                    {
                        foreach (var fieldName in context.FieldNames)
                        {
                            dataRecord.FieldValues.Add(fieldName, dataRecordObject.GetFieldValue(fieldName));
                        }
                    }
                    dataRecords.Add(dataRecord);
                    loadedRecords++;
                    if (loadedRecords >= context.LimitResult)
                        loadContext.StopLoadingRecords();
                }
            };
            _businessObjectDataRecordStorageSettings.Settings.ExtendedSettings.LoadRecords(loadContext);
            return dataRecords;
        }
        public bool Delete(List<object> recordFieldIds)
        {
            throw new NotImplementedException();
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
                Fields = recordType.Fields.Where(fld => fld.Formula != null).Select(fld => fld.Name).ToList()
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
            throw new NotImplementedException();
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

        public void DeleteRecords(DateTime from, DateTime to, RecordFilterGroup recordFilterGroup)
        {
            throw new NotImplementedException();
        }

        public void DeleteRecords(DateTime dateTime, RecordFilterGroup recordFilterGroup)
        {
            throw new NotImplementedException();
        }

        public bool AreDataRecordsUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
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


        public DateTime? GetMinDateTimeWithMaxIdAfterId(long id, string idFieldName, string dateTimeFieldName, out long? maxId)
        {
            throw new NotImplementedException();
        }


        public void DeleteRecords(DateTime fromDate, DateTime toDate, List<long> idsToDelete, string idFieldName, string dateTimeFieldName)
        {
            throw new NotImplementedException();
        }


        public long? GetMaxId(string idFieldName, string dateTimeFieldName, out DateTime? maxDate, out DateTime? minDate)
        {
            throw new NotImplementedException();
        }
    }
}