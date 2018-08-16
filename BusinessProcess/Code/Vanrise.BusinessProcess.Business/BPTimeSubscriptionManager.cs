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

        public int InsertBPTimeSubscription(long processInstanceId, string bookmarkName, TimeSpan delay, BPTimeSubscriptionPayload payload = null)
        {
            IBPTimeSubscriptionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTimeSubscriptionDataManager>();
            return dataManager.InsertBPTimeSubscription(processInstanceId, bookmarkName, delay, payload);
        }

        public static string GetWFBookmark(long processInstanceId)
        {
            return string.Format("Delay_{0}_{1}", processInstanceId, Guid.NewGuid());
        }
    }
}