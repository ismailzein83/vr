using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    public class ApplyNumberProfilesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyNumberProfilesToDB : DependentAsyncActivity<ApplyNumberProfilesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyNumberProfilesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (preparedNumberProfiles) =>
                        {
                            dataManager.ApplyNumberProfilesToDB(preparedNumberProfiles);
                        });
                } while (!ShouldStop(handle) && hasItems);
            }
                );
        }

        protected override ApplyNumberProfilesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyNumberProfilesToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
