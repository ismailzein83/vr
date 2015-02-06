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
        void CreateQueue(string queueName, string title, string itemFQTN, QueueSettings settings, IEnumerable<int> sourceQueueIds);

        List<QueueSubscription> GetSubscriptions();

        object GetSubscriptionsMaxTimestamp();

        bool HaveSubscriptionsChanged(object timestampToCompare);

        QueueInstance GetQueueInstance(string queueName);
    }
}
