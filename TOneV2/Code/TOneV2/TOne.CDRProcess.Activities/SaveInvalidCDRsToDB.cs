using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;
using Vanrise.BusinessProcess;

namespace TOne.CDRProcess.Activities
{
    #region Arguments Classes

    public class SaveInvalidCDRsToDBInput
    {
        public TOneQueue<CDRInvalid> InputQueue { get; set; }
    }

    #endregion

    public sealed class SaveInvalidCDRsToDB : DependentAsyncActivity<SaveInvalidCDRsToDBInput>
    {
        [RequiredArgument]
        public InOutArgument<TOneQueue<CDRInvalid>> InputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            if (this.InputQueue.Get(context) == null)
                this.InputQueue.Set(context, new TOneQueue<CDRBase>());
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(SaveInvalidCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((invalidCDR) =>
                    {

                        
                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override SaveInvalidCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new SaveInvalidCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
