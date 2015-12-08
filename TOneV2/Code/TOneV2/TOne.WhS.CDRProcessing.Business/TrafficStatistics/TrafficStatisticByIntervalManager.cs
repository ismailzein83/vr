using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Data;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business
{
    public class TrafficStatisticByIntervalManager : BaseTrafficStatisticManager<TrafficStatisticByInterval, TrafficStatisticByIntervalBatch>
    {
        int _intervalInMinute;

        public TrafficStatisticByIntervalManager(int intervalInMinute)
        {
            this._intervalInMinute = intervalInMinute;
        }

        protected override void GetRawItemBatchTimeRange(BillingCDRBase rawItem, out DateTime batchStart, out DateTime batchEnd)
        {
            DateTime cdrTime = rawItem.Attempt;
            batchStart = new DateTime(cdrTime.Year, cdrTime.Month, cdrTime.Day, cdrTime.Hour, ((int)(cdrTime.Minute / _intervalInMinute)) * _intervalInMinute, 0);
            batchEnd = batchStart.AddMinutes(_intervalInMinute);
        }

        protected override void InsertStatisticItemsToDB(IEnumerable<TrafficStatisticByInterval> items)
        {
              ITrafficStatisticByIntervalDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<ITrafficStatisticByIntervalDataManager>();
              dataManager.InsertStatisticItemsToDB(items);
        }

        protected override void UpdateStatisticItemsInDB(IEnumerable<TrafficStatisticByInterval> items)
        {
            ITrafficStatisticByIntervalDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<ITrafficStatisticByIntervalDataManager>();
            dataManager.UpdateStatisticItemsInDB(items);
        }

        protected override Dictionary<string, long> GetStatisticItemsIdsByKeyFromDB(TrafficStatisticByIntervalBatch batch)
        {
            ITrafficStatisticByIntervalDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<ITrafficStatisticByIntervalDataManager>();
            return dataManager.GetStatisticItemsIdsByKeyFromDB(batch);
        }
        protected override string GetStatisticItemKey(TrafficStatisticByInterval statisticItem)
        {
            return TrafficStatisticByInterval.GetStatisticItemKey(statisticItem.CustomerId, statisticItem.SupplierId, statisticItem.SaleZoneId, statisticItem.SupplierZoneId, statisticItem.PortOut, statisticItem.PortIn, statisticItem.SwitchID);
        }

        protected override string GetStatisticItemKey(BillingCDRBase rawItem)
        {
            return TrafficStatisticByInterval.GetStatisticItemKey(rawItem.CustomerId, rawItem.SupplierId, rawItem.SaleZoneID, rawItem.SupplierZoneID, rawItem.PortOut, rawItem.PortIn, rawItem.SwitchID);
        }

        protected override TrafficStatisticByInterval CreateStatisticItemFromRawItem(BillingCDRBase rawItem)
        {
            TrafficStatisticByInterval statisticItem = new TrafficStatisticByInterval();
            statisticItem.CustomerId = rawItem.CustomerId;
            statisticItem.SupplierId = rawItem.SupplierId;
            statisticItem.SaleZoneId = rawItem.SaleZoneID;
            statisticItem.SupplierZoneId = rawItem.SupplierZoneID;
            statisticItem.PortIn = rawItem.PortIn;
            statisticItem.PortOut = rawItem.PortOut;
            statisticItem.SwitchID = rawItem.SwitchID;
            return statisticItem;
        }
    }
}
