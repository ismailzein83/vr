using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    public class ApplyDWCDRsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyDWCDRsToDB : DependentAsyncActivity<ApplyDWCDRsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyDWCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IDWCDRDataManager dataManager = FraudDataManagerFactory.GetDataManager<IDWCDRDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (preparedDWCDRs) =>
                        {
                            dataManager.ApplyDWCDRsToDB(preparedDWCDRs);
                        });
                } while (!ShouldStop(handle) && hasItems);
            }
                );
        }

        protected override ApplyDWCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyDWCDRsToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
