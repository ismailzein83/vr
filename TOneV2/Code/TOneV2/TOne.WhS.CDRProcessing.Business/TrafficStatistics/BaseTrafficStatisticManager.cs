using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business
{
    public abstract class BaseTrafficStatisticManager<T,Q> : Vanrise.Common.Business.StatisticManagement.StatisticBatchManager<BillingCDRBase, T,Q>
        where Q :TrafficStatisticBatch<T>
        where T : BaseTrafficStatistic
        
    {
  
        protected override void UpdateStatisticItemFromRawItem(T statisticItem, BillingCDRBase item)
        {
            statisticItem.Attempts++;
            statisticItem.DurationInSeconds += item.DurationInSeconds;
            statisticItem.FirstCDRAttempt = item.Attempt <= statisticItem.FirstCDRAttempt || statisticItem.FirstCDRAttempt == default(DateTime) ? item.Attempt : statisticItem.FirstCDRAttempt;
            statisticItem.LastCDRAttempt = item.Attempt > statisticItem.LastCDRAttempt ? item.Attempt : statisticItem.LastCDRAttempt;
            statisticItem.NumberOfCalls++;
            if (item.DurationInSeconds > 0)
            {
                statisticItem.SuccessfulAttempts++;
                statisticItem.DeliveredAttempts++;
                var time = GetFirstValidDateTime(item.Alert, item.Connect);
                var pdd = time.HasValue ? 
                (int)time.Value.Subtract(item.Attempt).TotalSeconds
                : (item.Disconnect.HasValue ? (int)item.Disconnect.Value.Subtract(item.Attempt).TotalSeconds - item.DurationInSeconds : 0);
                statisticItem.PDDInSeconds = statisticItem.PDDInSeconds > 0 ? (int)(statisticItem.PDDInSeconds + pdd) : pdd;
            }
            statisticItem.MaxDurationInSeconds = item.DurationInSeconds >= statisticItem.MaxDurationInSeconds ? item.DurationInSeconds : statisticItem.MaxDurationInSeconds;
            if (item.DurationInSeconds > 0)
                statisticItem.DeliveredNumberOfCalls++;

            if (item.Connect.HasValue)
            {
                statisticItem.PGAD += item.Connect.Value.Subtract(item.Attempt).Seconds;
            }
           // statisticItem.CeiledDuration += (int)Math.Ceiling(item.DurationInSeconds);
            //if (item.Disconnect.HasValue) 
            //    statisticItem.Utilization += item.Disconnect.Value.Subtract(item.Attempt);

        }
        public static DateTime? GetFirstValidDateTime(params DateTime?[] times)
        {
            DateTime? result = null;
            foreach (var time in times)
                if (time.HasValue) return time;
            return result;
        }


    
}
}
