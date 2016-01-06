using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class QueueSubscriptionManager
    {
        public List<int> GetSubscribedQueueIds(int sourceQueueId)
        {
            string cacheName = String.Format("GetSubscribedQueueIds_{0}", sourceQueueId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
               () =>
               {
                   var allSubscriptions = GetAllSubscriptions();
                   if (allSubscriptions == null)
                       return null;
                   var subscribedQueueIds = new List<int>();
                   FillSubscribedQueueIdsFromSubscriptions(subscribedQueueIds, sourceQueueId, sourceQueueId, allSubscriptions);
                   return subscribedQueueIds;
               });
        }

        public void AddSubscriptions(List<int> sourceQueueIds, int subscribedQueueId)
        {
            IQueueSubscriptionDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueSubscriptionDataManager>();
            dataManager.InsertSubscription(sourceQueueIds, subscribedQueueId);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        void FillSubscribedQueueIdsFromSubscriptions(List<int> subscribedQueueIds, int sourceQueueId, int mainSourceQueueId, List<QueueSubscription> allSubscriptions)
        {
            foreach (var subscription in allSubscriptions.Where(itm => itm.QueueID == sourceQueueId))
            {
                if (subscription.SubsribedQueueID != mainSourceQueueId && !subscribedQueueIds.Contains(subscription.SubsribedQueueID))
                {
                    subscribedQueueIds.Add(subscription.SubsribedQueueID);
                    FillSubscribedQueueIdsFromSubscriptions(subscribedQueueIds, subscription.SubsribedQueueID, mainSourceQueueId, allSubscriptions);
                }
            }
        }

        public List<QueueSubscription> GetAllSubscriptions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSubscriptions",
               () =>
               {
                   IQueueSubscriptionDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueSubscriptionDataManager>();
                   return dataManager.GetSubscriptions();
               });
        }

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IQueueSubscriptionDataManager _dataManager = QDataManagerFactory.GetDataManager<IQueueSubscriptionDataManager>();
            object _updateHandle;

            public override Caching.CacheObjectSize ApproximateObjectSize
            {
                get
                {
                    return Caching.CacheObjectSize.ExtraSmall;
                }
            }
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSubscriptionsUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
