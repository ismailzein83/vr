using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class PrepareSupplierZoneDetailForDBApplyInput
    {
        public BaseQueue<SupplierZoneDetailBatch> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareSupplierZoneDetailForDBApply : DependentAsyncActivity<PrepareSupplierZoneDetailForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<SupplierZoneDetailBatch>> InputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareSupplierZoneDetailForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISupplierZoneDetailsDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ISupplierZoneDetailsDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, SupplierZoneDetailsBatch => SupplierZoneDetailsBatch.SupplierZoneDetails);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Supplier Zone Detail For DB Apply is done", null);
        }

        protected override PrepareSupplierZoneDetailForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareSupplierZoneDetailForDBApplyInput()
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
