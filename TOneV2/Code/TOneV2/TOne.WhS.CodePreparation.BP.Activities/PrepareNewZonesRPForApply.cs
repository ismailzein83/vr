using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class PrepareNewZonesRoutingProductsForApplyInput
    {
        public BaseQueue<IEnumerable<AddedZoneRoutingProduct>> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }
    public sealed class PrepareNewZonesRoutingProductsForApply : DependentAsyncActivity<PrepareNewZonesRoutingProductsForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<AddedZoneRoutingProduct>>> InputQueue { get; set; }
        
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareNewZonesRoutingProductsForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INewSaleZoneRoutingProductDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleZoneRoutingProductDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, NewZonesRoutingProductsList => NewZonesRoutingProductsList);
        }

        protected override PrepareNewZonesRoutingProductsForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareNewZonesRoutingProductsForApplyInput()
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
