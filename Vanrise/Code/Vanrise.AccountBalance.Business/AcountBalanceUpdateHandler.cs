using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;

namespace Vanrise.AccountBalance.Business
{
    public class AcountBalanceUpdateHandler
    {
        Dictionary<long, LiveBalanceAccountInfo> AccountsInfo;
        CurrencyExchangeRateManager currencyExchangeRateManager;
        AccountManager manager;
        ILiveBalanceDataManager dataManager;
        bool LoadingError;

        #region ctor
        public AcountBalanceUpdateHandler()
        {
            currencyExchangeRateManager = new CurrencyExchangeRateManager();
            dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            manager = new AccountManager();
            IntializeAccountsInfo();
        }
        #endregion

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
            UpdateLiveBalanceFromBillingTransaction(billingTransactions, accountId,accountInfo);
            
        }
        public void AddAndUpdateLiveBalanceFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<UsageBalanceUpdate> usageBalanceUpdates)
        {
            if(this.LoadingError)
            {
                IntializeAccountsInfo();
            }
            var accountsToInsert = usageBalanceUpdates.Where(x => !AccountsInfo.ContainsKey(x.AccountId)).Distinct();
            foreach(var accountToInsert in accountsToInsert)
            {
                AddLiveAccountInfo(accountToInsert.AccountId);
            }
            UpdateLiveBalanceFromBalanceUsageQueue(balanceUsageQueueId, usageBalanceUpdates);
        }
        public bool AddLiveBalance(long accountId, int currencyId)
        {
            LiveBalance liveBalance = new LiveBalance
            {
                AccountId = accountId,
                CurrencyId = currencyId,
                CurrentBalance = 0,
                UsageBalance = 0,
            };
            return dataManager.Insert(liveBalance);
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
            var accountBlances = dataManager.GetLiveBalanceAccountsInfo();
            AccountsInfo = accountBlances.ToDictionary(x => x.AccountId, x => x);
        }
        private bool UpdateLiveBalanceFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<UsageBalanceUpdate> usageBalanceUpdates)
        {
            foreach (var itm in usageBalanceUpdates)
            {
                LiveBalanceAccountInfo accountInfo = null;
                AccountsInfo.TryGetValue(itm.AccountId, out accountInfo);
                itm.Value = currencyExchangeRateManager.ConvertValueToCurrency(itm.Value, itm.CurrencyId, accountInfo.CurrencyId, itm.EffectiveOn);
            }
            var groupedResult = usageBalanceUpdates.GroupBy(elt => elt.AccountId).Select(group => new UsageBalanceUpdate { AccountId = group.Key, Value = group.Sum(elt => elt.Value)});
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
                amount += ConvertValueToCurrency(billingTransaction.Amount, billingTransaction.CurrencyId, accountInfo.CurrencyId, billingTransaction.TransactionTime);
            }
            return dataManager.UpdateLiveBalanceFromBillingTransaction(accountId, billingTransactionIds, amount);
        }

        #endregion

    }
}
