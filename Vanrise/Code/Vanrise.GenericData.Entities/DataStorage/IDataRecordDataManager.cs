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

        List<DataRecord> GetFilteredDataRecords(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input);

        void GetDataRecords(DateTime from, DateTime to, Action<dynamic> onItemReady);

        void DeleteRecords(DateTime from, DateTime to);

        void DeleteRecords(DateTime dateTime);
        bool Update(Dictionary<string, Object> fieldValues);
        bool Insert(Dictionary<string, Object> fieldValues, out object insertedId);
        List<DataRecord> GetAllDataRecords(List<string> columns);
        bool AreDataRecordsUpdated(ref object updateHandle);
    }

    public interface ISummaryRecordDataManager
    {
        void InsertSummaryRecords(IEnumerable<dynamic> records);

        void UpdateSummaryRecords(IEnumerable<dynamic> records);

        IEnumerable<dynamic> GetExistingSummaryRecords(DateTime batchStart);
    }

    public interface IRemoteRecordDataManager
    {
        Vanrise.Entities.IDataRetrievalResult<DataRecordDetail> GetFilteredDataRecords(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input);
    }
}
