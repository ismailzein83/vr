using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Data;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Argument Classes
    public class PrepareStrategyExecutionDetailsForDBApplyInput
    {
        public BaseQueue<StrategyExecutionDetailBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion
    public sealed class PrepareStrategyExecutionDetailsForDBApply : DependentAsyncActivity<PrepareStrategyExecutionDetailsForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<StrategyExecutionDetailBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareStrategyExecutionDetailsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IStrategyExecutionDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();

            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, strategyExecutionDetailBatch => strategyExecutionDetailBatch.StrategyExecutionDetails);
        }

        protected override PrepareStrategyExecutionDetailsForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareStrategyExecutionDetailsForDBApplyInput
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
