using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.CodePreparation.Data;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class PrepareChangedCodesForApplyInput
    {
        public BaseQueue<IEnumerable<ChangedCode>> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }
    public sealed class PrepareChangedCodesForApply : DependentAsyncActivity<PrepareChangedCodesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ChangedCode>>> InputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareChangedCodesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IChangedSaleCodeDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedSaleCodeDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, ChangedCodesList => ChangedCodesList);
        }

        protected override PrepareChangedCodesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareChangedCodesForApplyInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<Object>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
