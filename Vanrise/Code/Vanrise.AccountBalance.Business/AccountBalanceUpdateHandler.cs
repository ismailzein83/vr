using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;

namespace Vanrise.AccountBalance.Business
{
    public class AccountBalanceUpdateHandler
    {
        static ConcurrentDictionary<Guid, AccountBalanceUpdateHandler> _handlersByAccountTypeId = new ConcurrentDictionary<Guid, AccountBalanceUpdateHandler>();
        Dictionary<long, LiveBalanceAccountInfo> AccountsInfo;
        CurrencyExchangeRateManager currencyExchangeRateManager;
        AccountManager manager;
        ILiveBalanceDataManager dataManager;
        bool LoadingError;
        Guid _accountTypeId;
        #region ctor
        private AccountBalanceUpdateHandler(Guid accountTypeId)
        {
            currencyExchangeRateManager = new CurrencyExchangeRateManager();
            dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            manager = new AccountManager();
            _accountTypeId = accountTypeId;
            IntializeAccountsInfo();
        }
        #endregion

        public static AccountBalanceUpdateHandler GetHandlerByAccountTypeId(Guid accountTypeId)
        {
            AccountBalanceUpdateHandler handler;
            if (!_handlersByAccountTypeId.TryGetValue(accountTypeId, out handler))
            {
                handler = new AccountBalanceUpdateHandler(accountTypeId);
                _handlersByAccountTypeId.TryAdd(accountTypeId, handler);
            }
            return handler;
        }

        #region Public Methods
        public void AddAndUpdateLiveBalanceFromBillingTransction(List<BillingTransaction> billingTransactions)
        {
            if (this.LoadingError)
            {
                IntializeAccountsInfo();
            }

            long accountId = billingTransactions.FirstOrDefault().AccountId;
            LiveBalanceAccountInfo accountInfo = null;
            if (!AccountsInfo.TryGetValue(accountId, out accountInfo))
            {
                accountInfo = AddLiveAccountInfo(accountId);
            }
            UpdateLiveBalanceFromBillingTransaction(billingTransactions, accountId, accountInfo);

        }
        public void AddAndUpdateLiveBalanceFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<UsageBalanceUpdate> usageBalanceUpdates)
        {
            if (this.LoadingError)
            {
                IntializeAccountsInfo();
            }
            var accountsToInsert = usageBalanceUpdates.Where(x => !AccountsInfo.ContainsKey(x.AccountId)).Distinct();
            foreach (var accountToInsert in accountsToInsert)
            {
                AddLiveAccountInfo(accountToInsert.AccountId);
            }
            UpdateLiveBalanceFromBalanceUsageQueue(balanceUsageQueueId, usageBalanceUpdates);
        }
        public bool AddLiveBalance(long accountId, int currencyId)
        {
            return dataManager.AddLiveBalance(accountId, _accountTypeId, 0, currencyId, 0, 0);
        }
        #endregion

        #region Private Methods
        private LiveBalanceAccountInfo AddLiveAccountInfo(long accountId)
        {
            var account = manager.GetAccountInfo(accountId);
            var accountInfo = new LiveBalanceAccountInfo { AccountId = accountId, CurrencyId = account.CurrencyId };

            if (AddLiveBalance(accountId, accountInfo.CurrencyId))
            {
                AccountsInfo.Add(accountId, accountInfo);
                return accountInfo;
            }
            else
            {
                LoadingError = true;
                throw new Exception(string.Format("Same account id {0} Exist.", accountId));
            }
        }
        private void IntializeAccountsInfo()
        {
            AccountsInfo = new Dictionary<long, LiveBalanceAccountInfo>();
            var accountBlances = dataManager.GetLiveBalanceAccountsInfo(_accountTypeId);
            AccountsInfo = accountBlances.ToDictionary(x => x.AccountId, x => x);
        }
        private bool UpdateLiveBalanceFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<UsageBalanceUpdate> usageBalanceUpdates)
        {
            foreach (var itm in usageBalanceUpdates)
            {
                LiveBalanceAccountInfo accountInfo = null;
                AccountsInfo.TryGetValue(itm.AccountId, out accountInfo);
                itm.Value = itm.CurrencyId != accountInfo.CurrencyId ? currencyExchangeRateManager.ConvertValueToCurrency(itm.Value, itm.CurrencyId, accountInfo.CurrencyId, itm.EffectiveOn) : itm.Value;
            }
            var groupedResult = usageBalanceUpdates.GroupBy(elt => elt.AccountId).Select(group => new UsageBalanceUpdate { AccountId = group.Key, Value = group.Sum(elt => elt.Value) });
            return dataManager.UpdateLiveBalanceFromBalanceUsageQueue(groupedResult, balanceUsageQueueId);
        }
        private decimal ConvertValueToCurrency(decimal amount, int fromCurrencyId, int currencyId, DateTime effectiveOn)
        {
            return currencyExchangeRateManager.ConvertValueToCurrency(amount, fromCurrencyId, currencyId, effectiveOn);
        }
        private bool UpdateLiveBalanceFromBillingTransaction(List<BillingTransaction> billingTransactions, long accountId, LiveBalanceAccountInfo accountInfo)
        {
            decimal amount = 0;
            List<long> billingTransactionIds = new List<long>();
            foreach (var billingTransaction in billingTransactions)
            {
                billingTransactionIds.Add(billingTransaction.AccountBillingTransactionId);
                accountId = billingTransaction.AccountId;
                amount += billingTransaction.CurrencyId != accountInfo.CurrencyId ? ConvertValueToCurrency(billingTransaction.Amount, billingTransaction.CurrencyId, accountInfo.CurrencyId, billingTransaction.TransactionTime) : billingTransaction.Amount;
            }
            return dataManager.UpdateLiveBalanceFromBillingTransaction(accountId, billingTransactionIds, amount);
        }

        #endregion

    }
}
