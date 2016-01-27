﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Common;

namespace Vanrise.Queueing
{
    public class QueueingManager
    {
        private readonly IQueueDataManager _dataManager;
        private readonly IQueueItemDataManager _itemDataManager;

        public QueueingManager()
        {
            _dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            _itemDataManager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
        }

        public QueueInstance GetQueueInstance(string queueName)
        {
            return GetCachedQueueInstancesByName().GetRecord(queueName);
        }

        public List<QueueItemType> GetQueueItemTypes()
        {
            return _dataManager.GetQueueItemTypes();
        }

        public IEnumerable<QueueInstance> GetQueueInstances(IEnumerable<int> queueItemTypes)
        {
            var readyQueueInstances = GetReadyQueueInstances();
            if (readyQueueInstances == null)
                return null;
            return readyQueueInstances.Where(itm => queueItemTypes == null || queueItemTypes.Contains(itm.ItemTypeId));
        }

        public List<QueueItemHeader> GetHeaders(IEnumerable<int> queueIds, IEnumerable<QueueItemStatus> statuses, DateTime dateFrom, DateTime dateTo)
        {
            return _itemDataManager.GetHeaders(queueIds, statuses, dateFrom, dateTo);
        }

        public Dictionary<long, ItemExecutionFlowInfo> GetItemsExecutionFlowStatus(List<long> itemIds)
        {
            List<ItemExecutionFlowInfo> originalList = _itemDataManager.GetItemExecutionFlowInfo(itemIds);

            var subLists = originalList.GroupBy(p => p.ItemId)
                        .Select(g => g.ToList());

            Dictionary<long, ItemExecutionFlowInfo> itemsExectionStatusDic = new Dictionary<long, ItemExecutionFlowInfo>();

            foreach (List<ItemExecutionFlowInfo> list in subLists)
            {
                ItemExecutionFlowInfo resultItem =
                        new ItemExecutionFlowInfo() { ItemId = list[0].ItemId, ExecutionFlowTriggerItemId = list[0].ExecutionFlowTriggerItemId };

                resultItem.Status = this.GetExecutionFlowStatus(list);
                itemsExectionStatusDic.Add(list[0].ItemId, resultItem);
            }

            return itemsExectionStatusDic;
        }

        public ItemExecutionFlowStatus GetExecutionFlowStatus(List<ItemExecutionFlowInfo> list)
        {
            ItemExecutionFlowStatus result = ItemExecutionFlowStatus.New;

            int statusNewCount = 0;
            int statusProcessedCount = 0;
            int statusItemFailedCount = 0;
            int statusItemProcessingCount = 0;
            int statusItemSuspendedCount = 0;

            foreach (ItemExecutionFlowInfo item in list)
            {
                switch (item.Status)
                {
                    case ItemExecutionFlowStatus.New:
                        statusNewCount++;
                        break;
                    case ItemExecutionFlowStatus.Processing:
                        statusItemProcessingCount++;
                        break;
                    case ItemExecutionFlowStatus.Failed:
                        statusItemFailedCount++;
                        break;
                    case ItemExecutionFlowStatus.Suspended:
                        statusItemSuspendedCount++;
                        break;
                    default:
                        statusProcessedCount++;
                        break;
                }
            }

            if (statusNewCount == list.Count)
                result = ItemExecutionFlowStatus.New;
            else if ((statusItemSuspendedCount + statusProcessedCount) == list.Count)
            {
                result = statusItemSuspendedCount > 0 ? ItemExecutionFlowStatus.Suspended : ItemExecutionFlowStatus.Processed;
            }
            else if (statusItemFailedCount > 0)
                result = ItemExecutionFlowStatus.Failed;
            else
                result = ItemExecutionFlowStatus.Processing;

            return result;
        }

        public Vanrise.Entities.IDataRetrievalResult<QueueItemHeader> GetQueueItemsHeader(Vanrise.Entities.DataRetrievalInput<List<long>> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _itemDataManager.GetQueueItemsHeader(input));
        }

        private QueueItemStatus GetSubStatus(List<ItemExecutionFlowInfo> subList)
        {
            return QueueItemStatus.Processed;
        }

        public IEnumerable<QueueInstance> GetReadyQueueInstances()
        {
            var cachedQueues = GetCachedQueueInstances();
            if (cachedQueues != null)
                return cachedQueues.Values.FindAllRecords(itm => itm.Status == QueueInstanceStatus.ReadyToUse);
            else
                return null;
        }

        #region Private Classes

        Dictionary<string, QueueInstance> GetCachedQueueInstancesByName()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceCacheManager>().GetOrCreateObject("GetCachedQueueInstancesByName",
              () =>
              {
                  IQueueDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
                  IEnumerable<QueueInstance> queueInstances = dataManager.GetAllQueueInstances();
                  return queueInstances.ToDictionary(kvp => kvp.Name, kvp => kvp);
              });
        }

        Dictionary<int, QueueInstance> GetCachedQueueInstances()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QueueInstanceCacheManager>().GetOrCreateObject("GetCachedQueueInstances",
               () =>
               {
                   IQueueDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
                   IEnumerable<QueueInstance> queueInstances = dataManager.GetAllQueueInstances();
                   return queueInstances.ToDictionary(kvp => kvp.QueueInstanceId, kvp => kvp);
               });
        }

        #endregion
    }

    internal class QueueInstanceCacheManager : Vanrise.Caching.BaseCacheManager
    {
        IQueueDataManager _queueDataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
        IQueueExecutionFlowDataManager _queueExecutionFlowDataManager = QDataManagerFactory.GetDataManager<IQueueExecutionFlowDataManager>();
        object _queueUpdateHandle;
        object _queueExecutionFlowUpdateHandle;

        public override Caching.CacheObjectSize ApproximateObjectSize
        {
            get
            {
                return Caching.CacheObjectSize.ExtraSmall;
            }
        }
        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return _queueDataManager.AreQueuesUpdated(ref _queueUpdateHandle) || _queueExecutionFlowDataManager.AreExecutionFlowsUpdated(ref _queueExecutionFlowUpdateHandle);
        }
    }
}
