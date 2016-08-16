using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Data;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public class PreparePreviewRatesForDBApplyInput
    {
        public BaseQueue<IEnumerable<RatePreview>> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PreparePreviewRatesForDBApply : DependentAsyncActivity<PreparePreviewRatesForDBApplyInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<RatePreview>>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }


        protected override void DoWork(PreparePreviewRatesForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISaleRatePreviewDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleRatePreviewDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, RatePreviewList => RatePreviewList);
        }

        protected override PreparePreviewRatesForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PreparePreviewRatesForDBApplyInput()
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
