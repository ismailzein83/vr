using CDRComparison.Data;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace CDRComparison.BP.Activities
{
    public class ApplyDisputeCDRsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
        public string TableKey { get; set; }
    }

    public sealed class ApplyDisputeCDRsToDB : DependentAsyncActivity<ApplyDisputeCDRsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<string> TableKey { get; set; }
        protected override void DoWork(ApplyDisputeCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IDisputeCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IDisputeCDRDataManager>();
            dataManager.TableNameKey = inputArgument.TableKey;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (cdrItem) =>
                        {
                            dataManager.ApplyDisputeCDRsToDB(cdrItem);
                        });
                } while (!ShouldStop(handle) && hasItems);
            }
                );
        }

        protected override ApplyDisputeCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyDisputeCDRsToDBInput()
            {
                InputQueue = this.InputQueue.Get(context),
                TableKey = this.TableKey.Get(context)
            };
        }
    }
}
