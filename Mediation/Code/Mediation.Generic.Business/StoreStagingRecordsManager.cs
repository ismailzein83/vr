using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Data;
using Mediation.Generic.Entities;
using Vanrise.GenericData.Business;

namespace Mediation.Generic.Business
{
    public class StoreStagingRecordsManager
    {
        public IEnumerable<StoreStagingRecord> GetStoreStagingRecords()
        {
            IStoreStagingRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IStoreStagingRecordsDataManager>();
            return dataManager.GetStoreStagingRecords();
        }
        public IEnumerable<StoreStagingRecord> GetStoreStagingRecordsByStatus(EventStatus status)
        {
            IStoreStagingRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IStoreStagingRecordsDataManager>();
            return dataManager.GetStoreStagingRecordsByStatus(status);
        }

        public IEnumerable<StoreStagingRecord> GetStoreStagingRecordsByStatus(EventStatus status, int dataRecordTypeId)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            Type recordType = dataRecordTypeManager.GetDataRecordRuntimeType(dataRecordTypeId);
            IStoreStagingRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IStoreStagingRecordsDataManager>();
            return dataManager.GetStoreStagingRecordsByStatus(status);
        }
        public IEnumerable<StoreStagingRecord> GetStoreStagingRecordsByIds(IEnumerable<int> eventIds)
        {
            IStoreStagingRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IStoreStagingRecordsDataManager>();
            return dataManager.GetStoreStagingRecordsByIds(eventIds);
        }

    }
}
