using InterConnect.BusinessEntity.Data;
using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.SummaryTransformation;

namespace InterConnect.BusinessEntity.Business.SummaryTesting
{
    public class TrafficStatsManager : Vanrise.Common.Business.SummaryTransformation.SummaryTransformationManager<dynamic, TrafficStats, TrafficStatsBatch>
    {
        ITrafficStatsDataManager _dataManager = BEDataManagerFactory.GetDataManager<ITrafficStatsDataManager>();

        protected override IEnumerable<TrafficStats> GetItemsFromDB(DateTime batchStart)
        {
            return _dataManager.GetTrafficStats(batchStart);
        }

        protected override void GetRawItemBatchTimeRange(dynamic rawItem, out DateTime batchStart)
        {
            DateTime cdrTime = rawItem.AttemptTime;
            batchStart = new DateTime(cdrTime.Year, cdrTime.Month, cdrTime.Day, cdrTime.Hour, ((int)(cdrTime.Minute / 10)) * 10, 0);
        }

        protected override string GetSummaryItemKey(TrafficStats summaryItem)
        {
            return String.Format("{0}_{1}", summaryItem.OperatorId, summaryItem.CDPN).ToLower();
        }

        protected override string GetSummaryItemKey(dynamic rawItem)
        {
            return String.Format("{0}_{1}", rawItem.OperatorAccount, rawItem.CDPN).ToLower();
        }

        protected override void InsertItemsToDB(List<TrafficStats> itemsToAdd)
        {
            _dataManager.Insert(itemsToAdd);
        }

        protected override void SetSummaryItemGroupingFields(TrafficStats summaryItem, dynamic item)
        {
            summaryItem.OperatorId = item.OperatorAccount;
            summaryItem.CDPN = item.CDPN;
        }

        protected override void UpdateItemsInDB(List<TrafficStats> itemsToUpdate)
        {
            _dataManager.Update(itemsToUpdate);
        }

        protected override void UpdateSummaryItemFromRawItem(TrafficStats summaryItem, dynamic item)
        {
            summaryItem.Attempts++;
            summaryItem.TotalDuration += item.DurationInSec;
        }

        protected override void UpdateSummaryItemFromSummaryItem(TrafficStats existingItem, TrafficStats newItem)
        {
            existingItem.Attempts += newItem.Attempts;
            existingItem.TotalDuration += newItem.TotalDuration;
        }

    }
}
