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

    public class ApplyMainCDRsToDBInput
    {
        public TOneQueue<Object> InputQueue { get; set; }
    }

    #endregion

    public sealed class ApplyMainCDRsToDB : DependentAsyncActivity<ApplyMainCDRsToDBInput>
    {
        [RequiredArgument]
        public InArgument<TOneQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyMainCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (preparedMainCDRs) =>
                        {
                            dataManager.ApplyMainCDRsToDB(preparedMainCDRs);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyMainCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyMainCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
