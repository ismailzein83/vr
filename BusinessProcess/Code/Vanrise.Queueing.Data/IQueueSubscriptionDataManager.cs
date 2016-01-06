using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data
{
    public interface IQueueSubscriptionDataManager : IDataManager
    {
        void InsertSubscription(IEnumerable<int> sourceQueueIds, int susbscribedQueueId);

        List<QueueSubscription> GetSubscriptions();

        bool AreSubscriptionsUpdated(ref object updateHandle);
    }
}
