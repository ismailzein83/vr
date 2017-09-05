using Mediation.Generic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;
using Vanrise.Entities;

namespace Mediation.Generic.MainExtensions.MediationOutputHandlers
{
    public class StoreRecordsOutputHandler : MediationOutputHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("14F0A218-77B5-4417-AB80-6A9386A7BB49"); }
        }

        public Guid DataRecordStorageId { get; set; }
        static DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
        public override void Execute(IMediationOutputHandlerContext context)
        {
            var recordStorageDataManager = _dataRecordStorageManager.GetStorageDataManager(this.DataRecordStorageId);
            if (recordStorageDataManager == null)
                throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", this.DataRecordStorageId));

            MemoryQueue<PreparedBatchForDBApply> queuePreparedBatchesForDBApply = new MemoryQueue<PreparedBatchForDBApply>();
            AsyncActivityStatus prepareBatchForDBApplyStatus = new AsyncActivityStatus();

            StartPrepareBatchForDBApplyTask(context, recordStorageDataManager, queuePreparedBatchesForDBApply, prepareBatchForDBApplyStatus);
            ApplyBatchesToDB(context, recordStorageDataManager, queuePreparedBatchesForDBApply, prepareBatchForDBApplyStatus);
        }

        private static void StartPrepareBatchForDBApplyTask(IMediationOutputHandlerContext context, IDataRecordDataManager recordStorageDataManager, MemoryQueue<PreparedBatchForDBApply> queuePreparedBatchesForDBApply, AsyncActivityStatus prepareBatchForDBApplyStatus)
        {
            Task prepareDataTask = new Task(() =>
            {                
                try
                {                    
                    context.DoWhilePreviousRunning(() =>
                    {
                        bool hasItems = false;
                        do
                        {
                            hasItems = context.InputQueue.TryDequeue(
                            (inputBatch) =>
                            {
                                object dbApplyStream = recordStorageDataManager.InitialiazeStreamForDBApply();
                                foreach (Object item in inputBatch.BatchRecords)
                                {
                                    recordStorageDataManager.WriteRecordToStream(item, dbApplyStream);
                                }

                                Object preparedItemsForDBApply = recordStorageDataManager.FinishDBApplyStream(dbApplyStream);
                                queuePreparedBatchesForDBApply.Enqueue(
                                    new PreparedBatchForDBApply
                                    {
                                        OriginalBatch = inputBatch,
                                        BatchToApply = preparedItemsForDBApply
                                    });

                            });
                        } while (!context.ShouldStop() && hasItems);
                    });

                    //context.PrepareDataForDBApply(recordStorageDataManager, context.InputQueue, queuePreparedBatchesForDBApply, CDRsBatch => CDRsBatch.BatchRecords.Cast<Object>());
                }
                finally
                {
                    prepareBatchForDBApplyStatus.IsComplete = true;
                }
            });
            prepareDataTask.Start();
        }


        private void ApplyBatchesToDB(IMediationOutputHandlerContext context, IDataRecordDataManager recordStorageDataManager, MemoryQueue<PreparedBatchForDBApply> queuePreparedBatchesForDBApply, AsyncActivityStatus prepareBatchForDBApplyStatus)
        {
            context.DoWhilePreviousRunning(prepareBatchForDBApplyStatus, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = queuePreparedBatchesForDBApply.TryDequeue((preparedBatchForDBApply) =>
                    {                        
                        recordStorageDataManager.ApplyStreamToDB(preparedBatchForDBApply.BatchToApply);
                        context.SetOutputHandlerExecutedOnBatch(preparedBatchForDBApply.OriginalBatch);
                    });
                } while (!context.ShouldStop() && hasItem);
            });
        }

        private class PreparedBatchForDBApply
        {
            public PreparedRecordsBatch OriginalBatch { get; set; }

            public Object BatchToApply { get; set; }
        }
    }
}
