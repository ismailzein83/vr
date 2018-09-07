using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class QueueingManager
    {
        #region Fiedds/Ctor

        private readonly IQueueDataManager _dataManager;
        private readonly IQueueItemDataManager _itemDataManager;

        public QueueingManager()
        {
            _dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            _itemDataManager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
        }

        #endregion

        #region Public Methods

        public List<QueueItemType> GetQueueItemTypes()
        {
            return _dataManager.GetQueueItemTypes();
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

        public Vanrise.Entities.IDataRetrievalResult<QueueItemHeaderDetails> GetQueueItemsHeader(Vanrise.Entities.DataRetrievalInput<List<long>> input)
        {
            var data = _itemDataManager.GetQueueItemsHeader(input);
            Vanrise.Entities.BigResult<QueueItemHeaderDetails> returnedResult = new Vanrise.Entities.BigResult<QueueItemHeaderDetails>();
            returnedResult.ResultKey = data.ResultKey;
            returnedResult.TotalCount = data.TotalCount;
            returnedResult.Data = data.Data.MapRecords(QueueItemHeaderManager.QueueItemHeaderDetailMapper);

            ResultProcessingHandler<QueueItemHeaderDetails> handler = new ResultProcessingHandler<QueueItemHeaderDetails>()
            {
                ExportExcelHandler = new QueueItemHeaderManager.QueueItemHeaderExportHandler()
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, returnedResult, handler);
        }
              
        #endregion
    }
}