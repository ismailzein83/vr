using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Data;
using TOne.CDR.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.CDRProcess.Activities
{
    #region Argument Classes
    public class StoreDailyStatisticsToDBInput
    {
        public BaseQueue<TrafficStatisticDailyBatch> InputQueue { get; set; }
    }

    #endregion

    public sealed class StoreDailyStatisticsToDB : DependentAsyncActivity<StoreDailyStatisticsToDBInput>
    {
        [RequiredArgument]
        public InOutArgument<BaseQueue<TrafficStatisticDailyBatch>> InputQueue { get; set; }

        protected override void DoWork(StoreDailyStatisticsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ITrafficStatisticDataManager dataManager = CDRDataManagerFactory.GetDataManager<ITrafficStatisticDataManager>();

            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((dailyTrafficStatisticsBatch) =>
                    {
                        dataManager.UpdateTrafficStatisticDailyBatch(dailyTrafficStatisticsBatch.BatchDate, dailyTrafficStatisticsBatch.TrafficStatistics);
                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override StoreDailyStatisticsToDBInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new StoreDailyStatisticsToDBInput
            {
                InputQueue = this.InputQueue.Get(context),
            };
        }
    }
}
