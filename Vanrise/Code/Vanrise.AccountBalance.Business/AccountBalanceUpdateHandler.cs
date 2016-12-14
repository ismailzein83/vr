using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;
namespace Vanrise.AccountBalance.Business
{
    public class AccountBalanceUpdateHandler
    {
        #region Fields

        static ConcurrentDictionary<Guid, AccountBalanceUpdateHandler> _handlersByAccountTypeId = new ConcurrentDictionary<Guid, AccountBalanceUpdateHandler>();

        Dictionary<long, LiveBalanceAccountInfo> AccountsInfo;
        Dictionary<DateTime, Dictionary<long,AccountUsageInfo>> AccountsUsageByPeriod;
        CurrencyExchangeRateManager currencyExchangeRateManager;
        AccountManager manager;
        ILiveBalanceDataManager liveBalanceDataManager;
        IAccountUsageDataManager accountUsageDataManager;
        Guid _accountTypeId;
        AccountUsagePeriodSettings _accountUsagePeriodSettings;
        BillingTransactionTypeManager billingTransactionTypeManager;

        #endregion

        #region ctor
        private AccountBalanceUpdateHandler(Guid accountTypeId)
        {
            currencyExchangeRateManager = new CurrencyExchangeRateManager();
            liveBalanceDataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            accountUsageDataManager = AccountBalanceDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
            manager = new AccountManager();
            _accountTypeId = accountTypeId;
            billingTransactionTypeManager = new BillingTransactionTypeManager();
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
            
            Dictionary<long, LiveBalanceToUpdate> liveBalnacesToUpdate = new Dictionary<long, LiveBalanceToUpdate>();
            List<long> billingTransactionIds = new List<long>();
            foreach (var billingTransaction in billingTransactions)
            {
                var liveBalanceInfo = GetLiveBalanceInfo(billingTransaction.AccountId);
                var transactionType = billingTransactionTypeManager.GetBillingTransactionType(billingTransaction.TransactionTypeId);
                decimal value = billingTransaction.Amount;
                if (!transactionType.IsCredit)
                    value = -value;
                GroupLiveBalanceToUpdateById(liveBalnacesToUpdate, billingTransaction.TransactionTime, billingTransaction.CurrencyId, value, liveBalanceInfo);
                billingTransactionIds.Add(billingTransaction.AccountBillingTransactionId);
            }
            if (liveBalnacesToUpdate.Count > 0)
            {
                UpdateLiveBalanceFromBillingTransaction(liveBalnacesToUpdate.Values, billingTransactionIds);
            }
        }
        public void AddAndUpdateLiveBalanceFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<UsageBalanceUpdate> usageBalanceUpdates)
        {

            Dictionary<long, LiveBalanceToUpdate> liveBalnacesToUpdate = new Dictionary<long, LiveBalanceToUpdate>();
            Dictionary<long, AccountUsageToUpdate> accountsUsageToUpdate = new Dictionary<long, AccountUsageToUpdate>();

            foreach (var usageBalanceUpdate in usageBalanceUpdates)
            {
                var liveBalanceInfo = GetLiveBalanceInfo(usageBalanceUpdate.AccountId);
                GroupLiveBalanceToUpdateById(liveBalnacesToUpdate, usageBalanceUpdate.EffectiveOn, usageBalanceUpdate.CurrencyId, -usageBalanceUpdate.Value, liveBalanceInfo);
                
                AccountUsagePeriodEvaluationContext context = new AccountUsagePeriodEvaluationContext
                {
                    UsageTime = usageBalanceUpdate.EffectiveOn
                };
                _accountUsagePeriodSettings.EvaluatePeriod(context);
                var accountUsageInfo = GetAccountUsageInfo(usageBalanceUpdate.AccountId, context.PeriodStart, context.PeriodEnd);
                GroupAccountUsageToUpdateById(accountsUsageToUpdate, usageBalanceUpdate.EffectiveOn, usageBalanceUpdate.CurrencyId,usageBalanceUpdate.Value, accountUsageInfo);
            }
            if (liveBalnacesToUpdate.Count > 0 || accountsUsageToUpdate.Count>0)
            {
                UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(balanceUsageQueueId, liveBalnacesToUpdate.Values, accountsUsageToUpdate.Values);
            }
        }

        #endregion

