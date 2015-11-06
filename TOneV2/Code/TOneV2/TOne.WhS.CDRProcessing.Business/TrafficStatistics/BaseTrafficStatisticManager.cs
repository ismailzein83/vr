using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business
{
    public abstract class BaseTrafficStatisticManager<T> : Vanrise.Common.Business.StatisticManagement.StatisticBatchManager<BillingCDRBase, T, TrafficStatisticBatch<T>>
        where T : BaseTrafficStatistic
    {
        protected override T CreateStatisticItemFromRawItem(BillingCDRBase rawItem)
        {
            T statisticItem = Activator.CreateInstance<T>();
            statisticItem.CustomerId = rawItem.CustomerId;
            statisticItem.SupplierId = rawItem.SupplierId;
            return statisticItem;
        }

        protected override string GetStatisticItemKey(T statisticItem)
        {
            return BaseTrafficStatistic.GetStatisticItemKey(statisticItem.CustomerId, statisticItem.SupplierId);
        }

        protected override string GetStatisticItemKey(BillingCDRBase rawItem)
        {
            return BaseTrafficStatistic.GetStatisticItemKey(rawItem.CustomerId, rawItem.SupplierId);
        }

        protected override Dictionary<string, long> GetStatisticItemsIdsByKeyFromDB(TrafficStatisticBatch<T> batch)
        {
            throw new NotImplementedException();
        }

        protected override void InsertStatisticItemsToDB(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateStatisticItemFromRawItem(T statisticItem, BillingCDRBase item)
        {
            statisticItem.Attempts++;
            statisticItem.TotalDurationInSeconds += item.DurationInSeconds;
        }

        protected override void UpdateStatisticItemsInDB(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }
    }
}
