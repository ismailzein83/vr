﻿using System;
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
        bool LoadingError;
        public AcountBalanceUpdateHandler()
        {
            currencyExchangeRateManager = new CurrencyExchangeRateManager();
            IntializeAccountsInfo();
        }
        private void IntializeAccountsInfo()
        {
            AccountsInfo = new Dictionary<long, LiveBalanceAccountInfo>();
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            var accountBlances = dataManager.GetLiveBalanceAccountsInfo();
            AccountsInfo = accountBlances.ToDictionary(x => x.AccountId, x => x);
        }
        public void AddAndUpdateLiveBalanceFromBillingTransction(List<BillingTransaction> billingTransactions)
        {
            if (this.LoadingError)
            {
                IntializeAccountsInfo();
            }

            long accountId = billingTransactions.FirstOrDefault().AccountId;

            LiveBalanceAccountInfo accountInfo = null;

            if (!AccountsInfo.ContainsKey(accountId))
            {
                AccountManager manager = new AccountManager();
                var account = manager.GetAccountInfo(accountId);

                accountInfo = new LiveBalanceAccountInfo { AccountId = accountId, CurrencyId = account.CurrencyId };

                if (AddLiveBalance(accountId, accountInfo.CurrencyId))
                {
                    AccountsInfo.Add(accountId, accountInfo);
                }
                else
                {
                    LoadingError = true;
                    throw new Exception(string.Format("Same account id {0} Exist.", accountId));
                }

            }
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();

            if(accountInfo == null)
            {
                AccountsInfo.TryGetValue(accountId, out accountInfo);
            }
            
            decimal amount = 0;
            List<long> billingTransactionIds = new List<long>();
            foreach (var billingTransaction in billingTransactions)
            {
                billingTransactionIds.Add(billingTransaction.AccountBillingTransactionId);
                accountId = billingTransaction.AccountId;
                amount += ConvertValueToCurrency(billingTransaction.Amount, billingTransaction.CurrencyId, accountInfo.CurrencyId, billingTransaction.TransactionTime);
            }
            dataManager.UpdateLiveBalanceFromBillingTransaction(accountId, billingTransactionIds, amount);
        }
        public void AddAndUpdateLiveBalanceFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<UsageBalanceUpdate> usageBalanceUpdates)
        {
            if(this.LoadingError)
            {
                IntializeAccountsInfo();
            }

            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            var accountsToInsert = usageBalanceUpdates.Where(x => !AccountsInfo.ContainsKey(x.AccountId)).Distinct();
            foreach(var accountToInsert in accountsToInsert)
            {
                if (AddLiveBalance(accountToInsert.AccountId, accountToInsert.CurrencyId))
                {
                   AccountsInfo.Add(accountToInsert.AccountId, new LiveBalanceAccountInfo { AccountId = accountToInsert.AccountId, CurrencyId = accountToInsert.CurrencyId });
                }
                else
                {
                    LoadingError = true;
                   throw new Exception(string.Format("Same account id {0} Exist.",accountToInsert.AccountId));
                }
            }
            foreach (var itm in usageBalanceUpdates)
            {
                LiveBalanceAccountInfo accountInfo = null;
                AccountsInfo.TryGetValue(itm.AccountId, out accountInfo);
                itm.Value = currencyExchangeRateManager.ConvertValueToCurrency(itm.Value, itm.CurrencyId, accountInfo.CurrencyId, itm.EffectiveOn);
            }
            dataManager.UpdateLiveBalanceFromBalanceUsageQueue(usageBalanceUpdates, balanceUsageQueueId);
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

            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            return dataManager.Insert(liveBalance);
        }
        private decimal ConvertValueToCurrency(decimal amount , int fromCurrencyId,int currencyId,DateTime effectiveOn)
        {
            return currencyExchangeRateManager.ConvertValueToCurrency(amount, fromCurrencyId, currencyId, effectiveOn);
        }

    }
}
