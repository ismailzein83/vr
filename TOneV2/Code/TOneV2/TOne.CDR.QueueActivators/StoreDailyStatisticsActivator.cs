using TOne.CDR.Data;
using TOne.CDR.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.QueueActivators
{
    public class StoreDailyStatisticsActivator : QueueActivator
    {
        public override void OnDisposed()
        {
            
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            ITrafficStatisticDataManager dataManager = CDRDataManagerFactory.GetDataManager<ITrafficStatisticDataManager>();
            TrafficStatisticDailyBatch dailyTrafficStatisticsBatch = item as TrafficStatisticDailyBatch;
            if (dailyTrafficStatisticsBatch != null)
                dataManager.UpdateTrafficStatisticDailyBatch(dailyTrafficStatisticsBatch.BatchDate, dailyTrafficStatisticsBatch.TrafficStatistics);
        }
    }
}
