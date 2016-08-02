using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Data;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{


    public class PrepareProductRoutesForApplyInput
    {
        public BaseQueue<RPRouteBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareProductRoutesForApply : DependentAsyncActivity<PrepareProductRoutesForApplyInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<RPRouteBatch>> InputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareProductRoutesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IRPRouteDataManager productRoutesDataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, productRoutesDataManager, inputArgument.InputQueue, inputArgument.OutputQueue, ProductRoutesBatch => ProductRoutesBatch.RPRoutes);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Product Routes For Apply is done", null);
        }

        protected override PrepareProductRoutesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareProductRoutesForApplyInput
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
