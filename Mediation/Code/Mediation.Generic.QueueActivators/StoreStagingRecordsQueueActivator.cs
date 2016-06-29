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

        public int MediationDefinitionId { get; set; }

        public override void ProcessItem(IQueueActivatorExecutionContext context)
        {
            IMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationRecordsDataManager>();

            MediationDefinitionManager mediationManager = new MediationDefinitionManager();
            MediationDefinition mediationDefinition = mediationManager.GetMediationDefinition(MediationDefinitionId);
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            RecordFilterManager filterManager = new RecordFilterManager();

            List<MediationRecord> mediationRecords = new List<MediationRecord>();
            foreach (var batchRecord in batchRecords)
            {
                DataRecordFilterGenericFieldMatchContext dataRecordFilterContext = new DataRecordFilterGenericFieldMatchContext(batchRecord, recordTypeId);
                MediationRecord mediationRecord = new MediationRecord();
                mediationRecord.SessionId = (long)GetPropertyValue(batchRecord, mediationDefinition.ParsedRecordIdentificationSetting.SessionIdField);
                mediationRecord.EventTime = (DateTime)GetPropertyValue(batchRecord, mediationDefinition.ParsedRecordIdentificationSetting.EventTimeField);
                foreach (var statusMapping in mediationDefinition.ParsedRecordIdentificationSetting.StatusMappings)
                {
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
            dataManager.SaveMediationRecordsToDB(mediationRecords);
        }

        object GetPropertyValue(object batchRecord, string propertyName)
        {
            var reader = Vanrise.Common.Utilities.GetPropValueReader(propertyName);
            return reader.GetPropertyValue(batchRecord);
        }
    }
}
