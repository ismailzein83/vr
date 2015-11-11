using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Data;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business
{
    public class TrafficStatisticDailyManager : Vanrise.Common.Business.StatisticManagement.StatisticBatchManager<TrafficStatisticByInterval, TrafficStatisticDaily, TrafficStatisticDailyBatch>
    {

        protected override TrafficStatisticDaily CreateStatisticItemFromRawItem(TrafficStatisticByInterval rawItem)
        {
            TrafficStatisticDaily statisticItem = new TrafficStatisticDaily();
            statisticItem.CustomerId = rawItem.CustomerId;
            statisticItem.SupplierId = rawItem.SupplierId;
            statisticItem.SaleZoneId = rawItem.SaleZoneId;
            statisticItem.SupplierZoneId = rawItem.SupplierZoneId;
            statisticItem.PortIn = rawItem.PortIn;
            statisticItem.PortOut = rawItem.PortOut;
            return statisticItem;
        }

        protected override void GetRawItemBatchTimeRange(TrafficStatisticByInterval rawItem, out DateTime batchStart, out DateTime batchEnd)
        {
            batchStart =rawItem.FirstCDRAttempt.Date;
            batchEnd = batchStart.AddHours(24);
        }

        protected override string GetStatisticItemKey(TrafficStatisticDaily statisticItem)
        {
            return BaseTrafficStatistic.GetStatisticItemKey(statisticItem.CustomerId, statisticItem.SupplierId, statisticItem.SaleZoneId, statisticItem.SupplierZoneId, statisticItem.PortOut, statisticItem.PortIn);
        }

        protected override string GetStatisticItemKey(TrafficStatisticByInterval rawItem)
        {
            return BaseTrafficStatistic.GetStatisticItemKey(rawItem.CustomerId, rawItem.SupplierId, rawItem.SaleZoneId, rawItem.SupplierZoneId, rawItem.PortOut, rawItem.PortIn);
        }

        protected override Dictionary<string, long> GetStatisticItemsIdsByKeyFromDB(TrafficStatisticDailyBatch batch)
        {
            ITrafficStatisticDailyDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<ITrafficStatisticDailyDataManager>();
            return dataManager.GetStatisticItemsIdsByKeyFromDB(batch);
        }

        protected override void InsertStatisticItemsToDB(IEnumerable<TrafficStatisticDaily> items)
        {
            ITrafficStatisticDailyDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<ITrafficStatisticDailyDataManager>();
            dataManager.InsertStatisticItemsToDB(items);
        }

        protected override void UpdateStatisticItemFromRawItem(TrafficStatisticDaily statisticItem, TrafficStatisticByInterval item)
        {
            statisticItem.Attempts++;
            statisticItem.TotalDurationInSeconds += item.TotalDurationInSeconds;
            statisticItem.FirstCDRAttempt = item.FirstCDRAttempt <= statisticItem.FirstCDRAttempt || statisticItem.FirstCDRAttempt == default(DateTime) ? item.FirstCDRAttempt : statisticItem.FirstCDRAttempt;
            statisticItem.LastCDRAttempt = item.LastCDRAttempt >= statisticItem.LastCDRAttempt ? item.LastCDRAttempt : statisticItem.LastCDRAttempt;
            statisticItem.NumberOfCalls++;
            if (item.TotalDurationInSeconds > 0)
            {
                statisticItem.SuccessfulAttempts++;
                statisticItem.DeliveredAttempts++;
                statisticItem.PDDInSeconds = item.PDDInSeconds > 0 ? (int)statisticItem.PDDInSeconds + item.PDDInSeconds :  statisticItem.PDDInSeconds;
            }
            statisticItem.MaxDurationInSeconds = item.MaxDurationInSeconds >= statisticItem.MaxDurationInSeconds ? item.MaxDurationInSeconds : statisticItem.MaxDurationInSeconds;

  
        }

        protected override void UpdateStatisticItemsInDB(IEnumerable<TrafficStatisticDaily> items)
        {
            ITrafficStatisticDailyDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<ITrafficStatisticDailyDataManager>();
            dataManager.UpdateStatisticItemsInDB(items);
        }

    }
}
