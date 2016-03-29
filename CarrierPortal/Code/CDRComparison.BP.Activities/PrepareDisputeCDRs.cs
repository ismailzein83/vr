using CDRComparison.Data;
using CDRComparison.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace CDRComparison.BP.Activities
{
    #region Argument Classes
    public class PrepareDisputeCDRsInput
    {
        public BaseQueue<DisputeCDRBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion
    public class PrepareDisputeCDRs : DependentAsyncActivity<PrepareDisputeCDRsInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<DisputeCDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }
        protected override void DoWork(PrepareDisputeCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IDisputeCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IDisputeCDRDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, disputeCDRBatch => disputeCDRBatch.DisputeCDRs);
        }

        protected override PrepareDisputeCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareDisputeCDRsInput
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
