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
    public class PrepareMainCDRsForDBApplyInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRMainBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion

    public sealed class PrepareMainCDRsForDBApply : DependentAsyncActivity<PrepareMainCDRsForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRMainBatch>> InputQueue { get; set; }


        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<object>());
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareMainCDRsForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareMainCDRsForDBApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void DoWork(PrepareMainCDRsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRMainDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRMainDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, mainCDRs =>
            {
                long lastPricedMainID;
                dataManager.ReserverIdRange(false, true, mainCDRs.MainCDRs.Count, out lastPricedMainID);
                foreach (TOne.CDR.Entities.BillingCDRMain mainCDR in mainCDRs.MainCDRs)
                {
                    mainCDR.ID = --lastPricedMainID;
                }
                return mainCDRs.MainCDRs;
            });
        }
    }
}
