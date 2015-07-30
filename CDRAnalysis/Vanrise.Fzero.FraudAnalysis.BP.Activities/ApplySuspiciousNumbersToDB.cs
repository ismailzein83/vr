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
    public class ApplySuspiciousNumbersToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplySuspiciousNumbersToDB : DependentAsyncActivity<ApplySuspiciousNumbersToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplySuspiciousNumbersToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISuspiciousNumberDataManager dataManager = FraudDataManagerFactory.GetDataManager<ISuspiciousNumberDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (preparedSuspiciousNumbers) =>
                        {
                            dataManager.ApplySuspiciousNumbersToDB(preparedSuspiciousNumbers);
                        });
                } while (!ShouldStop(handle) && hasItems);
            }
                );
        }

        protected override ApplySuspiciousNumbersToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplySuspiciousNumbersToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
