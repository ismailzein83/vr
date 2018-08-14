using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPTimeSubscriptionDataManager : IDataManager
    {
        IEnumerable<BPTimeSubscription> GetBPTimeSubscriptions();

        bool DeleteBPTimeSubscription(long bpTimeSubscriptionId);

        int InsertBPTimeSubscription(long processInstanceId, string bookmarkName, DateTime dueTime);
    }
}