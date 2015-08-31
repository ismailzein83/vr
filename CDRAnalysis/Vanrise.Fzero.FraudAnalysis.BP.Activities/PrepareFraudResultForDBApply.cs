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
    public class PrepareFraudResultForDBApplyInput
    {
        public BaseQueue<SuspiciousNumberBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion
    public sealed class PrepareFraudResultForDBApply : DependentAsyncActivity<PrepareFraudResultForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<SuspiciousNumberBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareFraudResultForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IFraudResultDataManager dataManager = FraudDataManagerFactory.GetDataManager<IFraudResultDataManager>();
           
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, suspicousNumberBatch => suspicousNumberBatch.SuspiciousNumbers);
        }

        protected override PrepareFraudResultForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareFraudResultForDBApplyInput
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
