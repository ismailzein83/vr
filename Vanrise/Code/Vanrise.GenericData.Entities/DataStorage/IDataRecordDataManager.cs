using System;
using System.Collections.Generic;

namespace Vanrise.GenericData.Entities
{
    public interface IDataRecordDataManager : Vanrise.Data.IBulkApplyDataManager<Object>
    {
        void ApplyStreamToDB(object stream);

        List<DataRecord> GetFilteredDataRecords(IDataRecordDataManagerGetFilteredDataRecordsContext context);

        void GetDataRecords(DateTime? from, DateTime? to, RecordFilterGroup recordFilterGroup, Func<bool> shouldStop, Action<dynamic> onItemReady, string orderColumnName = null, bool isOrderAscending = false);

        List<DataRecord> GetAllDataRecords(List<string> columns);

        bool Insert(Dictionary<string, Object> fieldValues, int? createdUserId, int? modifiedUserId, out object insertedId);

        void InsertRecords(IEnumerable<dynamic> records);

        bool Update(Dictionary<string, Object> fieldValues, int? modifiedUserId, RecordFilterGroup filterGroup);

        void UpdateRecords(IEnumerable<dynamic> records, List<string> fieldsToJoin, List<string> fieldsToUpdate);

        bool Delete(List<object> recordFieldIds);

        void DeleteRecords(DateTime from, DateTime to, RecordFilterGroup recordFilterGroup);

        void DeleteRecords(DateTime dateTime, RecordFilterGroup recordFilterGroup);

        void DeleteRecords(DateTime fromDate, DateTime toDate, List<long> idsToDelete);

        bool AreDataRecordsUpdated(ref object updateHandle);

        int GetDBQueryMaxParameterNumber();

        DateTime? GetMinDateTimeWithMaxIdAfterId(long id, out long? maxId);

        DateTime? GetMinDateTimeWithMaxIdByFilter(RecordFilterGroup filterGroup, out long? maxId);

        long? GetMaxId(out DateTime? maxDate, out DateTime? minDate);
    }

    public interface IDataRecordDataManagerGetFilteredDataRecordsContext
    {
        DateTime FromTime { get; }

        DateTime ToTime { get; }

        RecordFilterGroup FilterGroup { get; }

        List<string> FieldNames { get; }

        int? LimitResult { get; }

        OrderDirection Direction { get; }
    }

    public interface ISummaryRecordDataManager
    {
        IEnumerable<dynamic> GetExistingSummaryRecords(DateTime batchStart);
    }

    public interface IRemoteRecordDataManager
    {
        Vanrise.Entities.IDataRetrievalResult<DataRecordDetail> GetFilteredDataRecords(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input);
    }
}