        #region Private Methods
        private void IntializeAccountsInfo()
        {
            AccountsInfo = new Dictionary<long, LiveBalanceAccountInfo>();
            var accountBlances = liveBalanceDataManager.GetLiveBalanceAccountsInfo(_accountTypeId);
            AccountsInfo = accountBlances.ToDictionary(x => x.AccountId, x => x);
            AccountsUsageByPeriod = new Dictionary<DateTime, Dictionary<long, AccountUsageInfo>>();
            _accountUsagePeriodSettings = new AccountTypeManager().GetAccountUsagePeriodSettings(_accountTypeId);
        }
        private LiveBalanceAccountInfo GetLiveBalanceInfo(long accountId)
        {
            return AccountsInfo.GetOrCreateItem(accountId, () =>
            {
                return AddLiveAccountInfo(accountId);
            });
        }
        private LiveBalanceAccountInfo AddLiveAccountInfo(long accountId)
        {
            var account = manager.GetAccountInfo(_accountTypeId, accountId);
            return TryAddLiveBalanceAndGet(accountId, account.CurrencyId);
        }
        private LiveBalanceAccountInfo TryAddLiveBalanceAndGet(long accountId, int currencyId)
        {
            return liveBalanceDataManager.TryAddLiveBalanceAndGet(accountId, _accountTypeId, 0, currencyId, 0, 0);

        }
        private AccountUsageInfo GetAccountUsageInfo(long accountId, DateTime periodStart, DateTime periodEnd)
        {
            var accountsUsageByPeriod = AccountsUsageByPeriod.GetOrCreateItem(periodStart, () =>
            {
                IEnumerable<AccountUsageInfo> accountsUsageInfo = accountUsageDataManager.GetAccountsUsageInfoByPeriod(_accountTypeId, periodStart);
                return accountsUsageInfo.ToDictionary(x=>x.AccountId,x=>x);
            });

          return accountsUsageByPeriod.GetOrCreateItem(accountId,()=>
            {
                return AddAccountUsageInfo(accountId, periodStart, periodEnd);
            });
        }
        private AccountUsageInfo AddAccountUsageInfo(long accountId, DateTime periodStart, DateTime periodEnd)
        {
            return TryAddAccountUsageAndGet(accountId, periodStart, periodEnd);
        }
        private AccountUsageInfo TryAddAccountUsageAndGet(long accountId, DateTime periodStart,DateTime periodEnd)
        {
            return accountUsageDataManager.TryAddAccountUsageAndGet(_accountTypeId, accountId, periodStart, periodEnd, 0);
        }
        private void GroupLiveBalanceToUpdateById(Dictionary<long, LiveBalanceToUpdate> liveBalnacesToUpdate, DateTime effectiveOn, int currencyId, decimal value, LiveBalanceAccountInfo liveBalanceInfo)
        {
            LiveBalanceToUpdate liveBalanceToUpdate = liveBalnacesToUpdate.GetOrCreateItem(liveBalanceInfo.LiveBalanceId, () =>  new LiveBalanceToUpdate
                {
                    LiveBalanceId = liveBalanceInfo.LiveBalanceId
                });
            liveBalanceToUpdate.Value += currencyId != liveBalanceInfo.CurrencyId ? currencyExchangeRateManager.ConvertValueToCurrency(value, currencyId, liveBalanceInfo.CurrencyId, effectiveOn) : value;
        }
        private void GroupAccountUsageToUpdateById(Dictionary<long, AccountUsageToUpdate> accountsUsageToUpdate, DateTime effectiveOn,  int currencyId,decimal value, AccountUsageInfo accountUsageInfo)
        {

            AccountUsageToUpdate accountUsageToUpdate;
            if (!accountsUsageToUpdate.TryGetValue(accountUsageInfo.AccountUsageId, out accountUsageToUpdate))
            {
                accountsUsageToUpdate.Add(accountUsageInfo.AccountUsageId, new AccountUsageToUpdate
                {
                    AccountUsageId = accountUsageInfo.AccountUsageId,
                    Value = value
                });
            }
            else
            {
                accountUsageToUpdate.Value += value;
                accountsUsageToUpdate[accountUsageInfo.AccountUsageId] = accountUsageToUpdate;
            }

        }
        private decimal ConvertValueToCurrency(decimal amount, int fromCurrencyId, int currencyId, DateTime effectiveOn)
        {
            return currencyExchangeRateManager.ConvertValueToCurrency(amount, fromCurrencyId, currencyId, effectiveOn);
        }
        private bool UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate, IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate)
        {
            return liveBalanceDataManager.UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(balanceUsageQueueId, liveBalnacesToUpdate, accountsUsageToUpdate);
        }
        private bool UpdateLiveBalanceFromBillingTransaction(IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate, List<long> billingTransactionIds)
        {
            return liveBalanceDataManager.UpdateLiveBalanceFromBillingTransaction(liveBalnacesToUpdate, billingTransactionIds);
        }

        #endregion
       
    }
}
