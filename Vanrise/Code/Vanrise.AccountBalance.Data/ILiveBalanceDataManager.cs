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
        LiveBalance GetLiveBalance(Guid accountTypeId, String accountId);
        void GetLiveBalanceAccounts(Action<LiveBalance> onLiveBalanceReady);
        IEnumerable<Vanrise.AccountBalance.Entities.AccountBalance> GetFilteredAccountBalances(AccountBalanceQuery query);
        bool UpdateLiveBalanceFromBillingTransaction(IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate, List<long> billingTransactionIds);
        IEnumerable<LiveBalanceAccountInfo> GetLiveBalanceAccountsInfo(Guid accountTypeId);
        LiveBalanceAccountInfo TryAddLiveBalanceAndGet(String accountId, Guid accountTypeId, decimal initialBalance, int currencyId, decimal usageBalance, decimal currentBalance);
        bool UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate, IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate, Guid? correctionProcessId);
        bool UpdateLiveBalanceThreshold(Guid accountTypeId, List<BalanceAccountThreshold> balanceAccountsThresholds);
        void UpdateLiveBalanceAlertRule(List<AccountBalanceAlertRule> accountBalanceAlertRules);
        void UpdateBalanceRuleInfos(List<LiveBalanceNextThresholdUpdateEntity> updateEntities);
        void UpdateBalanceLastAlertInfos(List<LiveBalanceLastThresholdUpdateEntity> updateEntities);
        void GetLiveBalancesToAlert(Action<LiveBalance> onLiveBalanceReady);
        void GetLiveBalancesToClearAlert(Action<LiveBalance> onLiveBalanceReady);
    }
}
