using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;

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

        public List<QueueItemType> GetQueueItemTypes()
        {
            return _dataManager.GetQueueItemTypes();
        }

        public List<QueueInstance> GetQueueInstances(IEnumerable<int> queueItemTypes)
        {
            return _dataManager.GetQueueInstancesByTypes(queueItemTypes);
        }

        public List<QueueItemHeader> GetHeaders(IEnumerable<int> queueIds, IEnumerable<QueueItemStatus> statuses, DateTime dateFrom, DateTime dateTo)
        {
            return _itemDataManager.GetHeaders(queueIds, statuses, dateFrom, dateTo);
        }

        public Dictionary<long, ItemExecutionStatus> GetItemsExecutionStatus(List<long> itemIds)
        {
            List<ItemExecutionStatus> originalList = _itemDataManager.GetItemsExecutionStatus(itemIds);

            var subLists = originalList.GroupBy(p => p.ItemId)
                        .Select(g => g.ToList());

            Dictionary<long, ItemExecutionStatus> itemsExectionStatusDic = new Dictionary<long, ItemExecutionStatus>();

            foreach (List<ItemExecutionStatus> list in subLists)
            {
                ItemExecutionStatus resultItem =
                        new ItemExecutionStatus() { ItemId = list[0].ItemId, ExecutionFlowTriggerItemId = list[0].ExecutionFlowTriggerItemId };

                resultItem.Status = this.GetExecutionStatus(list);
                itemsExectionStatusDic.Add(list[0].ItemId, resultItem);
            }

            return itemsExectionStatusDic;
        }

        public QueueItemStatus GetExecutionStatus(List<ItemExecutionStatus> list)
        {
            QueueItemStatus result = QueueItemStatus.New;

            int statusNewCount = 0;
            int statusProcessedCount = 0;
            int statusItemFailedCount = 0;
            int statusItemProcessingCount = 0;

            foreach (ItemExecutionStatus item in list)
            {
                switch (item.Status)
                {
                    case QueueItemStatus.New:
                        statusNewCount++;
                        break;
                    case QueueItemStatus.Processing:
                        statusItemProcessingCount++;
                        break;
                    case QueueItemStatus.Failed:
                        statusItemFailedCount++;
                        break;
                    default:
                        statusProcessedCount++;
                        break;
                }
            }

            if (statusNewCount == list.Count)
                result = QueueItemStatus.New;
            else if (statusProcessedCount == list.Count)
                result = QueueItemStatus.Processed;
            else if (statusItemFailedCount > 0)
                result = QueueItemStatus.Failed;
            else if (statusItemProcessingCount > 0)
                result = QueueItemStatus.Processing;


            return result;
        }

        public List<QueueItemHeader> GetQueueItemsHeader(List<long> itemIds)
        {
            return _itemDataManager.GetQueueItemsHeader(itemIds);
        }

        private QueueItemStatus GetSubStatus(List<ItemExecutionStatus> subList)
        {
            return QueueItemStatus.Processed;
        }

    }
}
