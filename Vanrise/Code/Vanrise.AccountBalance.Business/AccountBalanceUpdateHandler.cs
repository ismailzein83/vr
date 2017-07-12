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
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountBalanceUpdateHandler
    {
        #region Fields

        static ConcurrentDictionary<Guid, AccountBalanceUpdateHandler> _handlersByAccountTypeId = new ConcurrentDictionary<Guid, AccountBalanceUpdateHandler>();

        Dictionary<String, LiveBalanceAccountInfo> AccountsInfo;
        Dictionary<DateTime, Dictionary<AccountTransaction, AccountUsageInfo>> AccountsUsageByPeriod;
        CurrencyExchangeRateManager currencyExchangeRateManager;
        AccountManager manager;
        ILiveBalanceDataManager liveBalanceDataManager;
        IAccountUsageDataManager accountUsageDataManager;
        Guid _accountTypeId;
        AccountUsagePeriodSettings _accountUsagePeriodSettings;
        BillingTransactionTypeManager billingTransactionTypeManager;
        AccountUsageManager accountUsageManager;

        #endregion

        #region Constructors

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
            if (!_handlersByAccountTypeId.TryGetValue(accountTypeId, out handler))
            {
                handler = new AccountBalanceUpdateHandler(accountTypeId);
                _handlersByAccountTypeId.TryAdd(accountTypeId, handler);
            }
            else
            {
                if (handler.AccountsUsageByPeriod.Count > 0)
                    handler.AccountsUsageByPeriod.Clear(); //to keep Usage IsOverridden synchronized with database
            }
            return handler;
        }
       
        public void AddAndUpdateLiveBalanceFromBillingTransction(List<BillingTransaction> billingTransactions)
        {
            var balancesToUpdateByAccountId = new Dictionary<string, LiveBalanceToUpdate>();
            var transactionIds = new List<long>();

            var usageOverridesToAdd = new List<AccountUsageOverride>();
            IEnumerable<long> usageIdsToOverride = new List<long>();

            var deletedTransactionIds = new List<long>();
            IEnumerable<long> overridenUsageIdsToRollback = new List<long>();

            foreach (BillingTransaction transaction in billingTransactions)
            {
                LiveBalanceAccountInfo accountBalanceInfo = GetLiveBalanceInfo(transaction.AccountId);

                LiveBalanceToUpdate balanceToUpdate = balancesToUpdateByAccountId.GetOrCreateItem(accountBalanceInfo.AccountId, () => new LiveBalanceToUpdate()
                {
                    LiveBalanceId = accountBalanceInfo.LiveBalanceId
                });

                BillingTransactionType transactionType = billingTransactionTypeManager.GetBillingTransactionType(transaction.TransactionTypeId);
                transactionType.ThrowIfNull("transactionType", transaction.TransactionTypeId);

                decimal convertedTransactionAmount = currencyExchangeRateManager.ConvertValueToCurrency(transaction.Amount, transaction.CurrencyId, accountBalanceInfo.CurrencyId, transaction.TransactionTime);

                if (!transactionType.IsCredit)
                    convertedTransactionAmount = -convertedTransactionAmount;

                if (transaction.IsDeleted)
                {
                    deletedTransactionIds.Add(transaction.AccountBillingTransactionId);
                    balanceToUpdate.Value -= convertedTransactionAmount;
                }
                else
                {
                    if (transaction.Settings != null && transaction.Settings.UsageOverrides != null && transaction.Settings.UsageOverrides.Count > 0)
                        AddUsageOverrides(usageOverridesToAdd, transaction);

                    balanceToUpdate.Value += convertedTransactionAmount;
                    transactionIds.Add(transaction.AccountBillingTransactionId);
                }
            }

            if (usageOverridesToAdd.Count > 0)
                ProcessUsageOverridingTransactions(usageOverridesToAdd, balancesToUpdateByAccountId, out usageIdsToOverride);

            if (deletedTransactionIds.Count > 0)
                ProcessDeletedTransactions(deletedTransactionIds, balancesToUpdateByAccountId, out overridenUsageIdsToRollback);

            if (balancesToUpdateByAccountId.Count > 0)
                UpdateLiveBalancesFromBillingTransactions(balancesToUpdateByAccountId.Values, transactionIds, usageIdsToOverride, usageOverridesToAdd, overridenUsageIdsToRollback, deletedTransactionIds);

            if (AccountsUsageByPeriod.Count > 0)
                AccountsUsageByPeriod.Clear();//to keep Usage IsOverridden synchronized with database
        }
        public void AddAndUpdateLiveBalanceFromBalanceUsageQueue(long balanceUsageQueueId, Guid transactionTypeId, IEnumerable<UpdateUsageBalanceItem> usageBalanceUpdates)
        {
            var balancesToUpdate = new Dictionary<long, LiveBalanceToUpdate>();
            var accountUsagesToUpdate = new Dictionary<long, AccountUsageToUpdate>();

            BillingTransactionType transactionType = billingTransactionTypeManager.GetBillingTransactionType(transactionTypeId);

            foreach (UpdateUsageBalanceItem usageBalanceUpdate in usageBalanceUpdates)
            {
                LiveBalanceAccountInfo accountLiveBalanceInfo = GetLiveBalanceInfo(usageBalanceUpdate.AccountId);

                var context = new AccountUsagePeriodEvaluationContext { UsageTime = usageBalanceUpdate.EffectiveOn };
                _accountUsagePeriodSettings.EvaluatePeriod(context);
                AccountUsageInfo accountUsageInfo = GetAccountUsageInfo(transactionTypeId, usageBalanceUpdate.AccountId, context.PeriodStart, context.PeriodEnd, accountLiveBalanceInfo.CurrencyId);

                GroupAccountUsageToUpdateById(accountUsagesToUpdate, usageBalanceUpdate.EffectiveOn, usageBalanceUpdate.CurrencyId, accountLiveBalanceInfo.CurrencyId, usageBalanceUpdate.Value, accountUsageInfo);

                if (!accountUsageInfo.IsOverridden)
                {
                    decimal liveBalanceValue = usageBalanceUpdate.Value;
                    if (!transactionType.IsCredit)
                        liveBalanceValue = -liveBalanceValue;
                    GroupLiveBalanceToUpdateById(balancesToUpdate, usageBalanceUpdate.EffectiveOn, usageBalanceUpdate.CurrencyId, liveBalanceValue, accountLiveBalanceInfo);
                }
            }

            if (balancesToUpdate.Count > 0 || accountUsagesToUpdate.Count > 0)
                UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(balanceUsageQueueId, balancesToUpdate.Values, accountUsagesToUpdate.Values, null);
        }
        public void CorrectBalanceFromBalanceUsageQueue(long balanceUsageQueueId, Guid transactionTypeId, IEnumerable<CorrectUsageBalanceItem> correctUsageBalanceItems, DateTime periodDate, Guid correctionProcessId, bool isLastBatch)
        {
            BillingTransactionType transactionType = new BillingTransactionTypeManager().GetBillingTransactionType(transactionTypeId);

            if (correctUsageBalanceItems != null)
            {
                var accountUsagesToUpdate = new List<AccountUsageToUpdate>();
                var liveBalancesToUpdate = new List<LiveBalanceToUpdate>();

                List<string> accountIds = correctUsageBalanceItems.MapRecords(x => x.AccountId).ToList();
                IEnumerable<AccountUsage> accountUsages = GetAccountUsageForSpecificPeriodByAccountIds(_accountTypeId, transactionTypeId, periodDate, accountIds);

                foreach (CorrectUsageBalanceItem correctUsageBalanceItem in correctUsageBalanceItems)
                {
                    AccountUsage accountUsage = accountUsages.FirstOrDefault(x => x.AccountId == correctUsageBalanceItem.AccountId);
                    CorrectLiveBalanceAndAccountUsage(liveBalancesToUpdate, accountUsagesToUpdate, correctUsageBalanceItem.AccountId, transactionType, correctUsageBalanceItem.Value, correctUsageBalanceItem.CurrencyId, accountUsage, periodDate);
                }

                if (liveBalancesToUpdate.Count > 0 || accountUsagesToUpdate.Count > 0)
                    UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(balanceUsageQueueId, liveBalancesToUpdate, accountUsagesToUpdate, correctionProcessId);
            }

            if (isLastBatch)
            {
                List<AccountUsage> faultyAccountUsages = GetAccountUsageErrorData(transactionTypeId, correctionProcessId, periodDate);

                if (faultyAccountUsages != null && faultyAccountUsages.Count > 0)
                {
                    var accountUsagesToUpdate = new List<AccountUsageToUpdate>();
                    var liveBalancesToUpdate = new List<LiveBalanceToUpdate>();

                    foreach (AccountUsage faultyAccountUsage in faultyAccountUsages)
                    {
                        CorrectLiveBalanceAndAccountUsage(liveBalancesToUpdate, accountUsagesToUpdate, faultyAccountUsage.AccountId, transactionType, 0, faultyAccountUsage.CurrencyId, faultyAccountUsage, periodDate);
                    }

                    UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(balanceUsageQueueId, liveBalancesToUpdate, accountUsagesToUpdate, correctionProcessId);
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
            return TryAddLiveBalanceAndGet(accountId, account.CurrencyId,account.BED,account.EED,account.Status,account.IsDeleted);
        }
        private LiveBalanceAccountInfo TryAddLiveBalanceAndGet(String accountId, int currencyId,DateTime? bed,DateTime? eed, VRAccountStatus status,bool isDeleted)
        {
            return liveBalanceDataManager.TryAddLiveBalanceAndGet(accountId, _accountTypeId, 0, currencyId, 0, bed, eed, status, isDeleted);

        }
        private void GroupLiveBalanceToUpdateById(Dictionary<long, LiveBalanceToUpdate> liveBalancesToUpdate, DateTime effectiveOn, int currencyId, decimal value, LiveBalanceAccountInfo liveBalanceInfo)
        {
            LiveBalanceToUpdate liveBalanceToUpdate = liveBalancesToUpdate.GetOrCreateItem(liveBalanceInfo.LiveBalanceId, () => new LiveBalanceToUpdate
            {
                LiveBalanceId = liveBalanceInfo.LiveBalanceId
            });
            liveBalanceToUpdate.Value += currencyExchangeRateManager.ConvertValueToCurrency(value, currencyId, liveBalanceInfo.CurrencyId, effectiveOn);
        }
        private bool UpdateLiveBalancesFromBillingTransactions(IEnumerable<LiveBalanceToUpdate> liveBalancesToUpdate, IEnumerable<long> billingTransactionIds, IEnumerable<long> accountUsageIdsToOverride, IEnumerable<AccountUsageOverride> usageOverridesToAdd, IEnumerable<long> overridenUsageIdsToRollback, IEnumerable<long> deletedTransactionIds)
        {
            return liveBalanceDataManager.UpdateLiveBalancesFromBillingTransactions(liveBalancesToUpdate, billingTransactionIds, accountUsageIdsToOverride, usageOverridesToAdd, overridenUsageIdsToRollback, deletedTransactionIds);
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
                return accountsUsageInfo.ToDictionary(x => new AccountTransaction { AccountId = x.AccountId, TransactionTypeId = x.TransactionTypeId }, x => x);
            });

            return accountsUsageByPeriod.GetOrCreateItem(new AccountTransaction { AccountId = accountId, TransactionTypeId = transactionTypeId }, () =>
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
            return accountUsageDataManager.TryAddAccountUsageAndGet(_accountTypeId, transactionTypeId, accountId, periodStart, periodEnd, currencyId, 0);
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
        private void CorrectLiveBalanceAndAccountUsage(List<LiveBalanceToUpdate> liveBalancesToUpdate, List<AccountUsageToUpdate> accountUsagesToUpdate, String accountId, BillingTransactionType transactionType, decimal correctValue, int currencyId, AccountUsage accountUsage, DateTime periodDate)
        {
            LiveBalanceAccountInfo accountLiveBalanceInfo = GetLiveBalanceInfo(accountId);

            decimal convertedCorrectValue = ConvertValueToCurrency(correctValue, currencyId, accountLiveBalanceInfo.CurrencyId, periodDate);
            decimal amountDifference = convertedCorrectValue;

            bool isAccountUsageOverriden = false;

            if (accountUsage != null)
            {
                amountDifference = convertedCorrectValue - accountUsage.UsageBalance;

                isAccountUsageOverriden = accountUsage.IsOverriden;
                AddAccountUsageToUpdate(accountUsagesToUpdate, accountUsage.AccountUsageId, amountDifference);
            }
            else
            {
                var context = new AccountUsagePeriodEvaluationContext { UsageTime = periodDate };
                _accountUsagePeriodSettings.EvaluatePeriod(context);

                AccountUsageInfo accountUsageInfo = AddAccountUsageInfo(transactionType.BillingTransactionTypeId, accountId, context.PeriodStart, context.PeriodEnd, accountLiveBalanceInfo.CurrencyId);

                isAccountUsageOverriden = accountUsageInfo.IsOverridden;
                AddAccountUsageToUpdate(accountUsagesToUpdate, accountUsageInfo.AccountUsageId, amountDifference);
            }

            if (!isAccountUsageOverriden)
            {
                liveBalancesToUpdate.Add(new LiveBalanceToUpdate
                {
                    LiveBalanceId = accountLiveBalanceInfo.LiveBalanceId,
                    Value = (transactionType != null && transactionType.IsCredit) ? amountDifference : -amountDifference
                });
            }
        }

        #endregion

        #region Process Billing Transactions

        private void AddUsageOverrides(List<AccountUsageOverride> usageOverridesToAdd, BillingTransaction transaction)
        {
            foreach (BillingTransactionUsageOverride usageOverride in transaction.Settings.UsageOverrides)
            {
                var accountUsageOverride = new AccountUsageOverride()
                {
                    AccountTypeId = transaction.AccountTypeId,
                    AccountId = transaction.AccountId,
                    TransactionTypeId = usageOverride.TransactionTypeId,
                    PeriodStart = usageOverride.FromDate,
                    PeriodEnd = usageOverride.ToDate,
                    OverriddenByTransactionId = transaction.AccountBillingTransactionId
                };
                usageOverridesToAdd.Add(accountUsageOverride);
            }
        }

        private void ProcessUsageOverridingTransactions(IEnumerable<AccountUsageOverride> usageOverridesToAdd, Dictionary<string, LiveBalanceToUpdate> balancesToUpdateById, out IEnumerable<long> usageIdsToOverride)
        {
            IEnumerable<TransactionAccountUsageQuery> usageQueries = usageOverridesToAdd.MapRecords(x => new TransactionAccountUsageQuery()
            {
                TransactionId = x.OverriddenByTransactionId,
                TransactionTypeId = x.TransactionTypeId,
                AccountId = x.AccountId,
                AccountTypeId = x.AccountTypeId,
                PeriodStart = x.PeriodStart,
                PeriodEnd = x.PeriodEnd
            });

            IEnumerable<AccountUsage> allUsagesToOverride = accountUsageManager.GetAccountUsagesByTransactionAccountUsageQueries(usageQueries);
            var usageIdsToOverrideValue = new List<long>();

            foreach (AccountUsage usageToOverride in allUsagesToOverride)
            {
                LiveBalanceAccountInfo accountBalanceInfo = AccountsInfo.GetRecord(usageToOverride.AccountId);
                LiveBalanceToUpdate balanceToUpdate = balancesToUpdateById.GetRecord(usageToOverride.AccountId);

                BillingTransactionType transactionType = billingTransactionTypeManager.GetBillingTransactionType(usageToOverride.TransactionTypeId);
                decimal convertedUsageBalance = currencyExchangeRateManager.ConvertValueToCurrency(usageToOverride.UsageBalance, usageToOverride.CurrencyId, accountBalanceInfo.CurrencyId, usageToOverride.PeriodStart);

                if (transactionType.IsCredit)
                    balanceToUpdate.Value -= convertedUsageBalance;
                else
                    balanceToUpdate.Value += convertedUsageBalance;

                usageIdsToOverrideValue.Add(usageToOverride.AccountUsageId);
            }

            usageIdsToOverride = usageIdsToOverrideValue;
        }

        private void ProcessDeletedTransactions(IEnumerable<long> deletedTransactionIds, Dictionary<string, LiveBalanceToUpdate> balancesToUpdateByAccountId, out IEnumerable<long> overridenUsageIdsToRollback)
        {
            IEnumerable<AccountUsage> allOverridenUsagesToRollback = new AccountUsageManager().GetOverridenAccountUsagesByDeletedTransactionIds(deletedTransactionIds);
            var overridenUsageIdsToRollbackValue = new List<long>();

            foreach (AccountUsage overridenUsageToRollback in allOverridenUsagesToRollback)
            {
                LiveBalanceAccountInfo accountBalanceInfo = AccountsInfo.GetRecord(overridenUsageToRollback.AccountId);
                LiveBalanceToUpdate balanceToUpdate = balancesToUpdateByAccountId.GetRecord(overridenUsageToRollback.AccountId);

                BillingTransactionType transactionType = billingTransactionTypeManager.GetBillingTransactionType(overridenUsageToRollback.TransactionTypeId);
                decimal convertedUsageBalance = currencyExchangeRateManager.ConvertValueToCurrency(overridenUsageToRollback.UsageBalance, overridenUsageToRollback.CurrencyId, accountBalanceInfo.CurrencyId, overridenUsageToRollback.PeriodStart);

                if (transactionType.IsCredit)
                    balanceToUpdate.Value += convertedUsageBalance;
                else
                    balanceToUpdate.Value -= convertedUsageBalance;

                overridenUsageIdsToRollbackValue.Add(overridenUsageToRollback.AccountUsageId);
            }

            overridenUsageIdsToRollback = overridenUsageIdsToRollbackValue;
        }

        #endregion
    }

    public struct AccountTransaction
    {
        public String AccountId { get; set; }
        public Guid TransactionTypeId { get; set; }
    }
}
