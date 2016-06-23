using System;
using System.Collections.Generic;
using Mediation.Generic.Entities;
using Vanrise.Data;

namespace Mediation.Generic.Data
{
    public interface IStoreStagingRecordsDataManager : IDataManager, IBulkApplyDataManager<StoreStagingRecord>
    {
        void SaveStoreStagingRecordsToDB(List<StoreStagingRecord> storeStagingRecords);

        IEnumerable<StoreStagingRecord> GetStoreStagingRecords();

        IEnumerable<StoreStagingRecord> GetStoreStagingRecordsByStatus(EventStatus status);

        IEnumerable<StoreStagingRecord> GetStoreStagingRecordsByIds(IEnumerable<int> eventIds);

        int DataRecordTypeId { set; }
    }
}
