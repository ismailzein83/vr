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
    public class PrepareInvalidCDRsForDBApplyInput
    {
        public TOneQueue<CDRInvalidBatch> InputQueue { get; set; }

        public TOneQueue<Object> OutputQueue { get; set; }
    }

    #endregion

    public sealed class PrepareInvalidCDRsForDBApply : DependentAsyncActivity<PrepareInvalidCDRsForDBApplyInput>
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<TOneQueue<CDRInvalidBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<TOneQueue<Object>> OutputQueue { get; set; }


        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new TOneQueue<object>());
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(PrepareInvalidCDRsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (invalidCDR) =>
                        {
                            Object preparedInvalidCDRs = dataManager.PrepareInvalidCDRsForDBApply(invalidCDR.InvalidCDRs);
                            inputArgument.OutputQueue.Enqueue(preparedInvalidCDRs);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override PrepareInvalidCDRsForDBApplyInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new PrepareInvalidCDRsForDBApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
