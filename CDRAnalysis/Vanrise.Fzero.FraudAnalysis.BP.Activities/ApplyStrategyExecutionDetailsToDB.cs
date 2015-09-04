using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    public class ApplyStrategyExecutionDetailsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyStrategyExecutionDetailsToDB : DependentAsyncActivity<ApplyStrategyExecutionDetailsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyStrategyExecutionDetailsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IStrategyExecutionDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (preparedStrategyExecutionDetails) =>
                        {
                            dataManager.ApplyStrategyExecutionDetailsToDB(preparedStrategyExecutionDetails);
                        });
                } while (!ShouldStop(handle) && hasItems);
            }
                );
        }

        protected override ApplyStrategyExecutionDetailsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyStrategyExecutionDetailsToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
