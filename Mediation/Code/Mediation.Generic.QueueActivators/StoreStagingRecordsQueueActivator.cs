using System;
using System.Collections.Generic;
using System.Linq;
using Mediation.Generic.Business;
using Mediation.Generic.Data;
using Mediation.Generic.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.QueueActivators;
using Vanrise.Queueing.Entities;

namespace Mediation.Generic.QueueActivators
{
    public class StoreStagingRecordsQueueActivator : QueueActivator
    {
        DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
        DataStoreManager _dataStoreManager = new DataStoreManager();

        public Guid MediationDefinitionId { get; set; }

        public override void ProcessItem(IQueueActivatorExecutionContext context)
        {
            MediationRecordsManager mediationRecordsManager = new MediationRecordsManager();
            MediationDefinitionManager mediationManager = new MediationDefinitionManager();
            MediationDefinition mediationDefinition = mediationManager.GetMediationDefinition(MediationDefinitionId);
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);
            List<MediationRecord> mediationRecords = mediationRecordsManager.GenerateMediationRecordsFromBatchRecords(mediationDefinition, recordTypeId, batchRecords);

            mediationRecords = CheckBadMediationRecords(mediationDefinition, mediationRecords);

            mediationRecordsManager.SaveMediationRecordsToDB(MediationDefinitionId, mediationRecords);
        }

        List<MediationRecord> CheckBadMediationRecords(MediationDefinition mediationDefinition, List<MediationRecord> mediationRecords)
        {
            if (mediationDefinition.BadCDRIdentificationSettings != null && mediationDefinition.BadCDRIdentificationSettings.BadCDRFilterGroup != null)
            {
                List<BadRecord> badRecords = new List<BadRecord>();
                List<MediationRecord> validRecords = new List<MediationRecord>();
                RecordFilterManager recordFilterManager = new RecordFilterManager();
                foreach (var mediationRecord in mediationRecords)
                {
                    DataRecordFilterGenericFieldMatchContext filterContext = new DataRecordFilterGenericFieldMatchContext(mediationRecord.EventDetails, mediationDefinition.ParsedRecordTypeId);
                    if (recordFilterManager.IsFilterGroupMatch(mediationDefinition.BadCDRIdentificationSettings.BadCDRFilterGroup, filterContext))
                        badRecords.Add(GetBadRecord(mediationRecord));
                    else
                    {
                        validRecords.Add(mediationRecord);
                    }
                }
                BadMediationRecordsManager badRecordsManager = new BadMediationRecordsManager();
                badRecordsManager.SaveMediationRecordsToDB(mediationDefinition.MediationDefinitionId, badRecords);

                return validRecords;
            }
            else
            {
                return mediationRecords;
            }
        }

        private BadRecord GetBadRecord(MediationRecord mediationRecord)
        {
            return new BadRecord
            {
                EventDetails = mediationRecord.EventDetails,
                EventStatus = mediationRecord.EventStatus,
                EventTime = mediationRecord.EventTime,
                MediationDefinitionId = mediationRecord.MediationDefinitionId,
                SessionId = mediationRecord.SessionId
            };
        }
    }
}
