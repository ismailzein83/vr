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
    public class PrepareChangedRatesForApplyInput
    {
        public BaseQueue<IEnumerable<ChangedRate>> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareChangedRatesToDB : DependentAsyncActivity<PrepareChangedRatesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<ChangedRate>>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareChangedRatesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IChangedSupplierRateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IChangedSupplierRateDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, ChangedRatesList => ChangedRatesList);
        }

        protected override PrepareChangedRatesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareChangedRatesForApplyInput()
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
