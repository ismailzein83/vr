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

    public class ApplyInvalidCDRsToDBInput
    {
        public TOneQueue<Object> InputQueue { get; set; }
    }

    #endregion

    public sealed class ApplyInvalidCDRsToDB : DependentAsyncActivity<ApplyInvalidCDRsToDBInput>
    {
        [RequiredArgument]
        public InArgument<TOneQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyInvalidCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (preparedInvalidCDRs) =>
                        {
                            dataManager.ApplyInvalidCDRsToDB(preparedInvalidCDRs);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyInvalidCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyInvalidCDRsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
