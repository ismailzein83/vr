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

        public void GetMediationRecordsByStatus(Guid mediationDefinitionId, EventStatus status, Guid dataRecordTypeId, long lastCommittedId, Action<string> onSessionIdLoaded)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            IMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationRecordsDataManager>();
            dataManager.DataRecordTypeId = dataRecordTypeId;
            dataManager.GetMediationRecordsByStatus(mediationDefinitionId, status, lastCommittedId, onSessionIdLoaded);
        }
        public IEnumerable<MediationRecord> GetMediationRecordsByIds(Guid mediationDefinitionId, IEnumerable<string> sessionIds, Guid dataRecordTypeId, long lastCommittedId)
        {
            IMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationRecordsDataManager>();
            dataManager.DataRecordTypeId = dataRecordTypeId;
            return dataManager.GetMediationRecordsByIds(mediationDefinitionId, sessionIds, lastCommittedId);
        }
        public void SaveMediationRecordsToDB(Guid mediationDefinitionId, List<MediationRecord> mediationRecords)
        {
            long startingId;
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(typeof(Mediation.Generic.Entities.MediationRecord), mediationRecords.Count, out startingId);
            foreach (var r in mediationRecords)
            {
                r.EventId = startingId++;
            }
            IMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationRecordsDataManager>();
            dataManager.SaveMediationRecordsToDB(mediationRecords);

            MediationCommittedIdManager mediationCommittedIdManager = new MediationCommittedIdManager();
            mediationCommittedIdManager.InsertOrUpdateLastCommitedId(mediationDefinitionId, startingId);
        }
        public List<MediationRecord> GenerateMediationRecordsFromBatchRecords(MediationDefinition mediationDefinition, Guid recordTypeId, List<dynamic> batchRecords)
        {
            List<MediationRecord> mediationRecords = new List<MediationRecord>();
            foreach (var batchRecord in batchRecords)
            {
                if (batchRecord == null)
                    continue;
                DataRecordFilterGenericFieldMatchContext dataRecordFilterContext = new DataRecordFilterGenericFieldMatchContext(batchRecord, recordTypeId);
                MediationRecord mediationRecord = new MediationRecord();
                mediationRecord.SessionId = GetPropertyValue(batchRecord, mediationDefinition.ParsedRecordIdentificationSetting.SessionIdField) as string;
                mediationRecord.EventTime = (DateTime)GetPropertyValue(batchRecord, mediationDefinition.ParsedRecordIdentificationSetting.EventTimeField);
                foreach (var statusMapping in mediationDefinition.ParsedRecordIdentificationSetting.StatusMappings)
                {
                    RecordFilterManager filterManager = new RecordFilterManager();
                    if (filterManager.IsFilterGroupMatch(statusMapping.FilterGroup, dataRecordFilterContext))
                    {
                        mediationRecord.EventStatus = statusMapping.Status;
                        break;
                    }
                }
                mediationRecord.EventDetails = batchRecord;
                mediationRecord.MediationDefinitionId = mediationDefinition.MediationDefinitionId;
                mediationRecords.Add(mediationRecord);
            }
            return mediationRecords;
        }
        object GetPropertyValue(object batchRecord, string propertyName)
        {
            var reader = Vanrise.Common.Utilities.GetPropValueReader(propertyName);
            return reader.GetPropertyValue(batchRecord);
        }
    }
}
