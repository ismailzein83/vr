using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPTimeSubscriptionManager
    {
        public IEnumerable<BPTimeSubscription> GetDueBPTimeSubscriptions()
        {
            IBPTimeSubscriptionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTimeSubscriptionDataManager>();
            return dataManager.GetDueBPTimeSubscriptions();
        }

        public bool DeleteBPTimeSubscription(long bpTimeSubscriptionId)
        {
            IBPTimeSubscriptionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTimeSubscriptionDataManager>();
            return dataManager.DeleteBPTimeSubscription(bpTimeSubscriptionId);
        }

        public int InsertBPTimeSubscription(long processInstanceId, string bookmarkName, TimeSpan delay)
        {
            IBPTimeSubscriptionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTimeSubscriptionDataManager>();
            return dataManager.InsertBPTimeSubscription(processInstanceId, bookmarkName, delay);
        }
    }
}