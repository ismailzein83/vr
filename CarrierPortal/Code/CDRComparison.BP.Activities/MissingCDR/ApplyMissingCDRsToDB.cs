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
    public class ApplyMissingCDRsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
        public string TableKey { get; set; }
    }

    public sealed class ApplyMissingCDRsToDB : DependentAsyncActivity<ApplyMissingCDRsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<string> TableKey { get; set; }
        protected override void DoWork(ApplyMissingCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IMissingCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IMissingCDRDataManager>();
            dataManager.TableNameKey = inputArgument.TableKey;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (cdrItem) =>
                        {
                            dataManager.ApplyMissingCDRsToDB(cdrItem);
                        });
                } while (!ShouldStop(handle) && hasItems);
            }
                );
        }

        protected override ApplyMissingCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyMissingCDRsToDBInput()
            {
                InputQueue = this.InputQueue.Get(context),
                TableKey = this.TableKey.Get(context)
            };
        }
    }
}
