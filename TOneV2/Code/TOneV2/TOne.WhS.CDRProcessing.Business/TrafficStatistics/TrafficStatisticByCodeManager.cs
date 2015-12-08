using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Data;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business
{
    public class TrafficStatisticByCodeManager : BaseTrafficStatisticManager<TrafficStatisticByCode, TrafficStatisticByCodeBatch>
    {
        int _intervalInMinute;

        public TrafficStatisticByCodeManager(int intervalInMinute)
        {
            this._intervalInMinute = intervalInMinute;
        }

        protected override void GetRawItemBatchTimeRange(BillingCDRBase rawItem, out DateTime batchStart, out DateTime batchEnd)
        {
            DateTime cdrTime = rawItem.Attempt;
            batchStart = new DateTime(cdrTime.Year, cdrTime.Month, cdrTime.Day, cdrTime.Hour, ((int)(cdrTime.Minute / _intervalInMinute)) * _intervalInMinute, 0);
            batchEnd = batchStart.AddMinutes(_intervalInMinute);
        }

        protected override Dictionary<string, long> GetStatisticItemsIdsByKeyFromDB(TrafficStatisticByCodeBatch batch)
        {
            ITrafficStatisticByCodeDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<ITrafficStatisticByCodeDataManager>();
            return dataManager.GetStatisticItemsIdsByKeyFromDB(batch);
        }

        protected override void InsertStatisticItemsToDB(IEnumerable<TrafficStatisticByCode> items)
        {
            ITrafficStatisticByCodeDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<ITrafficStatisticByCodeDataManager>();
            dataManager.InsertStatisticItemsToDB(items);
        }

        protected override void UpdateStatisticItemsInDB(IEnumerable<TrafficStatisticByCode> items)
        {
            ITrafficStatisticByCodeDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<ITrafficStatisticByCodeDataManager>();
            dataManager.UpdateStatisticItemsInDB(items);
        }

        protected override string GetStatisticItemKey(TrafficStatisticByCode statisticItem)
        {
            return TrafficStatisticByCode.GetStatisticItemKey(statisticItem.CustomerId, statisticItem.SupplierId, statisticItem.SaleZoneId, statisticItem.SupplierZoneId, statisticItem.PortOut, statisticItem.PortIn, statisticItem.SwitchID, statisticItem.SaleCode, statisticItem.SupplierCode);
        }

        protected override string GetStatisticItemKey(BillingCDRBase rawItem)
        {
            return TrafficStatisticByCode.GetStatisticItemKey(rawItem.CustomerId, rawItem.SupplierId, rawItem.SaleZoneID, rawItem.SupplierZoneID, rawItem.PortOut, rawItem.PortIn, rawItem.SwitchID, rawItem.SaleCode, rawItem.SupplierCode);
        }

        protected override TrafficStatisticByCode CreateStatisticItemFromRawItem(BillingCDRBase rawItem)
        {
            TrafficStatisticByCode statisticItem = new TrafficStatisticByCode();
            statisticItem.CustomerId = rawItem.CustomerId;
            statisticItem.SupplierId = rawItem.SupplierId;
            statisticItem.SaleZoneId=rawItem.SaleZoneID;
            statisticItem.SupplierZoneId=rawItem.SupplierZoneID;
            statisticItem.PortIn=rawItem.PortIn;
            statisticItem.PortOut = rawItem.PortOut;
            statisticItem.SwitchID = rawItem.SwitchID;
            statisticItem.SaleCode = rawItem.SaleCode;
            statisticItem.SupplierCode = rawItem.SupplierCode;
            return statisticItem;
        }
    }
}
