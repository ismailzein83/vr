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

    public class PreparePreviewOtherRatesForDBApplyInput
    {
        public BaseQueue<IEnumerable<OtherRatePreview>> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PreparePreviewOtherRatesForDBApply : DependentAsyncActivity<PreparePreviewOtherRatesForDBApplyInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<OtherRatePreview>>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }


        protected override void DoWork(PreparePreviewOtherRatesForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISupplierOtherRatePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierOtherRatePreviewDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, OtherRatePreview => OtherRatePreview);
        }

        protected override PreparePreviewOtherRatesForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PreparePreviewOtherRatesForDBApplyInput()
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
