using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface ILiveBalanceDataManager : IDataManager
    {
        LiveBalance GetLiveBalance(Guid accountTypeId, String accountId);
        void GetLiveBalanceAccounts(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady);
        IEnumerable<Vanrise.AccountBalance.Entities.AccountBalance> GetFilteredAccountBalances(AccountBalanceQuery query);
        bool UpdateLiveBalancesFromBillingTransactions(IEnumerable<LiveBalanceToUpdate> liveBalancesToUpdate, IEnumerable<long> billingTransactionIds, IEnumerable<long> accountUsageIdsToOverride, IEnumerable<AccountUsageOverride> accountUsageOverrides, IEnumerable<long> overridenUsageIdsToRollback, IEnumerable<long> deletedTransactionIds);
        IEnumerable<LiveBalanceAccountInfo> GetLiveBalanceAccountsInfo(Guid accountTypeId);
        LiveBalanceAccountInfo TryAddLiveBalanceAndGet(String accountId, Guid accountTypeId, decimal initialBalance, int currencyId, decimal currentBalance, DateTime? bed, DateTime? eed, VRAccountStatus status, bool isDeleted);
        bool UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate, IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate, Guid? correctionProcessId);
        void UpdateBalanceRuleInfos(List<LiveBalanceNextThresholdUpdateEntity> updateEntities);
        void UpdateBalanceLastAlertInfos(List<LiveBalanceLastThresholdUpdateEntity> updateEntities);
        void GetLiveBalancesToAlert(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady);
        void GetLiveBalancesToClearAlert(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady);
        bool CheckIfAccountHasTransactions(Guid accountTypeId, String accountId);
        bool TryUpdateLiveBalanceStatus(String accountId, Guid accountTypeId, DateTime? bed, DateTime? eed, VRAccountStatus status, bool isDeleted);
    }
}
