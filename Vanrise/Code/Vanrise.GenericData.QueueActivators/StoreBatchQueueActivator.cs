using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.QueueActivators
{
    public class StoreBatchQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Reprocess.Entities.IReprocessStageActivator
    {
        DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
        DataStoreManager _dataStoreManager = new DataStoreManager();

        public Guid DataRecordStorageId { get; set; }


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
            if (context.QueueItem == null)
                throw new NullReferenceException("context.QueueItem");

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
            foreach (var record in batchRecords)
            {
                record.QueueItemId = context.QueueItem.ItemId;
                recordStorageDataManager.WriteRecordToStream(record as Object, dbApplyStream);
            }
            var streamReadyToApply = recordStorageDataManager.FinishDBApplyStream(dbApplyStream);
            recordStorageDataManager.ApplyStreamToDB(streamReadyToApply);
        }

        #endregion

        #region IReprocessStageActivator

        public object InitializeStage(Reprocess.Entities.IReprocessStageActivatorInitializingContext context)
        {
            return new DataRecordStorageManager().CreateTempStorage(this.DataRecordStorageId, context.ProcessId);
        }

        public void ExecuteStage(Reprocess.Entities.IReprocessStageActivatorExecutionContext context)
        {
            TempStorageInformation tempStorageInformation = context.InitializationStageOutput != null ? context.InitializationStageOutput.CastWithValidate<TempStorageInformation>("tempStorageInformation") : null;

            var recordStorageDataManager = _dataRecordStorageManager.GetStorageDataManager(this.DataRecordStorageId, tempStorageInformation);
            if (recordStorageDataManager == null)
                throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", this.DataRecordStorageId));

            Queueing.MemoryQueue<Object> queuePreparedBatchesForDBApply = new Queueing.MemoryQueue<object>();
            AsyncActivityStatus prepareBatchForDBApplyStatus = new AsyncActivityStatus();

            StartPrepareBatchForDBApplyTask(context, recordStorageDataManager, queuePreparedBatchesForDBApply, prepareBatchForDBApplyStatus);
            if (tempStorageInformation == null)
            {
                var dataRecordStorage = _dataRecordStorageManager.GetDataRecordStorage(DataRecordStorageId);
                var recordFilterGroup = context.GetRecordFilterGroup(dataRecordStorage.DataRecordTypeId);
                DeleteFromDB(context, recordStorageDataManager, recordFilterGroup);
            }
            ApplyBatchesToDB(context, recordStorageDataManager, queuePreparedBatchesForDBApply, prepareBatchForDBApplyStatus);
        }

        public void FinalizeStage(Reprocess.Entities.IReprocessStageActivatorFinalizingContext context)
        {
        }

        public int? GetStorageRowCount(Reprocess.Entities.IReprocessStageActivatorGetStorageRowCountContext context)
        {
            TempStorageInformation tempStorageInformation = null;

            if (context.InitializationStageOutput != null)
                tempStorageInformation = context.InitializationStageOutput.CastWithValidate<TempStorageInformation>("context.InitializationStageOutput");

            return new DataRecordStorageManager().GetStorageRowCount(this.DataRecordStorageId, tempStorageInformation);
        }

        public void CommitChanges(Reprocess.Entities.IReprocessStageActivatorCommitChangesContext context)
        {
            TempStorageInformation tempStorageInformation = context.InitializationStageOutput.CastWithValidate<TempStorageInformation>("context.InitializationStageOutput");
            var dataRecordStorage = _dataRecordStorageManager.GetDataRecordStorage(DataRecordStorageId);
            var recordFilterGroup = context.GetRecordFilterGroup(dataRecordStorage.DataRecordTypeId);

            new DataRecordStorageManager().FillDataRecordStorageFromTempStorage(this.DataRecordStorageId, tempStorageInformation, context.From, context.To, recordFilterGroup);
        }

        public void DropStorage(Reprocess.Entities.IReprocessStageActivatorDropStorageContext context)
        {
            TempStorageInformation tempStorageInformation = context.InitializationStageOutput.CastWithValidate<TempStorageInformation>("context.InitializationStageOutput");
            new DataRecordStorageManager().DropStorage(this.DataRecordStorageId, tempStorageInformation);
        }

        public List<string> GetOutputStages(List<string> stageNames)
        {
            return null;
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

        private void DeleteFromDB(Reprocess.Entities.IReprocessStageActivatorExecutionContext context, IDataRecordDataManager recordStorageDataManager, RecordFilterGroup recordFilterGroup)
        {
            recordStorageDataManager.DeleteRecords(context.From, context.To, recordFilterGroup);
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

        #endregion
    }
}
