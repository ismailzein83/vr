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
    public class PrepareCDRsForDBApplyInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }

        public int SwitchId { get; set; }
    }

    #endregion

    public sealed class PrepareCDRsForDBApply : DependentAsyncActivity<PrepareCDRsForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> SwitchId { get; set; }

        protected override void DoWork(PrepareCDRsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (CDR) =>
                        {
                            Object preparedMainCDRs = dataManager.PrepareCDRsForDBApply(CDR.CDRs , inputArgument.SwitchId);
                            inputArgument.OutputQueue.Enqueue(preparedMainCDRs);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override PrepareCDRsForDBApplyInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new PrepareCDRsForDBApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                SwitchId = this.SwitchId.Get(context),
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<object>());
            base.OnBeforeExecute(context, handle);
        }

    }
}
