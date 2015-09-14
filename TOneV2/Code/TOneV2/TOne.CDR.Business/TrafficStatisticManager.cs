using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Data;
using TOne.CDR.Entities;

namespace TOne.CDR.Business
{
    public class TrafficStatisticManager
    {
        public void UpdateTrafficStatisticFromCDR(TrafficStatistic trafficStatistic, BillingCDRBase cdr)
        {
            UpdateBaseTrafficStatisticFromCDR(trafficStatistic, cdr);
            // Update Min/Max Date/ID of CDRs
            if (cdr.Attempt > trafficStatistic.LastCDRAttempt) trafficStatistic.LastCDRAttempt = cdr.Attempt;
            if (trafficStatistic.FirstCDRAttempt == DateTime.MinValue || cdr.Attempt < trafficStatistic.FirstCDRAttempt) trafficStatistic.FirstCDRAttempt = cdr.Attempt;
        }
        public void DeleteTrafficStats(DateTime from, DateTime to, List<string> customerIds, List<string> supplierIds)
        {
            ITrafficStatisticDataManager dataManager =  CDRDataManagerFactory.GetDataManager<ITrafficStatisticDataManager>();
            dataManager.DeleteTrafficStats(from, to, customerIds, supplierIds);
        }
        public void UpdateBaseTrafficStatisticFromCDR(BaseTrafficStatistic trafficStatistic, BillingCDRBase cdr)
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
            if (cdr.DurationInSeconds >= trafficStatistic.MaxDurationInSeconds) trafficStatistic.MaxDurationInSeconds = cdr.DurationInSeconds;
            if (cdr.ReleaseSource != null && cdr.ReleaseSource.ToUpper().Equals("A")) trafficStatistic.ReleaseSourceAParty += 1;
            if (cdr.ReleaseSource != null && cdr.ReleaseSource.ToUpper().Equals("S")) trafficStatistic.ReleaseSourceS += 1;
        }

    }
}
