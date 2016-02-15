using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    public class ApplyDWFactsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyDWFactsToDB : DependentAsyncActivity<ApplyDWFactsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyDWFactsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IDWFactDataManager dataManager = FraudDataManagerFactory.GetDataManager<IDWFactDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (preparedDWFacts) =>
                        {
                            dataManager.ApplyDWFactsToDB(preparedDWFacts);
                        });
                } while (!ShouldStop(handle) && hasItems);
            }
                );
        }

        protected override ApplyDWFactsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyDWFactsToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
