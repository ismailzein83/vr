using System;
using System.Activities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class PrepareModifiedCustomerRoutesPreviewForApplyInput
    {
        public BaseQueue<PartialCustomerRoutesPreviewBatch> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareModifiedCustomerRoutesPreviewForApply : DependentAsyncActivity<PrepareModifiedCustomerRoutesPreviewForApplyInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<PartialCustomerRoutesPreviewBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareModifiedCustomerRoutesPreviewForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IModifiedCustomerRoutePreviewDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IModifiedCustomerRoutePreviewDataManager>();

            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, PreviewRoutesBatch => PreviewRoutesBatch.AffectedPartialCustomerRoutesPreview);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Modified Customer Routes Preview For Apply is done", null);
        }

        protected override PrepareModifiedCustomerRoutesPreviewForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareModifiedCustomerRoutesPreviewForApplyInput
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