using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface ILiveBalanceDataManager : IDataManager
    {
        LiveBalance GetLiveBalance(Guid accountTypeId, long accountId);
        void GetLiveBalanceAccounts(Action<LiveBalance> onLiveBalanceReady);
        bool UpdateLiveBalanceFromBillingTransaction(Guid accountTypeId, long accountId, List<long> billingTransactionIds, decimal amount);
        bool UpdateLiveBalanceFromBalanceUsageQueue(Guid accountTypeId, IEnumerable<UsageBalanceUpdate> groupedResult, long balanceUsageQueueId);
        IEnumerable<LiveBalanceAccountInfo> GetLiveBalanceAccountsInfo(Guid accountTypeId);
        bool TryAddLiveBalance(long accountId, Guid accountTypeId, decimal initialBalance, int currencyId, decimal usageBalance, decimal currentBalance);
        bool UpdateLiveBalanceThreshold(Guid accountTypeId, List<BalanceAccountThreshold> balanceAccountsThresholds);
        void UpdateLiveBalanceAlertRule(List<AccountBalanceAlertRule> accountBalanceAlertRules);
        void UpdateBalanceRuleInfos(List<LiveBalanceNextThresholdUpdateEntity> updateEntities);
        void GetLiveBalancesToAlert(Action<LiveBalance> onLiveBalanceReady);
        void GetLiveBalancesToClearAlert(Action<LiveBalance> onLiveBalanceReady);
    }
}
