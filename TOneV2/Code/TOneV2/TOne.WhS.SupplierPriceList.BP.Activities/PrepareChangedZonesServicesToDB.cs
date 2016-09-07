using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.SupplierPriceList.Data;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class PrepareChangedZonesServicesForApplyInput
    {
        public BaseQueue<IEnumerable<ChangedZoneService>> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareChangedZonesServicesToDB : DependentAsyncActivity<PrepareChangedZonesServicesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ChangedZoneService>>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareChangedZonesServicesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IChangedSupplierZonesServicesDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IChangedSupplierZonesServicesDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, ChangedZonesServicesList => ChangedZonesServicesList);
        }

        protected override PrepareChangedZonesServicesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareChangedZonesServicesForApplyInput()
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
