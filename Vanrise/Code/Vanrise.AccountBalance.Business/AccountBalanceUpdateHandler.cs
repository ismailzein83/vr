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

        Dictionary<String, LiveBalanceAccountInfo> AccountsInfo;
        static Dictionary<DateTime, Dictionary<AccountTransaction, AccountUsageInfo>> AccountsUsageByPeriod;
        CurrencyExchangeRateManager currencyExchangeRateManager;
        AccountManager manager;
        ILiveBalanceDataManager liveBalanceDataManager;
        IAccountUsageDataManager accountUsageDataManager;
        Guid _accountTypeId;
        AccountUsagePeriodSettings _accountUsagePeriodSettings;
        BillingTransactionTypeManager billingTransactionTypeManager;
        AccountUsageManager accountUsageManager;
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
            accountUsageManager = new AccountUsageManager();


            IntializeAccountsInfo();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// A factory like method that will return a new instance if not exists for passed AccountTypeId
        /// </summary>
        /// <param name="accountTypeId">Account Type Id</param>
        /// <returns>New instance if AccountTypeId is not exist in dictionary, else return instance of passed AccountTypeId</returns>
        public static AccountBalanceUpdateHandler GetHandlerByAccountTypeId(Guid accountTypeId, int usageCacheDays)
        {
            AccountBalanceUpdateHandler handler;
            var cachePerdiodDate = DateTime.Today.AddDays(-usageCacheDays);
            DeleteItemsFromDictionaryBeforePeriod(cachePerdiodDate);
            if (!_handlersByAccountTypeId.TryGetValue(accountTypeId, out handler))
            {
                handler = new AccountBalanceUpdateHandler(accountTypeId);
                _handlersByAccountTypeId.TryAdd(accountTypeId, handler);
            }
            return handler;
        }
        private static void DeleteItemsFromDictionaryBeforePeriod(DateTime period)
        { 
            if (AccountsUsageByPeriod != null)
            {
                var listOfKeys = AccountsUsageByPeriod.Keys.ToList();
                foreach (var key in listOfKeys)
                {
                    if (key < period)
                    {
                        AccountsUsageByPeriod.Remove(key);
                    }
                }
            }
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
        public void AddAndUpdateLiveBalanceFromBalanceUsageQueue(long balanceUsageQueueId, Guid transactionTypeId, IEnumerable<UpdateUsageBalanceItem> usageBalanceUpdates)
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
                var accountUsageInfo = GetAccountUsageInfo(transactionTypeId,usageBalanceUpdate.AccountId, context.PeriodStart, context.PeriodEnd, liveBalanceInfo.CurrencyId);
                GroupAccountUsageToUpdateById(accountsUsageToUpdate, usageBalanceUpdate.EffectiveOn, usageBalanceUpdate.CurrencyId, liveBalanceInfo.CurrencyId, usageBalanceUpdate.Value, accountUsageInfo);
            }
            if (liveBalnacesToUpdate.Count > 0 || accountsUsageToUpdate.Count>0)
            {
                UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(balanceUsageQueueId, liveBalnacesToUpdate.Values, accountsUsageToUpdate.Values,null);
            }
        }
        public void CorrectBalanceFromBalanceUsageQueue(long balanceUsageQueueId, Guid transactionTypeId, IEnumerable<CorrectUsageBalanceItem> correctUsageBalanceItems, DateTime periodDate, Guid correctionProcessId, bool isLastBatch)
        {
            var transactionType = new BillingTransactionTypeManager().GetBillingTransactionType(transactionTypeId);
          
            if (correctUsageBalanceItems != null)
            {
                List<AccountUsageToUpdate> accountUsageToUpdates = new List<AccountUsageToUpdate>();
                List<LiveBalanceToUpdate> liveBalanceToUpdates = new List<LiveBalanceToUpdate>();
                var accountIds = correctUsageBalanceItems.Select(x => x.AccountId).ToList();
                var accountUsages = GetAccountUsageForSpecificPeriodByAccountIds(_accountTypeId, transactionTypeId, periodDate, accountIds);
                foreach (var correctUsageBalanceItem in correctUsageBalanceItems)
                {
                    var accountUsage = accountUsages.FirstOrDefault(x => x.AccountId == correctUsageBalanceItem.AccountId);
                    CorrectLiveBalanceAndAccountUsage(liveBalanceToUpdates, accountUsageToUpdates, correctUsageBalanceItem.AccountId, transactionType, correctUsageBalanceItem.Value, correctUsageBalanceItem.CurrencyId, accountUsage, periodDate);
                }
                if (liveBalanceToUpdates.Count > 0 || accountUsageToUpdates.Count > 0)
                {
                    UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(balanceUsageQueueId, liveBalanceToUpdates, accountUsageToUpdates, correctionProcessId);
                }
            }
            if (isLastBatch)
            {
                 var accountsUsageError = GetAccountUsageErrorData(transactionTypeId, correctionProcessId, periodDate);
                 if (accountsUsageError != null)
                 {
                     List<AccountUsageToUpdate> accountUsageToUpdates = new List<AccountUsageToUpdate>();
                     List<LiveBalanceToUpdate> liveBalanceToUpdates = new List<LiveBalanceToUpdate>();
                     foreach (var item in accountsUsageError)
                     {
                         CorrectLiveBalanceAndAccountUsage(liveBalanceToUpdates, accountUsageToUpdates, item.AccountId, transactionType, 0, item.CurrencyId, item, periodDate);
                     }
                     if (liveBalanceToUpdates.Count > 0 || accountUsageToUpdates.Count > 0)
                     {
                         UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(balanceUsageQueueId, liveBalanceToUpdates, accountUsageToUpdates, correctionProcessId);
                     }
                 }
                
            }
        }
        #endregion

        #region Private Methods

        private void IntializeAccountsInfo()
        {
            AccountsInfo = new Dictionary<String, LiveBalanceAccountInfo>();
            var accountBlances = liveBalanceDataManager.GetLiveBalanceAccountsInfo(_accountTypeId);
            AccountsInfo = accountBlances.ToDictionary(x => x.AccountId, x => x);
            AccountsUsageByPeriod = new Dictionary<DateTime, Dictionary<AccountTransaction, AccountUsageInfo>>();
            _accountUsagePeriodSettings = new AccountTypeManager().GetAccountUsagePeriodSettings(_accountTypeId);
        }

        #region Live Balance
        private LiveBalanceAccountInfo GetLiveBalanceInfo(String accountId)
        {
            return AccountsInfo.GetOrCreateItem(accountId, () =>
            {
                return AddLiveAccountInfo(accountId);
            });
        }
        private LiveBalanceAccountInfo AddLiveAccountInfo(String accountId)
        {
            var account = manager.GetAccountInfo(_accountTypeId, accountId);
            return TryAddLiveBalanceAndGet(accountId, account.CurrencyId);
        }
        private LiveBalanceAccountInfo TryAddLiveBalanceAndGet(String accountId, int currencyId)
        {
            return liveBalanceDataManager.TryAddLiveBalanceAndGet(accountId, _accountTypeId, 0, currencyId, 0);

        }
        private void GroupLiveBalanceToUpdateById(Dictionary<long, LiveBalanceToUpdate> liveBalnacesToUpdate, DateTime effectiveOn, int currencyId, decimal value, LiveBalanceAccountInfo liveBalanceInfo)
        {
            LiveBalanceToUpdate liveBalanceToUpdate = liveBalnacesToUpdate.GetOrCreateItem(liveBalanceInfo.LiveBalanceId, () => new LiveBalanceToUpdate
            {
                LiveBalanceId = liveBalanceInfo.LiveBalanceId
            });
            liveBalanceToUpdate.Value += currencyId != liveBalanceInfo.CurrencyId ? currencyExchangeRateManager.ConvertValueToCurrency(value, currencyId, liveBalanceInfo.CurrencyId, effectiveOn) : value;
        }
        private bool UpdateLiveBalanceFromBillingTransaction(IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate, List<long> billingTransactionIds)
        {
            return liveBalanceDataManager.UpdateLiveBalanceFromBillingTransaction(liveBalnacesToUpdate, billingTransactionIds);
        }
        #endregion

        #region AccountUsage
        private void AddAccountUsageToUpdate(List<AccountUsageToUpdate> accountUsageToUpdates, long accountUsageId, decimal value)
        {
            if (accountUsageToUpdates == null)
                accountUsageToUpdates = new List<AccountUsageToUpdate>();
            accountUsageToUpdates.Add(new AccountUsageToUpdate
            {
                AccountUsageId = accountUsageId,
                Value = value
            });
        }
        private AccountUsageInfo GetAccountUsageInfo(Guid transactionTypeId, String accountId, DateTime periodStart, DateTime periodEnd, int currencyId)
        {
            var accountsUsageByPeriod = AccountsUsageByPeriod.GetOrCreateItem(periodStart, () =>
            {
                IEnumerable<AccountUsageInfo> accountsUsageInfo = accountUsageDataManager.GetAccountsUsageInfoByPeriod(_accountTypeId, periodStart, transactionTypeId);
                return accountsUsageInfo.ToDictionary(x => new AccountTransaction { AccountId = x.AccountId ,TransactionTypeId = x.TransactionTypeId }, x => x);
            });

            return accountsUsageByPeriod.GetOrCreateItem(new AccountTransaction { AccountId =accountId ,TransactionTypeId = transactionTypeId}, () =>
            {
                return AddAccountUsageInfo(transactionTypeId, accountId, periodStart, periodEnd, currencyId);
            });
        }
        private AccountUsageInfo AddAccountUsageInfo(Guid transactionTypeId, String accountId, DateTime periodStart, DateTime periodEnd, int currencyId)
        {
            return TryAddAccountUsageAndGet(transactionTypeId, accountId, periodStart, periodEnd, currencyId);
        }
        private AccountUsageInfo TryAddAccountUsageAndGet(Guid transactionTypeId, String accountId, DateTime periodStart, DateTime periodEnd, int currencyId)
        {
            string billingTransactionNote = string.Format("Usage From {0:yyyy-MM-dd HH:mm} to {1:yyyy-MM-dd HH:mm}", periodStart, periodEnd);
            return accountUsageDataManager.TryAddAccountUsageAndGet(_accountTypeId, transactionTypeId, accountId, periodStart, periodEnd, currencyId, 0, billingTransactionNote);
        }
        private void GroupAccountUsageToUpdateById(Dictionary<long, AccountUsageToUpdate> accountsUsageToUpdate, DateTime effectiveOn, int usageCurrencyId, int liveBalanceCurrencyId, decimal value, AccountUsageInfo accountUsageInfo)
        {

            AccountUsageToUpdate accountUsageToUpdate = accountsUsageToUpdate.GetOrCreateItem(accountUsageInfo.AccountUsageId, () => new AccountUsageToUpdate
            {
                AccountUsageId = accountUsageInfo.AccountUsageId
            });
            accountUsageToUpdate.Value += usageCurrencyId != liveBalanceCurrencyId ? currencyExchangeRateManager.ConvertValueToCurrency(value, usageCurrencyId, liveBalanceCurrencyId, effectiveOn) : value;
        }
        private IEnumerable<AccountUsage> GetAccountUsageForSpecificPeriodByAccountIds(Guid accountTypeId, Guid transactionTypeId, DateTime datePeriod, List<String> accountIds)
        {
            return accountUsageManager.GetAccountUsageForSpecificPeriodByAccountIds(accountTypeId, transactionTypeId, datePeriod, accountIds);
        }
        private List<AccountUsage> GetAccountUsageErrorData(Guid transactionTypeId, Guid correctionProcessId, DateTime periodDate)
        {
            return accountUsageManager.GetAccountUsageErrorData(_accountTypeId, transactionTypeId, correctionProcessId, periodDate);
        }
        #endregion
        private decimal ConvertValueToCurrency(decimal amount, int fromCurrencyId, int currencyId, DateTime effectiveOn)
        {
            return currencyExchangeRateManager.ConvertValueToCurrency(amount, fromCurrencyId, currencyId, effectiveOn);
        }
        private bool UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate, IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate, Guid? correctionProcessId)
        {
            return liveBalanceDataManager.UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(balanceUsageQueueId, liveBalnacesToUpdate, accountsUsageToUpdate, correctionProcessId);
        }
        private void CorrectLiveBalanceAndAccountUsage(List<LiveBalanceToUpdate> liveBalanceToUpdates, List<AccountUsageToUpdate> accountUsageToUpdates, String accountId, BillingTransactionType transactionType, decimal value, int currencyId, AccountUsage accountUsage, DateTime periodDate)
        {
            var accountInfo = GetLiveBalanceInfo(accountId);
            var amount = ConvertValueToCurrency(value, currencyId, accountInfo.CurrencyId, periodDate);
            decimal differenceAmount = amount;
            if (accountUsage != null)
            {
                differenceAmount = amount - accountUsage.UsageBalance;
                AddAccountUsageToUpdate(accountUsageToUpdates, accountUsage.AccountUsageId, differenceAmount);
            }
            else
            {
                AccountUsagePeriodEvaluationContext context = new AccountUsagePeriodEvaluationContext
                {
                    UsageTime = periodDate
                };
                _accountUsagePeriodSettings.EvaluatePeriod(context);
                var accountUsageInfo = AddAccountUsageInfo(transactionType.BillingTransactionTypeId, accountId, context.PeriodStart, context.PeriodEnd, accountInfo.CurrencyId);
                AddAccountUsageToUpdate(accountUsageToUpdates, accountUsageInfo.AccountUsageId, differenceAmount);
            }
            liveBalanceToUpdates.Add(new LiveBalanceToUpdate
            {
                LiveBalanceId = accountInfo.LiveBalanceId,
                Value = transactionType != null && transactionType.IsCredit ? differenceAmount : -differenceAmount
            });
        }
    
        #endregion
       
    }
    public struct AccountTransaction
    {
        public String AccountId { get; set; }
        public Guid TransactionTypeId { get; set; }
    }
}
