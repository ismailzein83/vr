using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data
{
    public interface IQueueDataManager : IDataManager
    {
        int GetQueue(string queueName);

        void EnqueueItem(int queueId, Guid itemId, byte[] item);
        //void DequeueItem(string queueName, TimeSpan waitTime, Action<byte[]> onItemReady);

        //QueueItem DequeueItem(int queueId, Guid processId);

        QueueItem DequeueItem(int queueId, int currentProcessId, IEnumerable<int> runningProcessesIds);

        void DeleteItem(int queueId, Guid itemId);
    }
}
