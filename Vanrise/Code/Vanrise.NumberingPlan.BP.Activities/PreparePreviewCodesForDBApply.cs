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

    public class PreparePreviewCodesForApplyInput
    {
        public BaseQueue<IEnumerable<CodePreview>> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PreparePreviewCodesForDBApply : DependentAsyncActivity<PreparePreviewCodesForApplyInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<CodePreview>>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }


        protected override void DoWork(PreparePreviewCodesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISaleCodePreviewDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleCodePreviewDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, ZonePreviewList => ZonePreviewList);
        }

        protected override PreparePreviewCodesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PreparePreviewCodesForApplyInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
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
