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
    public class PrepareNewZonesForApplyInput
    {
        public int SellingNumberPlanId { get; set; }
        public BaseQueue<IEnumerable<AddedZone>> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }
    public sealed class PrepareNewZonesForApply : DependentAsyncActivity<PrepareNewZonesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<AddedZone>>> InputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }


        protected override void DoWork(PrepareNewZonesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INewSaleZoneDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleZoneDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            dataManager.SellingNumberPlanId = inputArgument.SellingNumberPlanId;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, NewZonesList => NewZonesList);
        }

        protected override PrepareNewZonesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareNewZonesForApplyInput()
            {
                SellingNumberPlanId = this.SellingNumberPlanId.Get(context),
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
