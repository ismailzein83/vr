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
        public void SaveMediationRecordsToDB(List<MediationRecord> mediationRecords)
        {
            IMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationRecordsDataManager>();
            dataManager.SaveMediationRecordsToDB(mediationRecords);
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
