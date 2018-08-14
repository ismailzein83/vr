using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPTimeSubscriptionManager
    {
        public IEnumerable<BPTimeSubscription> GetBPTimeSubscriptions()
        {
            IBPTimeSubscriptionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTimeSubscriptionDataManager>();
            return dataManager.GetBPTimeSubscriptions();
        }

        public bool DeleteBPTimeSubscription(long bpTimeSubscriptionId)
        {
            IBPTimeSubscriptionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTimeSubscriptionDataManager>();
            return dataManager.DeleteBPTimeSubscription(bpTimeSubscriptionId);
        }

        public int InsertBPTimeSubscription(long processInstanceId, string bookmarkName, DateTime dueDate)
        {
            IBPTimeSubscriptionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTimeSubscriptionDataManager>();
            return dataManager.InsertBPTimeSubscription(processInstanceId, bookmarkName, dueDate);
        }
    }
}