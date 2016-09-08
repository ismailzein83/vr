using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public class PreparePreviewZonesServicesForDBApplyInput
    {
        public BaseQueue<IEnumerable<ZoneServicePreview>> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PreparePreviewZonesServicesForDBApply : DependentAsyncActivity<PreparePreviewZonesServicesForDBApplyInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ZoneServicePreview>>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }


        protected override void DoWork(PreparePreviewZonesServicesForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISupplierZoneServicePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZoneServicePreviewDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, ZoneServicePreview => ZoneServicePreview);
        }

        protected override PreparePreviewZonesServicesForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PreparePreviewZonesServicesForDBApplyInput()
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
