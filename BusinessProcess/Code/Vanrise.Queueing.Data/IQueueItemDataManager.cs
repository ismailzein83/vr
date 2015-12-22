using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data
{
    public interface IQueueItemDataManager : IDataManager
    {
        void CreateQueue(int queueId);

        long GenerateItemID();

        void EnqueueItem(int queueId, long itemId, long executionFlowTriggerItemId, byte[] item, string description, QueueItemStatus queueItemStatus);

        void EnqueueItem(Dictionary<int, long> targetQueuesItemsIds, int sourceQueueId, long sourceItemId, long executionFlowTriggerItemId, byte[] item, string description, QueueItemStatus queueItemStatus);

        QueueItem DequeueItem(int queueId, int currentProcessId, IEnumerable<int> runningProcessesIds, bool singleQueueReader);

        void DeleteItem(int queueId, long itemId);

        Entities.QueueItemHeader GetHeader(long itemId, int queueId);

        void UpdateHeaderStatus(long itemId, QueueItemStatus queueItemStatus);

        void UpdateHeader(long itemId, QueueItemStatus queueItemStatus, int retryCount, string errorMessage);

        void UnlockItem(int queueId, long itemId, bool isSuspended);

        List<QueueItemHeader> GetHeaders(IEnumerable<int> queueIds, IEnumerable<QueueItemStatus> statuses, DateTime dateFrom, DateTime dateTo);

        List<ItemExecutionFlowInfo> GetItemExecutionFlowInfo(List<long> itemIds);

        List<QueueItemHeader> GetQueueItemsHeader(List<long> itemIds);
    }
}