using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.CDR.Entities;
using Vanrise.BusinessProcess;
using TOne.CDR.Data;

namespace TOne.CDRProcess.Activities
{
    #region Argument Classes

    public class UpdateTrafficStatisticsInDBInput
    {
        public BaseQueue<TrafficStatisticBatch> InputQueue { get; set; }
    }

    #endregion

    public sealed class UpdateTrafficStatisticsInDB : DependentAsyncActivity<UpdateTrafficStatisticsInDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<TrafficStatisticBatch>> InputQueue { get; set; }

        protected override UpdateTrafficStatisticsInDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateTrafficStatisticsInDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }

        protected override void DoWork(UpdateTrafficStatisticsInDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
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
    }
}
