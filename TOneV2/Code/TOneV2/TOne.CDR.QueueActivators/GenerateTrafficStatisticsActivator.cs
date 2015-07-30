using System;
using System.Collections.Generic;
using TOne.CDR.Business;
using TOne.CDR.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.QueueActivators
{
    public class GenerateTrafficStatisticsActivator : QueueActivator
    {

        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            TrafficStatisticGenerator trafficStatGenerator = new TrafficStatisticGenerator();
            int sampleIntervalInMinute = (int)(60 / TABS.SystemParameter.TrafficStatsSamplesPerHour.NumericValue.Value);

            CDRBillingBatch billingCdr = item as CDRBillingBatch;

            if (billingCdr == null || billingCdr.CDRs == null) return;


            Dictionary<DateTime, TrafficStatisticBatch> batches = new Dictionary<DateTime, TrafficStatisticBatch>();
            foreach (var cdr in billingCdr.CDRs)
            {
                DateTime cdrTime = cdr.Attempt;
                DateTime cdrTrafficBatchStart = new DateTime(cdrTime.Year, cdrTime.Month, cdrTime.Day, cdrTime.Hour, cdrTime.Minute / sampleIntervalInMinute * sampleIntervalInMinute, 0);

                TrafficStatisticBatch trafficStatisticBatch;
                if (!batches.TryGetValue(cdrTrafficBatchStart, out trafficStatisticBatch))
                {
                    trafficStatisticBatch = new TrafficStatisticBatch
                    {
                        BatchStart = cdrTrafficBatchStart,
                        BatchEnd = cdrTrafficBatchStart.AddMinutes(sampleIntervalInMinute),
                        TrafficStatistics = new TrafficStatisticsByKey()
                    };
                    batches.Add(cdrTrafficBatchStart, trafficStatisticBatch);
                }

                string trafficStatisticKey = TrafficStatistic.GetGroupKey(cdr.SwitchID, cdr.Port_IN, cdr.Port_OUT, cdr.CustomerID, cdr.OurZoneID, cdr.OriginatingZoneID, cdr.SupplierZoneID);
                TrafficStatistic trafficStatistic;
                if (!trafficStatisticBatch.TrafficStatistics.TryGetValue(trafficStatisticKey, out trafficStatistic))
                {
                    trafficStatistic = TrafficStatistic.CreateFromKey(cdr.SwitchID, cdr.Port_IN, cdr.Port_OUT, cdr.CustomerID, cdr.OurZoneID, cdr.OriginatingZoneID, cdr.SupplierID, cdr.SupplierZoneID);
                    trafficStatisticBatch.TrafficStatistics.Add(trafficStatisticKey, trafficStatistic);
                }
                trafficStatGenerator.UpdateTrafficStatisticFromCDR(trafficStatistic, cdr);
            }

            foreach (var trafficStatisticBatch in batches.Values)
            {
                outputItems.Add("Store Traffic Statistics", trafficStatisticBatch);
            }
        }
    }
}
