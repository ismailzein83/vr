using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Business;
using TOne.CDR.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.CDRProcess.Activities
{
    #region Argument Classes

    public class GenerateDailyTrafficStatisticsInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRBillingBatch> InputQueue { get; set; }

        public BaseQueue<TrafficStatisticDailyBatch> OutputQueue { get; set; }
    }

    #endregion

    public sealed class GenerateDailyTrafficStatistics : DependentAsyncActivity<GenerateDailyTrafficStatisticsInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRBillingBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<TrafficStatisticDailyBatch>> OutputQueue { get; set; }

        protected override void DoWork(GenerateDailyTrafficStatisticsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
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
                            Dictionary<DateTime, TrafficStatisticDailyBatch> batches = new Dictionary<DateTime, TrafficStatisticDailyBatch>();
                            foreach (var cdr in billingCDR.CDRs)
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
                                inputArgument.OutputQueue.Enqueue(trafficStatisticBatch);
                            }
                        }
                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override GenerateDailyTrafficStatisticsInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new GenerateDailyTrafficStatisticsInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<TrafficStatisticDailyBatch>());

            base.OnBeforeExecute(context, handle);
        }

    }
}
