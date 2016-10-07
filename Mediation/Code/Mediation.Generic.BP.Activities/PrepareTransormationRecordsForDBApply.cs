using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Mediation.Generic.BP.Activities
{
    public class PrepareTransormationRecordsForDBApplyInput
    {
        public Guid DataRecordStorageId { get; set; }
        public BaseQueue<PreparedCdrBatch> PreparedCdrBatch { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }
    public sealed class PrepareTransormationRecordsForDBApply : DependentAsyncActivity<PrepareTransormationRecordsForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<Guid> DataRecordStorageId { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<PreparedCdrBatch>> PreparedCdrBatch { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareTransormationRecordsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
            IDataRecordDataManager recordStorageDataManager = dataRecordStorageManager.GetStorageDataManager(inputArgument.DataRecordStorageId);
            if (recordStorageDataManager == null)
                throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", inputArgument.DataRecordStorageId));

            PrepareDataForDBApply(previousActivityStatus, handle, recordStorageDataManager, inputArgument.PreparedCdrBatch, inputArgument.OutputQueue, CDRsBatch => CDRsBatch.Cdrs.Cast<Object>());
        }
        
        protected override PrepareTransormationRecordsForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareTransormationRecordsForDBApplyInput()
            {
                OutputQueue = this.OutputQueue.Get(context),
                PreparedCdrBatch = this.PreparedCdrBatch.Get(context),
                DataRecordStorageId = this.DataRecordStorageId.Get(context)
            };
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.PreparedCdrBatch.Get(context) == null)
                this.PreparedCdrBatch.Set(context, new MemoryQueue<PreparedCdrBatch>());
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<Object>());
            base.OnBeforeExecute(context, handle);
        }



    }
}
