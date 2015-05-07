using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.CDR.Entities;

namespace TOne.CDRProcess.Activities
{
    #region Argument Classes

    public class GenerateTrafficStatisticsForRepricingInput
    {
        public BaseQueue<TOne.CDR.Entities.CDRBillingBatch> InputQueue { get; set; }

        public BaseQueue<TrafficStatisticBatch> OutputQueue { get; set; }
    }

    #endregion

    public sealed class GenerateTrafficStatisticsForRepricing : DependentAsyncActivity<GenerateTrafficStatisticsForRepricingInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<TOne.CDR.Entities.CDRBillingBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<TrafficStatisticBatch>> OutputQueue { get; set; }

        protected override GenerateTrafficStatisticsForRepricingInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new GenerateTrafficStatisticsForRepricingInput
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

        protected override void DoWork(GenerateTrafficStatisticsForRepricingInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
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
                            TrafficStatisticBatch trafficStatisticBatch = new TrafficStatisticBatch();
                            trafficStatisticBatch.TrafficStatistics = new TrafficStatisticsByKey();

                            foreach (var cdr in billingCDR.CDRs)
                            {
                                string trafficStatisticKey = TrafficStatistic.GetGroupKey(cdr.SwitchID, cdr.Port_IN, cdr.Port_OUT, cdr.CustomerID, cdr.OurZoneID, cdr.OriginatingZoneID, cdr.SupplierZoneID);
                                TrafficStatistic trafficStatistic;
                                if (!trafficStatisticBatch.TrafficStatistics.TryGetValue(trafficStatisticKey, out trafficStatistic))
                                {
                                    trafficStatistic = TrafficStatistic.CreateFromKey(cdr.SwitchID, cdr.Port_IN, cdr.Port_OUT, cdr.CustomerID, cdr.OurZoneID, cdr.OriginatingZoneID, cdr.SupplierID, cdr.SupplierZoneID);
                                    trafficStatistic.FirstCDRAttempt = cdr.Attempt;
                                    trafficStatistic.LastCDRAttempt = cdr.Attempt;
                                    trafficStatisticBatch.TrafficStatistics.Add(trafficStatisticKey, trafficStatistic);
                                }
                                UpdateTrafficStatisticFromCDR(trafficStatistic, cdr);
                            }
                            inputArgument.OutputQueue.Enqueue(trafficStatisticBatch);

                        }
                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        private void UpdateTrafficStatisticFromCDR(TrafficStatistic trafficStatistic, BillingCDRBase cdr)
        {


            TABS.Switch cdrSwitch = null;
            if (cdr.SwitchID != 0)
                if (!TABS.Switch.All.TryGetValue(cdr.SwitchID, out cdrSwitch))
                    throw new Exception(string.Format("UpdateTrafficStatisticFromCDR:Switch:{0} Not Exist", cdr.SwitchID));

            //trafficStatistic.Saveable = true;

            // Update Calculated fields
            // Attempts
            trafficStatistic.Attempts++;
            // Calls (Non-Rerouted Calls)
            if (!cdr.IsRerouted)
            {
                trafficStatistic.NumberOfCalls++;
                if (cdr.DurationInSeconds > 0)
                    trafficStatistic.DeliveredNumberOfCalls++;
                else if (cdr.ReleaseCode != null)
                {
                    TABS.SwitchReleaseCode releaseCode = null;
                    if (cdrSwitch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode))
                        if (releaseCode.IsDelivered)
                            trafficStatistic.DeliveredNumberOfCalls++;
                }
            }

            // Utilization
            if (cdr.Disconnect.HasValue) trafficStatistic.Utilization = trafficStatistic.Utilization.Add(cdr.Disconnect.Value.Subtract(cdr.Attempt));

            // Duration? then sucessful and delivered
            if (cdr.DurationInSeconds > 0)
            {
                trafficStatistic.SuccessfulAttempts++;
                trafficStatistic.DeliveredAttempts++;
                // PDD 
                if (cdr.PDDInSeconds > 0)
                {
                    decimal n = (decimal)trafficStatistic.SuccessfulAttempts - 1;
                    trafficStatistic.PDDInSeconds = ((n * trafficStatistic.PDDInSeconds) + cdr.PDDInSeconds) / (n + 1);
                }
                if (cdr.Connect.HasValue)
                {
                    decimal n = (decimal)trafficStatistic.SuccessfulAttempts - 1;
                    trafficStatistic.PGAD = ((n * trafficStatistic.PGAD) + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds) / (n + 1);
                }
            }
            else // No Duration check if release code can give us a hint about delivery
            {
                if (cdr.ReleaseCode != null)
                {

                    TABS.SwitchReleaseCode releaseCode = null;
                    if (cdrSwitch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode))
                        if (releaseCode.IsDelivered)
                            trafficStatistic.DeliveredAttempts++;
                }
            }

            // Sum up Durations
            trafficStatistic.DurationsInSeconds += cdr.DurationInSeconds;

            //Sum up ceiled durations
            trafficStatistic.CeiledDuration += (int)Math.Ceiling(cdr.DurationInSeconds);

            // Update Min/Max Date/ID of CDRs
            if (cdr.Attempt > trafficStatistic.LastCDRAttempt) trafficStatistic.LastCDRAttempt = cdr.Attempt;
            if (cdr.Attempt < trafficStatistic.FirstCDRAttempt) trafficStatistic.FirstCDRAttempt = cdr.Attempt;
            if (cdr.DurationInSeconds >= trafficStatistic.MaxDurationInSeconds) trafficStatistic.MaxDurationInSeconds = cdr.DurationInSeconds;
            if (cdr.ReleaseSource != null && cdr.ReleaseSource.ToUpper().Equals("A")) trafficStatistic.ReleaseSourceAParty += 1;
        }
    }
}
