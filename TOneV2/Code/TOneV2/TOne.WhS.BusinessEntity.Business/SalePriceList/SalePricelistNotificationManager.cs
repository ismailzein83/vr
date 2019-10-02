using System;
using Vanrise.Common;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SalePricelistNotificationManager
    {
        #region public Methods
        public bool Insert(int customerId, int pricelitsId, long fileId)
        {
            ISalePricelistNotificationDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePricelistNotificationDataManager>();
            return dataManager.Insert(customerId, pricelitsId, fileId);
        }
        public IEnumerable<SalePricelistNotification> GetSalePricelistNotifications()
        {
            ISalePricelistNotificationDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePricelistNotificationDataManager>();
            return dataManager.GetSalePricelistNotifications();
        }
        public IEnumerable<SalePricelistNotificationDetail> GetSalePricelistNotification(int pricelistId)
        {
            var salePricelistNotificationDetails = new List<SalePricelistNotificationDetail>();
            ISalePricelistNotificationDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePricelistNotificationDataManager>();
            var notifications = dataManager.GetSalePricelistNotifictaions(pricelistId);
            var fileManager = new VRFileManager();

            foreach (var notification in notifications)
            {
                var pricelistDetail = new SalePricelistNotificationDetail
                {
                    EmailCreationDate = notification.EmailCreationDate,
                    FileId = notification.FileId
                };
                var file = fileManager.GetFile(notification.FileId);
                if (file != null)
                    pricelistDetail.FileName = file.Name;
                salePricelistNotificationDetails.Add(pricelistDetail);
            }
            return salePricelistNotificationDetails;
        }


        public int? GetSalePricelistNotificationCount(int pricelistId)
        {
            Dictionary<int, int> cachedNotificationCounts = GetCachedNotificationCountByPricelistId();
            if (cachedNotificationCounts.TryGetValue(pricelistId, out int notificationCount))
            {
                return notificationCount;
            }
            return null;
        }
        public IEnumerable<SalePricelistNotification> GetLastSalePricelistNotification(IEnumerable<int> customerIds)
        {
            ISalePricelistNotificationDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePricelistNotificationDataManager>();
            return dataManager.GetLastSalePricelistNotifications(customerIds);
        }
        #endregion

        #region private Methods
        private Dictionary<int, int> GetCachedNotificationCountByPricelistId()
        {
            ISalePricelistNotificationDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePricelistNotificationDataManager>();

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetCachedNotificationCountByPricelistId"), () =>
            {
                return dataManager.GetNotificationCountByPricelistId();

            });
        }
        #endregion

        #region private classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISalePricelistNotificationDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISalePricelistNotificationDataManager>();
            object _updateHandle;

            public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
            {
                get
                {
                    return Vanrise.Caching.CacheObjectSize.Large;
                }
            }

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSalePriceListNotificationUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
}
