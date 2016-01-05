using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Argument Classes
    public class PrepareDWCDRsForDBApplyInput
    {
        public BaseQueue<DWCDRBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion
    public sealed class PrepareDWCDRsForDBApply : DependentAsyncActivity<PrepareDWCDRsForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<DWCDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareDWCDRsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IDWCDRDataManager dataManager = FraudDataManagerFactory.GetDataManager<IDWCDRDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, dwCDRsBatch => dwCDRsBatch.DWCDRs);
        }

        protected override PrepareDWCDRsForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareDWCDRsForDBApplyInput
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
