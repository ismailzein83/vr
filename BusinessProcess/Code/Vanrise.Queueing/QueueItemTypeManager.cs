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
        public IEnumerable<QueueItemTypeInfo> GetItemTypes(QueueItemTypeFilter filter)
        {
            IQueueItemTypeDataManager manager = QDataManagerFactory.GetDataManager<IQueueItemTypeDataManager>();
            IEnumerable<QueueItemType> itemTypes = manager.GetQueueItemTypes();

            return itemTypes.MapRecords(ItemTypesInfoMapper, null);

        }

        Dictionary<int, QueueItemType> GetCachedQueueItemTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceCacheManager>().GetOrCreateObject("QueueItemTypeManager_GetCachedQueueItemTypes",
               () =>
               {
                   IQueueItemTypeDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueItemTypeDataManager>();
                   IEnumerable<QueueItemType> queueItemTypes = dataManager.GetQueueItemTypes();
                   return queueItemTypes.ToDictionary(kvp => kvp.Id, kvp => kvp);
               });
        }

        public string GetItemTypeName(int itemTypeId)
        {
            QueueItemType queueItemType = GetQueueItemType(itemTypeId);
            return queueItemType != null ? queueItemType.Title : null;
        }


        private QueueItemType GetQueueItemType(int queueItemTypeId)
        {
            var queueItemTypes = GetCachedQueueItemTypes();
            return queueItemTypes.GetRecord(queueItemTypeId);
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



        #region Mappers

        private QueueItemTypeInfo ItemTypesInfoMapper(QueueItemType itemType)
        {
            QueueItemTypeInfo itemTypeInfo = new QueueItemTypeInfo();
            itemTypeInfo.Id = itemType.Id;
            itemTypeInfo.Title = itemType.Title;
            return itemTypeInfo;
        }

        #endregion


    }


}