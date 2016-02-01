using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Common;

namespace Vanrise.Queueing
{
    public class QueueItemTypeManager
    {
        public List<QueueItemType> GetItemTypes()
        {
            IQueueItemTypeDataManager manager = QDataManagerFactory.GetDataManager<IQueueItemTypeDataManager>();
            return manager.GetItemTypes();
            
        }


        Dictionary<int, QueueItemType> GetCachedQueueItemTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceCacheManager>().GetOrCreateObject("QueueItemTypeManager_GetCachedQueueItemTypes",
               () =>
               {
                   IQueueItemTypeDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueItemTypeDataManager>();
                   List<QueueItemType>  queueItemTypes = dataManager.GetItemTypes();
                   return queueItemTypes.ToDictionary(kvp => kvp.Id, kvp => kvp);
               });
        }

        private QueueItemType GetQueueItemType(int queueItemTypeId)
        {
            var queueItemTypes = GetCachedQueueItemTypes();
            return queueItemTypes.GetRecord(queueItemTypeId);
        }

        public string GetItemTypeName(int itemTypeId)
        {
            QueueItemType queueItemType = GetQueueItemType(itemTypeId);
            return queueItemType != null ? queueItemType.Title : null;
        }

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IQueueItemTypeDataManager _dataManager = QDataManagerFactory.GetDataManager<IQueueItemTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreItemTypeUpdated(ref _updateHandle);
            }
        }


        #endregion

        
    }

     
}