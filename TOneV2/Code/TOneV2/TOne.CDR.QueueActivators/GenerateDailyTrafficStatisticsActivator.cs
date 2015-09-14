using System;
using System.Collections.Generic;
using TOne.CDR.Business;
using TOne.CDR.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.QueueActivators
{
    public class GenerateDailyTrafficStatisticsActivator : QueueActivator
    {
        public override void OnDisposed()
        {
            
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            TrafficStatisticManager trafficStatGenerator = new TrafficStatisticManager();

            CDRBillingBatch billingCdr = item as CDRBillingBatch;

            if (billingCdr != null && billingCdr.CDRs != null)
            {
                Dictionary<DateTime, TrafficStatisticDailyBatch> batches = new Dictionary<DateTime, TrafficStatisticDailyBatch>();
                foreach (var cdr in billingCdr.CDRs)
                {
                    DateTime cdrTime = cdr.Attempt;
                    DateTime cdrTrafficBatchStart = new DateTime(cdrTime.Year, cdrTime.Month, cdrTime.Day, 0, 0, 0);

                    TrafficStatisticDailyBatch trafficStatisticBatch;
                    if (!batches.TryGetValue(cdrTrafficBatchStart, out trafficStatisticBatch))
                    {
                        trafficStatisticBatch = new TrafficStatisticDailyBatch
                        {
                            BatchDate = cdrTrafficBatchStart,
                            TrafficStatistics = new TrafficStatisticsDailyByKey()
                        };
                        batches.Add(cdrTrafficBatchStart, trafficStatisticBatch);
                    }

                    string trafficStatisticKey = TrafficStatisticDaily.GetGroupKey(cdr.SwitchID, cdr.CustomerID, cdr.OurZoneID, cdr.OriginatingZoneID, cdr.SupplierZoneID);
                    TrafficStatisticDaily trafficStatistic;
                    if (!trafficStatisticBatch.TrafficStatistics.TryGetValue(trafficStatisticKey, out trafficStatistic))
                    {
                        trafficStatistic = TrafficStatisticDaily.CreateFromKey(cdr.SwitchID, cdr.CustomerID, cdr.OurZoneID, cdr.OriginatingZoneID, cdr.SupplierID, cdr.SupplierZoneID);
                        trafficStatisticBatch.TrafficStatistics.Add(trafficStatisticKey, trafficStatistic);
                    }
                    trafficStatistic.CallDate = cdrTrafficBatchStart;
                    trafficStatGenerator.UpdateBaseTrafficStatisticFromCDR(trafficStatistic, cdr);
                }

                foreach (var trafficStatisticBatch in batches.Values)
                {
                    outputItems.Add("Store Daily Traffic Statistics", trafficStatisticBatch);
                }
            }
        }
    }
}
