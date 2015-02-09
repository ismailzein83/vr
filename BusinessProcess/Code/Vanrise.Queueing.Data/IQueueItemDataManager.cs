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

        void InsertQueueItemIDGen(int queueId);

        long GenerateItemID(int queueId);

        void EnqueueItem(int queueId, long itemId, byte[] item, string description, QueueItemStatus queueItemStatus);

        void EnqueueItem(Dictionary<int, long> targetQueuesItemsIds, int sourceQueueId, long sourceItemId, byte[] item, string description, QueueItemStatus queueItemStatus);

        QueueItem DequeueItem(int queueId, int currentProcessId, IEnumerable<int> runningProcessesIds, bool singleQueueReader);

        void DeleteItem(int queueId, long itemId);

        Entities.QueueItemHeader GetHeader(long itemId, int queueId);

        void UpdateHeaderStatus(long itemId, QueueItemStatus queueItemStatus);

        void UpdateHeader(long itemId, QueueItemStatus queueItemStatus, int retryCount, string errorMessage);
    }
}