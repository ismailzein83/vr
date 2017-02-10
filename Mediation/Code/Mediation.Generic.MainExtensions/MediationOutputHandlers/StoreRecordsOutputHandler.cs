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

            MemoryQueue<Object> queuePreparedBatchesForDBApply = new MemoryQueue<object>();
            AsyncActivityStatus prepareBatchForDBApplyStatus = new AsyncActivityStatus();

            StartPrepareBatchForDBApplyTask(context, recordStorageDataManager, queuePreparedBatchesForDBApply, prepareBatchForDBApplyStatus);
            ApplyBatchesToDB(context, recordStorageDataManager, queuePreparedBatchesForDBApply, prepareBatchForDBApplyStatus);
        }

        private static void StartPrepareBatchForDBApplyTask(IMediationOutputHandlerContext context, IDataRecordDataManager recordStorageDataManager, MemoryQueue<Object> queuePreparedBatchesForDBApply, AsyncActivityStatus prepareBatchForDBApplyStatus)
        {
            Task prepareDataTask = new Task(() =>
            {                
                try
                {
                    context.PrepareDataForDBApply(recordStorageDataManager, context.InputQueue, queuePreparedBatchesForDBApply, CDRsBatch => CDRsBatch.BatchRecords.Cast<Object>());
                }
                finally
                {
                    prepareBatchForDBApplyStatus.IsComplete = true;
                }
            });
            prepareDataTask.Start();
        }
       

        private void ApplyBatchesToDB(IMediationOutputHandlerContext context, IDataRecordDataManager recordStorageDataManager, MemoryQueue<object> queuePreparedBatchesForDBApply, AsyncActivityStatus prepareBatchForDBApplyStatus)
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
    }
}
