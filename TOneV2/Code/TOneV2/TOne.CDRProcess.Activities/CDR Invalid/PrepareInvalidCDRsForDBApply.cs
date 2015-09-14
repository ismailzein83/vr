using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Data;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.CDRProcess.Activities
{
    #region Argument Classes
    public class PrepareInvalidCDRsForDBApplyInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRInvalidBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion

    public sealed class PrepareInvalidCDRsForDBApply : DependentAsyncActivity<PrepareInvalidCDRsForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRInvalidBatch>> InputQueue { get; set; }


        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<object>());
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareInvalidCDRsForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareInvalidCDRsForDBApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void DoWork(PrepareInvalidCDRsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            
            ICDRInvalidDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRInvalidDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, invalidCDRs =>
                {
                    long lastPricedInvalidID;
                    dataManager.ReserverIdRange(false, true, invalidCDRs.InvalidCDRs.Count, out lastPricedInvalidID);
                    foreach (TOne.CDR.Entities.BillingCDRInvalid invalidCDR in invalidCDRs.InvalidCDRs)
                    {
                        invalidCDR.ID = --lastPricedInvalidID;
                    }
                  return invalidCDRs.InvalidCDRs;
                });
        }
    }
}
