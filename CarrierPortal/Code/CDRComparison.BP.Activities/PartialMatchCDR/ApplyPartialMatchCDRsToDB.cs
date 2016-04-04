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
    public class ApplyPartialMatchCDRsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
        public string TableKey { get; set; }
    }

    public sealed class ApplyPartialMatchCDRsToDB : DependentAsyncActivity<ApplyPartialMatchCDRsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<string> TableKey { get; set; }
        protected override void DoWork(ApplyPartialMatchCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IPartialMatchCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IPartialMatchCDRDataManager>();
            dataManager.TableNameKey = inputArgument.TableKey;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (cdrItem) =>
                        {
                            dataManager.ApplyPartialMatchCDRsToDB(cdrItem);
                        });
                } while (!ShouldStop(handle) && hasItems);
            }
                );
        }

        protected override ApplyPartialMatchCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyPartialMatchCDRsToDBInput()
            {
                InputQueue = this.InputQueue.Get(context),
                TableKey = this.TableKey.Get(context)
            };
        }
    }
}
