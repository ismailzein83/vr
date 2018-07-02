using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IDataRecordDataManager : Vanrise.Data.IBulkApplyDataManager<Object>
    {
        void ApplyStreamToDB(object stream);

        List<DataRecord> GetFilteredDataRecords(IDataRecordDataManagerGetFilteredDataRecordsContext context);

        void GetDataRecords(DateTime? from, DateTime? to, RecordFilterGroup recordFilterGroup, Func<bool> shouldStop, Action<dynamic> onItemReady);

        void DeleteRecords(DateTime from, DateTime to, RecordFilterGroup recordFilterGroup);

        void DeleteRecords(DateTime dateTime, RecordFilterGroup recordFilterGroup);

        bool Insert(Dictionary<string, Object> fieldValues, int? createdUserId, int? modifiedUserId, out object insertedId);

        void InsertRecords(IEnumerable<dynamic> records);

        bool Update(Dictionary<string, Object> fieldValues, int? modifiedUserId);

        void UpdateRecords(IEnumerable<dynamic> records, List<string> fieldsToJoin, List<string> fieldsToUpdate);

        List<DataRecord> GetAllDataRecords(List<string> columns);

        bool AreDataRecordsUpdated(ref object updateHandle);

        int GetDBQueryMaxParameterNumber();
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