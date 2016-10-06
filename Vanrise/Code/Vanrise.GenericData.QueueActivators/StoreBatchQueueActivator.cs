using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.QueueActivators
{
    public class StoreBatchQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Reprocess.Entities.IReprocessStageActivator
    {
        public Guid DataRecordStorageId { get; set; }

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


        void Reprocess.Entities.IReprocessStageActivator.ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {
            var recordStorageDataManager = _dataRecordStorageManager.GetStorageDataManager(this.DataRecordStorageId);
            if (recordStorageDataManager == null)
                throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", this.DataRecordStorageId));
            Queueing.MemoryQueue<Object> queuePreparedBatchesForDBApply = new Queueing.MemoryQueue<object>();
            AsyncActivityStatus prepareBatchForDBApplyStatus = new AsyncActivityStatus();
            StartPrepareBatchForDBApplyTask(context, recordStorageDataManager, queuePreparedBatchesForDBApply, prepareBatchForDBApplyStatus);
            
            DeleteFromDB(context, recordStorageDataManager);
            ApplyBatchesToDB(context, recordStorageDataManager, queuePreparedBatchesForDBApply, prepareBatchForDBApplyStatus);
        }

        private void DeleteFromDB(Reprocess.Entities.IReprocessStageActivatorExecutionContext context, IDataRecordDataManager recordStorageDataManager)
        {
            recordStorageDataManager.DeleteRecords(context.From, context.To);
        }

        private static void StartPrepareBatchForDBApplyTask(Reprocess.Entities.IReprocessStageActivatorExecutionContext context, IDataRecordDataManager recordStorageDataManager, Queueing.MemoryQueue<Object> queuePreparedBatchesForDBApply, AsyncActivityStatus prepareBatchForDBApplyStatus)
        {
            Task prepareDataTask = new Task(() =>
            {
                try
                {
                    context.DoWhilePreviousRunning(() =>
                    {
                        bool hasItem = false;
                        do
                        {
                            hasItem = context.InputQueue.TryDequeue((reprocessBatch) =>
                            {
                                Reprocess.Entities.GenericDataRecordBatch genericDataRecordBatch = reprocessBatch as Reprocess.Entities.GenericDataRecordBatch;
                                if (genericDataRecordBatch == null)
                                    throw new Exception(String.Format("reprocessBatch should be of type 'Reprocess.Entities.GenericDataRecordBatch'. and not of type '{0}'", reprocessBatch.GetType()));

                                var dbApplyStream = recordStorageDataManager.InitialiazeStreamForDBApply();
                                foreach (var record in genericDataRecordBatch.Records)
                                {
                                    recordStorageDataManager.WriteRecordToStream(record as Object, dbApplyStream);
                                }
                                var streamReadyToApply = recordStorageDataManager.FinishDBApplyStream(dbApplyStream);
                                queuePreparedBatchesForDBApply.Enqueue(streamReadyToApply);
                            });
                        } while (!context.ShouldStop() && hasItem);
                    });
                }
                finally
                {
                    prepareBatchForDBApplyStatus.IsComplete = true;
                }
            });
            prepareDataTask.Start();
        }

        private void ApplyBatchesToDB(Reprocess.Entities.IReprocessStageActivatorExecutionContext context, IDataRecordDataManager recordStorageDataManager, Queueing.MemoryQueue<object> queuePreparedBatchesForDBApply, AsyncActivityStatus prepareBatchForDBApplyStatus)
        {
            context.DoWhilePreviousRunning(prepareBatchForDBApplyStatus, () =>
                   {
                       bool hasItem = false;
                       do
                       {
                           hasItem = queuePreparedBatchesForDBApply.TryDequeue((preparedBatchForDBApply) =>
                           {
                               recordStorageDataManager.ApplyStreamToDB(preparedBatchForDBApply);
                           });
                       } while (!context.ShouldStop() && hasItem);
                   });
        }

        void Reprocess.Entities.IReprocessStageActivator.FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {
            
        }

        List<string> Reprocess.Entities.IReprocessStageActivator.GetOutputStages(List<string> stageNames)
        {
            return null;
        }

        Queueing.BaseQueue<Reprocess.Entities.IReprocessBatch> Reprocess.Entities.IReprocessStageActivator.GetQueue()
        {
            return new Queueing.MemoryQueue<Reprocess.Entities.IReprocessBatch>();
        }


        public List<Reprocess.Entities.BatchRecord> GetStageBatchRecords(Reprocess.Entities.IReprocessStageActivatorPreparingContext context)
        {
            return null;
        }
    }
}
