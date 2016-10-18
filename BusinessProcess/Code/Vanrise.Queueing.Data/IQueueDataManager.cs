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
        List<QueueInstance> GetAllQueueInstances();

        int InsertOrUpdateQueueItemType(string itemFQTN, string title, QueueSettings defaultQueueSettings );

        int InsertQueueInstance(Guid executionFlowId, string stageName, string queueName, string title, QueueInstanceStatus status, int itemTypeId, QueueSettings settings);

        bool UpdateQueueInstance(string queueName, string stageName, string title, QueueSettings settings);

        void UpdateQueueInstanceStatus(string queueName, QueueInstanceStatus status);

        bool UpdateQueueName(string queueName, QueueInstanceStatus status, string newQueueName);
        
        List<QueueItemType> GetQueueItemTypes();

        bool AreQueuesUpdated(ref object _updateHandle);
    }
}