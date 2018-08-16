using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPTimeSubscriptionDataManager : IDataManager
    {
        IEnumerable<BPTimeSubscription> GetDueBPTimeSubscriptions();

        bool DeleteBPTimeSubscription(long bpTimeSubscriptionId);

        int InsertBPTimeSubscription(long processInstanceId, string bookmarkName, TimeSpan delay, BPTimeSubscriptionPayload payload);
    }
}