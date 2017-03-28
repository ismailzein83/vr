using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.QueueActivators;

namespace Vanrise.GenericData.Notification
{
    public class CheckActionRulesQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Reprocess.Entities.IReprocessStageActivator
    {
        public Guid ActionRuleTypeId { get; set; }


        #region QueueActivator

        public override void OnDisposed()
        {
        }

        public override void ProcessItem(Queueing.Entities.PersistentQueueItem item, Queueing.Entities.ItemsToEnqueue outputItems)
        {
        }

        public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            foreach (var record in batchRecords)
            {

            }
        }

        #endregion

        #region IReprocessStageActivator

        public void ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {

        }

        public void FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {

        }

        public List<string> GetOutputStages(List<string> stageNames)
        {
            return null;
        }

        public Queueing.BaseQueue<Reprocess.Entities.IReprocessBatch> GetQueue()
        {
            return null;

        }

        public List<Reprocess.Entities.BatchRecord> GetStageBatchRecords(Reprocess.Entities.IReprocessStageActivatorPreparingContext context)
        {
            return null;
        }

        #endregion
    }
}
