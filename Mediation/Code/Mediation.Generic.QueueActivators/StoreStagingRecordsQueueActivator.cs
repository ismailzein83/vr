using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.QueueActivators;
using Vanrise.Queueing.Entities;

namespace Mediation.Generic.QueueActivators
{
    public class StoreStagingRecordsQueueActivator : QueueActivator
    {
        DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
        DataStoreManager _dataStoreManager = new DataStoreManager();
        public string SessionRecordTypeId { get; set; }
        public string EventTimeRecordTypeId { get; set; }
        public List<StatusMapping> StatusMappings { get; set; }

        public override void ProcessItem(IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);


        }
    }
}
