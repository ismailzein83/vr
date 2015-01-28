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

    public class SaveMainCDRsToDBInput
    {
        public TOneQueue<CDRMain> InputQueue { get; set; }

    }

    #endregion

    public sealed class SaveMainCDRsToDB : DependentAsyncActivity<SaveMainCDRsToDBInput>
    {
        [RequiredArgument]
        public InArgument <TOneQueue<CDRMain>> InputQueue { get; set; }

        protected override void DoWork(SaveMainCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((mainCDR) =>
                    {

                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override SaveMainCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new SaveMainCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
