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

    public class ApplyCDRsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    #endregion


    public sealed class ApplyCDRsToDB : DependentAsyncActivity<ApplyCDRsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (preparedCDRs) =>
                        {
                            dataManager.ApplyCDRsToDB(preparedCDRs);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyCDRsToDBInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new ApplyCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
