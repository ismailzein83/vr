using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data
{
    public interface IQueueItemHeaderDataManager : IDataManager
    {
        Entities.QueueItemHeader Get(Guid itemId);

        void Insert(Guid itemId, int queueId, string description, Entities.QueueItemStatus queueItemStatus);

        void UpdateStatus(Guid itemId, QueueItemStatus queueItemStatus);

        void Update(Guid itemId, QueueItemStatus queueItemStatus, int retryCount, string errorMessage);
    }
}
