using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class PreparePreviewZonesRoutingProductsForDBApplyInput
    {
        public BaseQueue<IEnumerable<ZoneRoutingProductPreview>> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }
    public sealed class PreparePreviewZonesRoutingProductsForDBApply : DependentAsyncActivity<PreparePreviewZonesRoutingProductsForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ZoneRoutingProductPreview>>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PreparePreviewZonesRoutingProductsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISaleZoneRoutingProductPreviewDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleZoneRoutingProductPreviewDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, ZoneRoutingProductPreviewList => ZoneRoutingProductPreviewList);
        }

        protected override PreparePreviewZonesRoutingProductsForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PreparePreviewZonesRoutingProductsForDBApplyInput()
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
