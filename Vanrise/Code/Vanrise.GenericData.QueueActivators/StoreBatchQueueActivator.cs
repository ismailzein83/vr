using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.QueueActivators
{
    public class StoreBatchQueueActivator : Vanrise.Queueing.Entities.QueueActivator
    {
        public int DataRecordStorageId { get; set; }

        DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
        DataStoreManager _dataStoreManager = new DataStoreManager();

        public override void OnDisposed()
        {
            throw new NotImplementedException();
        }

        public override void ProcessItem(Queueing.Entities.PersistentQueueItem item, Queueing.Entities.ItemsToEnqueue outputItems)
        {
            throw new NotImplementedException();
        }

        public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);
            
            var recordStorageDataManager = _dataRecordStorageManager.GetStorageDataManager(this.DataRecordStorageId);
            if (recordStorageDataManager == null)
                throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", this.DataRecordStorageId));
            var dbApplyStream = recordStorageDataManager.InitialiazeStreamForDBApply();
            foreach(var record in batchRecords)
            {
                recordStorageDataManager.WriteRecordToStream(record as Object, dbApplyStream);
            }
            var streamReadyToApply = recordStorageDataManager.FinishDBApplyStream(dbApplyStream);
            recordStorageDataManager.ApplyStreamToDB(streamReadyToApply);
        }
    }
}
