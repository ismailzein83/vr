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
        //void CreateQueue(string queueName, string title, string itemFQTN, QueueSettings settings, IEnumerable<int> sourceQueueIds);

        int InsertOrUpdateQueueItemType(string itemFQTN, string title, QueueSettings defaultQueueSettings );

        int InsertQueueInstance(int executionFlowId, string stageName, string queueName, string title, QueueInstanceStatus status, int itemTypeId, QueueSettings settings);

        bool UpdateQueueInstance(string queueName, string stageName, string title, QueueSettings settings);

        void UpdateQueueInstanceStatus(string queueName, QueueInstanceStatus status);

        bool UpdateQueueName(string queueName, QueueInstanceStatus status, string newQueueName);

        void InsertSubscription(IEnumerable<int> sourceQueueIds, int susbscribedQueueId);

        List<QueueSubscription> GetSubscriptions();

        object GetSubscriptionsMaxTimestamp();

        bool HaveSubscriptionsChanged(object timestampToCompare);

        QueueInstance GetQueueInstance(string queueName);

        List<QueueInstance> GetQueueInstances(IEnumerable<int> queueIds);

        List<QueueItemType> GetQueueItemTypes();

        List<QueueInstance> GetQueueInstancesByTypes(IEnumerable<int> queueItemTypes);
    }
}