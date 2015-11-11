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
        protected override T CreateStatisticItemFromRawItem(BillingCDRBase rawItem)
        {
            T statisticItem = Activator.CreateInstance<T>();
            statisticItem.CustomerId = rawItem.CustomerId;
            statisticItem.SupplierId = rawItem.SupplierId;
            statisticItem.SaleZoneId=rawItem.SaleZoneID;
            statisticItem.SupplierZoneId=rawItem.SupplierZoneID;
            statisticItem.PortIn=rawItem.PortIn;
            statisticItem.PortOut = rawItem.PortOut;
            return statisticItem;
        }

        protected override string GetStatisticItemKey(T statisticItem)
        {
            return BaseTrafficStatistic.GetStatisticItemKey(statisticItem.CustomerId, statisticItem.SupplierId, statisticItem.SaleZoneId, statisticItem.SupplierZoneId, statisticItem.PortOut, statisticItem.PortIn);
        }

        protected override string GetStatisticItemKey(BillingCDRBase rawItem)
        {
            return BaseTrafficStatistic.GetStatisticItemKey(rawItem.CustomerId, rawItem.SupplierId, rawItem.SaleZoneID, rawItem.SupplierZoneID, rawItem.PortOut, rawItem.PortIn);
        }

        protected override void UpdateStatisticItemFromRawItem(T statisticItem, BillingCDRBase item)
        {
            statisticItem.Attempts++;
            statisticItem.TotalDurationInSeconds += item.DurationInSeconds;
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
