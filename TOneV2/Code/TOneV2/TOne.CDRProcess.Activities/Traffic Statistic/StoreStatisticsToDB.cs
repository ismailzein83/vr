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
    public class StoreStatisticsToDBInput
    {
        public BaseQueue<TrafficStatisticBatch> InputQueue { get; set; }
    }

    #endregion

    public sealed class StoreStatisticsToDB : DependentAsyncActivity<StoreStatisticsToDBInput>
    {

        [RequiredArgument]
        public InOutArgument<BaseQueue<TrafficStatisticBatch>> InputQueue { get; set; }

        protected override void DoWork(StoreStatisticsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ITrafficStatisticDataManager dataManager = CDRDataManagerFactory.GetDataManager<ITrafficStatisticDataManager>();
            
            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((trafficStatisticsBatch) =>
                    {
                        dataManager.UpdateTrafficStatisticBatch(trafficStatisticsBatch.BatchStart, trafficStatisticsBatch.BatchEnd, trafficStatisticsBatch.TrafficStatistics);
                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override StoreStatisticsToDBInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new StoreStatisticsToDBInput
            {
                InputQueue = this.InputQueue.Get(context),
            };
        }
    }
}
