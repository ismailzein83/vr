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
    #region Argument Classes

    public class ApplyCDRsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
        public string TableKey { get; set; }
    }
    
    #endregion

    public sealed class ApplyCDRsToDB : DependentAsyncActivity<ApplyCDRsToDBInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<string> TableKey { get; set; }
        #endregion

        protected override void DoWork(ApplyCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRDataManager>();
            dataManager.TableNameKey = inputArgument.TableKey;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (preparedCDRs) =>
                        {
                            dataManager.ApplyCDRsToDB(preparedCDRs);
                        });
                } while (!ShouldStop(handle) && hasItems);
            });
        }

        protected override ApplyCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyCDRsToDBInput()
            {
                InputQueue = this.InputQueue.Get(context),
                TableKey = this.TableKey.Get(context)
            };
        }
    }
}
