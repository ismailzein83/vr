using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.NumberingPlan.Entities;
using Vanrise.NumberingPlan.Data;

namespace Vanrise.NumberingPlan.BP.Activities
{
    public class PrepareChangedZonesForApplyInput
    {
        public BaseQueue<IEnumerable<ChangedZone>> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }
    public sealed class PrepareChangedZonesForApply : DependentAsyncActivity<PrepareChangedZonesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ChangedZone>>> InputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareChangedZonesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IChangedSaleZoneDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedSaleZoneDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, ChangedZonesList => ChangedZonesList);
        }

        protected override PrepareChangedZonesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareChangedZonesForApplyInput()
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
