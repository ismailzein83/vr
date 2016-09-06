using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess;
using TOne.WhS.SupplierPriceList.Data;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public class PrepareNewZonesServicesForDBApplyInput
    {
        public BaseQueue<IEnumerable<NewZoneService>> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareNewZonesServicesForDBApply : DependentAsyncActivity<PrepareNewZonesServicesForDBApplyInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<NewZoneService>>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareNewZonesServicesForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INewSupplierZonesServicesDataManager dataManager = SupPLDataManagerFactory.GetDataManager<INewSupplierZonesServicesDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, NewZonesServicesList => NewZonesServicesList);
        }

        protected override PrepareNewZonesServicesForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareNewZonesServicesForDBApplyInput()
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
