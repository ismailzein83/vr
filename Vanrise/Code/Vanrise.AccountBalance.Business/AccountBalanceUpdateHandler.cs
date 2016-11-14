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
        #region Fields

        static ConcurrentDictionary<Guid, AccountBalanceUpdateHandler> _handlersByAccountTypeId = new ConcurrentDictionary<Guid, AccountBalanceUpdateHandler>();

        Dictionary<long, LiveBalanceAccountInfo> AccountsInfo;
        CurrencyExchangeRateManager currencyExchangeRateManager;
        AccountManager manager;
        ILiveBalanceDataManager dataManager;
        Guid _accountTypeId;

        #endregion

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

        #region Public Methods

        /// <summary>
        /// A factory like method that will return a new instance if not exists for passed AccountTypeId
        /// </summary>
        /// <param name="accountTypeId">Account Type Id</param>
        /// <returns>New instance if AccountTypeId is not exist in dictionary, else return instance of passed AccountTypeId</returns>
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

        public void AddAndUpdateLiveBalanceFromBillingTransction(List<BillingTransaction> billingTransactions)
        {
            var accountsToInsert = billingTransactions.Where(x => !AccountsInfo.ContainsKey(x.AccountId)).Select(a => a.AccountId).Distinct();
            InsertAccountInfos(accountsToInsert);

            UpdateLiveBalanceFromBillingTransaction(billingTransactions);

        }
        public void AddAndUpdateLiveBalanceFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<UsageBalanceUpdate> usageBalanceUpdates)
        {
            var accountsToInsert = usageBalanceUpdates.Where(x => !AccountsInfo.ContainsKey(x.AccountId)).Select(a => a.AccountId).Distinct();
            InsertAccountInfos(accountsToInsert);
            UpdateLiveBalanceFromBalanceUsageQueue(balanceUsageQueueId, usageBalanceUpdates);
        }

        private void InsertAccountInfos(IEnumerable<long> accountsToInsert)
        {
            foreach (var accountToInsert in accountsToInsert)
                AddLiveAccountInfo(accountToInsert);
        }
        public bool TryAddLiveBalance(long accountId, int currencyId)
        {
            return dataManager.TryAddLiveBalance(accountId, _accountTypeId, 0, currencyId, 0, 0);
        }
        #endregion

        #region Private Methods
        private LiveBalanceAccountInfo AddLiveAccountInfo(long accountId)
        {
            var account = manager.GetAccountInfo(accountId);
            var accountInfo = new LiveBalanceAccountInfo { AccountId = accountId, CurrencyId = account.CurrencyId };

            if (TryAddLiveBalance(accountId, accountInfo.CurrencyId))
            {
                AccountsInfo.Add(accountId, accountInfo);
            }
            return accountInfo;
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
            return dataManager.UpdateLiveBalanceFromBalanceUsageQueue(_accountTypeId, groupedResult, balanceUsageQueueId);
        }
        private decimal ConvertValueToCurrency(decimal amount, int fromCurrencyId, int currencyId, DateTime effectiveOn)
        {
            return currencyExchangeRateManager.ConvertValueToCurrency(amount, fromCurrencyId, currencyId, effectiveOn);
        }
        private void UpdateLiveBalanceFromBillingTransaction(List<BillingTransaction> billingTransactions)
        {
            decimal amount = 0;
            var groupedResult = billingTransactions.GroupBy(elt => elt.AccountId);
            List<long> billingTransactionIds = new List<long>();
            foreach (var group in groupedResult)
            {
                LiveBalanceAccountInfo accountInfo = null;
                AccountsInfo.TryGetValue(group.Key, out accountInfo);
                foreach (var billingTransaction in group)
                {
                    billingTransactionIds.Add(billingTransaction.AccountBillingTransactionId);
                    amount += billingTransaction.CurrencyId != accountInfo.CurrencyId ? ConvertValueToCurrency(billingTransaction.Amount, billingTransaction.CurrencyId, accountInfo.CurrencyId, billingTransaction.TransactionTime) : billingTransaction.Amount;
                }
                dataManager.UpdateLiveBalanceFromBillingTransaction(_accountTypeId, accountInfo.AccountId, billingTransactionIds, amount);
            }

        }

        #endregion

    }
}
