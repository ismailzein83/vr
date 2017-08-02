using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;

namespace Vanrise.GenericData.QueueActivators
{
    public class DistributeBatchQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Reprocess.Entities.IReprocessStageActivator
    {
        public List<string> OutputStages { get; set; }

        #region QueueActivator

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

            if (batchRecords != null && batchRecords.Count > 0)
            {
                DataRecordBatch transformedBatch = DataRecordBatch.CreateBatchFromRecords(batchRecords, queueItemType.BatchDescription, recordTypeId);
                if (this.OutputStages != null)
                {
                    foreach (var stageName in this.OutputStages)
                    {
                        context.OutputItems.Add(stageName, transformedBatch);
                    }
                }
            }
        }

        #endregion

        #region IReprocessStageActivator

        public object InitializeStage(Reprocess.Entities.IReprocessStageActivatorInitializingContext context)
        {
            return null;
        }

        public void ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {
            context.DoWhilePreviousRunning(() =>
            {
                bool hasItem = false;
                List<string> validStages = GetOutputStages(context.StageNames);
                do
                {
                    hasItem = context.InputQueue.TryDequeue((reprocessBatch) =>
                    {
                        Reprocess.Entities.GenericDataRecordBatch genericDataRecordBatch = reprocessBatch as Reprocess.Entities.GenericDataRecordBatch;
                        if (genericDataRecordBatch == null)
                            throw new Exception(String.Format("reprocessBatch should be of type 'Reprocess.Entities.GenericDataRecordBatch'. and not of type '{0}'", reprocessBatch.GetType()));

                        if (this.OutputStages != null)
                        {
                            foreach (var stageName in this.OutputStages)
                            {
                                if (validStages != null && validStages.Contains(stageName))
                                    context.EnqueueBatch(stageName, genericDataRecordBatch);
                            }
                        }
                    });
                } while (!context.ShouldStop() && hasItem);
            });
        }

        public void FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {

        }

        public int? GetStorageRowCount(Reprocess.Entities.IReprocessStageActivatorGetStorageRowCountContext context)
        {
            return null;
        }

        public void CommitChanges(Reprocess.Entities.IReprocessStageActivatorCommitChangesContext context)
        {
        }

        public void DropStorage(Reprocess.Entities.IReprocessStageActivatorDropStorageContext context)
        {
        }

        public List<string> GetOutputStages(List<string> stageNames)
        {
            if (OutputStages == null)
                return null;

            if (stageNames == null)
                return null;

            Func<string, bool> filterExpression = (itemObject) => stageNames.Contains(itemObject);

            IEnumerable<string> filteredStages = OutputStages.FindAllRecords(filterExpression);
            return filteredStages != null ? filteredStages.ToList() : null;
        }

        public Queueing.BaseQueue<Reprocess.Entities.IReprocessBatch> GetQueue()
        {
            return new Queueing.MemoryQueue<Reprocess.Entities.IReprocessBatch>();
        }

        public List<Reprocess.Entities.BatchRecord> GetStageBatchRecords(Reprocess.Entities.IReprocessStageActivatorPreparingContext context)
        {
            return null;
        }

        #endregion

        #region Private Methods



        #endregion
    }
}
