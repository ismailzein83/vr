using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.CDR.Entities;
using TOne.CDR.Business;

namespace TOne.CDRProcess.Activities
{
    #region Argument Classes

    public class GenerateTrafficStatisticsInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRBillingBatch> InputQueue { get; set; }

        public BaseQueue<TrafficStatisticBatch> OutputQueue { get; set; }
    }

    #endregion

    public sealed class GenerateTrafficStatistics : DependentAsyncActivity<GenerateTrafficStatisticsInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRBillingBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<TrafficStatisticBatch>> OutputQueue { get; set; }
        
        protected override GenerateTrafficStatisticsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new GenerateTrafficStatisticsInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<TrafficStatisticBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(GenerateTrafficStatisticsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            TrafficStatisticManager trafficStatGenerator = new TrafficStatisticManager();

            int sampleIntervalInMinute = (int)(60 / TABS.SystemParameter.TrafficStatsSamplesPerHour.NumericValue.Value);
            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((billingCDR) =>
                    {
                        if (billingCDR != null && billingCDR.CDRs != null)
                        {
                            Dictionary<DateTime, TrafficStatisticBatch> batches = new Dictionary<DateTime, TrafficStatisticBatch>();
                            foreach (var cdr in billingCDR.CDRs)
                            {
                                DateTime cdrTime = cdr.Attempt;
                                DateTime cdrTrafficBatchStart = new DateTime(cdrTime.Year, cdrTime.Month, cdrTime.Day, cdrTime.Hour, ((int)(cdrTime.Minute / sampleIntervalInMinute)) * sampleIntervalInMinute, 0);
                                
                                TrafficStatisticBatch trafficStatisticBatch;
                                if(!batches.TryGetValue(cdrTrafficBatchStart, out trafficStatisticBatch))
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
                                if(!trafficStatisticBatch.TrafficStatistics.TryGetValue(trafficStatisticKey, out trafficStatistic))
                                {
                                    trafficStatistic = TrafficStatistic.CreateFromKey(cdr.SwitchID, cdr.Port_IN, cdr.Port_OUT, cdr.CustomerID, cdr.OurZoneID, cdr.OriginatingZoneID, cdr.SupplierID, cdr.SupplierZoneID);
                                    trafficStatisticBatch.TrafficStatistics.Add(trafficStatisticKey, trafficStatistic);
                                }
                                trafficStatGenerator.UpdateTrafficStatisticFromCDR(trafficStatistic, cdr);
                            }

                            foreach (var trafficStatisticBatch in batches.Values)
                            {
                                inputArgument.OutputQueue.Enqueue(trafficStatisticBatch);
                            }
                        }
                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }
    }
}
