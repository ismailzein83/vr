using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.CDRImport.BP.Activities
{
    #region Argument Classes
    public class PrepareUnifiedCDRsForDBApplyInput
    {
        public BaseQueue<CDRBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion
    public sealed class PrepareUnifiedCDRsForDBApply : DependentAsyncActivity<PrepareUnifiedCDRsForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareUnifiedCDRsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            //PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, cdrsBatch => cdrsBatch.CDRBatchFilePath);
        }

        protected override PrepareUnifiedCDRsForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareUnifiedCDRsForDBApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<object>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
