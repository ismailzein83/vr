using TOne.CDR.Data;
using TOne.CDR.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.QueueActivators
{
    public class StoreStatisticsActivator : QueueActivator
    {
        public override void OnDisposed()
        {
            
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            ITrafficStatisticDataManager dataManager = CDRDataManagerFactory.GetDataManager<ITrafficStatisticDataManager>();
            
            TrafficStatisticBatch trafficStatisticsBatch = item as TrafficStatisticBatch;
            if (trafficStatisticsBatch != null)
                dataManager.UpdateTrafficStatisticBatch(trafficStatisticsBatch.BatchStart, trafficStatisticsBatch.BatchEnd, trafficStatisticsBatch.TrafficStatistics);
        }
    }
}
