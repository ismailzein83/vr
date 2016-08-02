using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class PrepareCustomerRoutesForApplyInput
    {
        public BaseQueue<CustomerRoutesBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareCustomerRoutesForApply : DependentAsyncActivity<PrepareCustomerRoutesForApplyInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<CustomerRoutesBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }


        protected override void DoWork(PrepareCustomerRoutesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICustomerRouteDataManager customeRoutesDataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, customeRoutesDataManager, inputArgument.InputQueue, inputArgument.OutputQueue, CustomerRoutesBatch => CustomerRoutesBatch.CustomerRoutes);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Customer Routes For Apply is done", null);
        }

        protected override PrepareCustomerRoutesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCustomerRoutesForApplyInput
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
