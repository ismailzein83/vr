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
    }

    public interface ISummaryRecordDataManager
    {
        void InsertSummaryRecords(List<dynamic> records);

        void UpdateSummaryRecords(List<dynamic> records);

        List<dynamic> GetExistingSummaryRecords(DateTime batchStart, DateTime batchEnd);
    }
}
