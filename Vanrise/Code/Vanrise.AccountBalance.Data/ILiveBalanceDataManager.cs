using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface ILiveBalanceDataManager:IDataManager
    {
        LiveBalance GetLiveBalance(long accountId);
        bool UpdateLiveBalanceFromBillingTransaction(long accountId, List<long> billingTransactionIds, decimal amount);
        bool UpdateLiveBalanceFromBalanceUsageQueue(IEnumerable<UsageBalanceUpdate> groupedResult, long balanceUsageQueueId);
        IEnumerable<LiveBalanceAccountInfo> GetLiveBalanceAccountsInfo();
        bool Insert(LiveBalance liveBalance);
    }
}
