﻿using System;
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
        LiveBalanceAccountInfo TryAddLiveBalanceAndGet(String accountId, Guid accountTypeId, decimal initialBalance, int currencyId, decimal currentBalance);
        bool UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate, IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate, Guid? correctionProcessId);
       
        void UpdateBalanceRuleInfos(List<LiveBalanceNextThresholdUpdateEntity> updateEntities);
        void UpdateBalanceLastAlertInfos(List<LiveBalanceLastThresholdUpdateEntity> updateEntities);
        void GetLiveBalancesToAlert(Action<LiveBalance> onLiveBalanceReady);
        void GetLiveBalancesToClearAlert(Action<LiveBalance> onLiveBalanceReady);
        bool CheckIfAccountHasTransactions(Guid accountTypeId, String accountId);
    }
}
