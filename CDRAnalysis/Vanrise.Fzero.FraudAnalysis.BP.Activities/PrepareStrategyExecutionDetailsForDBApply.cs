using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Argument Classes
    public class PrepareStrategyExecutionItemForDBApplyInput
    {
        public BaseQueue<StrategyExecutionItemBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion
    public sealed class PrepareStrategyExecutionItemForDBApply : DependentAsyncActivity<PrepareStrategyExecutionItemForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<StrategyExecutionItemBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareStrategyExecutionItemForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IStrategyExecutionDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();

            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, strategyExecutionDetailBatch => strategyExecutionDetailBatch.StrategyExecutionItem);
        }

        protected override PrepareStrategyExecutionItemForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareStrategyExecutionItemForDBApplyInput
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
