using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    public class ApplyStrategyExecutionItemToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyStrategyExecutionItemToDB : DependentAsyncActivity<ApplyStrategyExecutionItemToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyStrategyExecutionItemToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IStrategyExecutionDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (preparedStrategyExecutionItem) =>
                        {
                            dataManager.ApplyStrategyExecutionItemToDB(preparedStrategyExecutionItem);
                        });
                } while (!ShouldStop(handle) && hasItems);
            }
                );
        }

        protected override ApplyStrategyExecutionItemToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyStrategyExecutionItemToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
