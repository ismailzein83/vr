using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Data;
using TOne.Entities;
using Vanrise.BusinessProcess;

namespace TOne.CDRProcess.Activities
{
    #region Argument Classes
    public class PrepareMainCDRsForDBApplyInput
    {
        public TOneQueue<CDRMainBatch> InputQueue { get; set; }

        public TOneQueue<Object> OutputQueue { get; set; }
    }

    #endregion

    public sealed class PrepareMainCDRsForDBApply : DependentAsyncActivity<PrepareMainCDRsForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<TOneQueue<CDRMainBatch>> InputQueue { get; set; }

        
        [RequiredArgument]
        public InOutArgument<TOneQueue<Object>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new TOneQueue<object>());
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(PrepareMainCDRsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (cdrMainBatch) =>
                        {
                            Object preparedMainCDRs = dataManager.PrepareMainCDRsForDBApply(cdrMainBatch.mainCDRs);
                            inputArgument.OutputQueue.Enqueue(preparedMainCDRs);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override PrepareMainCDRsForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareMainCDRsForDBApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
