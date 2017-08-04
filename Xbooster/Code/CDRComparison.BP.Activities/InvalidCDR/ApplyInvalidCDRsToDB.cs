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
    #region Public Classes

    public class ApplyInvalidCDRsToDBInput
    {
        public string TableKey { get; set; }
        public BaseQueue<Object> InputQueue { get; set; }
    }

    #endregion

    public sealed class ApplyInvalidCDRsToDB : DependentAsyncActivity<ApplyInvalidCDRsToDBInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<string> TableKey { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        #endregion

        protected override ApplyInvalidCDRsToDBInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new ApplyInvalidCDRsToDBInput()
            {
                TableKey = this.TableKey.Get(context),
                InputQueue = this.InputQueue.Get(context)
            };
        }

        protected override void DoWork(ApplyInvalidCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IInvalidCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IInvalidCDRDataManager>();
            dataManager.TableNameKey = inputArgument.TableKey;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(preparedInvalidCDRs =>
                    {
                        dataManager.ApplyInvalidCDRsToDB(preparedInvalidCDRs);
                    });
                } while (!ShouldStop(handle) && hasItems);
            });
        }
    }
}
