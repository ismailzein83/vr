using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class PrepareNewRatesForApplyInput
    {
        public BaseQueue<IEnumerable<AddedRate>> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }
    public sealed class PrepareNewRatesForApply : DependentAsyncActivity<PrepareNewRatesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<AddedRate>>> InputQueue { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareNewRatesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INewSaleRateDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleRateDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, NewRatesList => NewRatesList);
        }

        protected override PrepareNewRatesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareNewRatesForApplyInput()
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
