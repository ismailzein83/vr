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
        void GetLiveBalanceAccounts(Action<LiveBalance> onLiveBalanceReady);
        void GetLiveBalancesToAlert(Action<LiveBalance> onLiveBalanceReady);
        bool UpdateLiveBalanceFromBillingTransaction(long accountId, List<long> billingTransactionIds, decimal amount);
        bool UpdateLiveBalanceFromBalanceUsageQueue(IEnumerable<UsageBalanceUpdate> groupedResult, long balanceUsageQueueId);
        IEnumerable<LiveBalanceAccountInfo> GetLiveBalanceAccountsInfo();
        bool AddLiveBalance(long accountId, decimal initialBalance, int currencyId, decimal usageBalance, decimal currentBalance);
        bool UpdateLiveBalanceThreshold(List<BalanceAccountThreshold> balanceAccountsThresholds);
        void UpdateLiveBalanceAlertRule(List<AccountBalanceAlertRule> accountBalanceAlertRules);
    }
}
