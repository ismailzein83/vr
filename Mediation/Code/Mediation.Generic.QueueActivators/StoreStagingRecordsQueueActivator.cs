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
            mediationRecordsManager.SaveMediationRecordsToDB(mediationRecords);
        }
    }
}
