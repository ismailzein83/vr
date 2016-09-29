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
    public class MediationRecordsManager
    {
        public IEnumerable<MediationRecord> GetMediationRecords()
        {
            IMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationRecordsDataManager>();
            return dataManager.GetMediationRecords();
        }

        public IEnumerable<MediationRecord> GetMediationRecordsByStatus(int mediationDefinitionId, EventStatus status, Guid dataRecordTypeId)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            Type recordType = dataRecordTypeManager.GetDataRecordRuntimeType(dataRecordTypeId);
            IMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationRecordsDataManager>();
            dataManager.DataRecordTypeId = dataRecordTypeId;
            return dataManager.GetMediationRecordsByStatus(mediationDefinitionId, status);
        }
        public IEnumerable<MediationRecord> GetMediationRecordsByIds(int mediationDefinitionId, IEnumerable<string> sessionIds, Guid dataRecordTypeId)
        {
            IMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationRecordsDataManager>();
            dataManager.DataRecordTypeId = dataRecordTypeId;
            return dataManager.GetMediationRecordsByIds(mediationDefinitionId, sessionIds);
        }

    }
}
