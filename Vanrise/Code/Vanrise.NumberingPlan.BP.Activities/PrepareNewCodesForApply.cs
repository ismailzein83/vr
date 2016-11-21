using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.NumberingPlan.Entities;
using Vanrise.NumberingPlan.Data;

namespace Vanrise.NumberingPlan.BP.Activities
{
    public class PrepareNewCodesForApplyInput
    {
        public BaseQueue<IEnumerable<AddedCode>> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }
    public sealed class PrepareNewCodesForApply : DependentAsyncActivity<PrepareNewCodesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<AddedCode>>> InputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareNewCodesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INewSaleCodeDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleCodeDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, NewCodesList => NewCodesList);
        }

        protected override PrepareNewCodesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareNewCodesForApplyInput()
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
