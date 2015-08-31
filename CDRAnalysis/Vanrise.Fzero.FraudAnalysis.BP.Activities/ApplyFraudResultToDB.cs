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
    public class ApplyFraudResultToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyFraudResultToDB : DependentAsyncActivity<ApplyFraudResultToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyFraudResultToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IFraudResultDataManager dataManager = FraudDataManagerFactory.GetDataManager<IFraudResultDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (preparedSuspiciousNumbers) =>
                        {
                            dataManager.ApplyFraudResultToDB(preparedSuspiciousNumbers);
                        });
                } while (!ShouldStop(handle) && hasItems);
            }
                );
        }

        protected override ApplyFraudResultToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyFraudResultToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
