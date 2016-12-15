﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.AccountBalance.Business
{
    public class AccountStatementManager
    {
        public Vanrise.Entities.IDataRetrievalResult<AccountStatementItem> GetFilteredAccountStatments(Vanrise.Entities.DataRetrievalInput<AccountStatementQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new AccountStatmentRequestHandler());
        }

        #region Private Classes
        private class AccountStatmentRequestHandler : BigDataRequestHandler<AccountStatementQuery, AccountStatementItem, AccountStatementItem>
        {
            public override AccountStatementItem EntityDetailMapper(AccountStatementItem entity)
            {
                return entity;
            }
            decimal _currenctBalance;
            string _currencyName;
            public override IEnumerable<AccountStatementItem> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<AccountStatementQuery> input)
            {
                AccountManager accountManager = new AccountManager();
                var accountInfo = accountManager.GetAccountInfo(input.Query.AccountTypeId, input.Query.AccountId);
                int currencyId = accountInfo.CurrencyId;
                CurrencyManager currencyManager = new CurrencyManager();
                _currencyName = currencyManager.GetCurrencyName(currencyId);
                 List<AccountStatementItem> accountStatementItems = BuildAccountStatementItems(input.Query.AccountTypeId, input.Query.AccountId, input.Query.FromDate, currencyId);
                var accountStatementItem = GetPendingAcountUsages(input.Query.AccountTypeId, input.Query.AccountId, currencyId);
                accountStatementItems.Add(accountStatementItem);
                return accountStatementItems;
            }
            protected override BigResult<AccountStatementItem> AllRecordsToBigResult(DataRetrievalInput<AccountStatementQuery> input, IEnumerable<AccountStatementItem> allRecords)
            {
                var query = input.Query;

                var analyticBigResult = new AccountStatementResult()
                {
                    ResultKey = input.ResultKey,
                    Data =  allRecords.VRGetPage(input),
                    TotalCount = allRecords.Count(),
                    CurrentBalance = _currenctBalance,
                    Currency = _currencyName
                };
                return analyticBigResult;
            }
            private List<AccountStatementItem> BuildAccountStatementItems(Guid accountTypeId, long accountId, DateTime fromDate, int currencyId)
            {
                CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
                BillingTransactionTypeManager billingTransactionTypeManager = new BillingTransactionTypeManager();
                BillingTransactionQuery billingTransactionQuery = new BillingTransactionQuery
                {
                    AccountsIds = new List<long> { accountId },
                    AccountTypeId = accountTypeId,
                    FromTime = new DateTime(1980, 01, 01),
                };
                IBillingTransactionDataManager billingTransactionDataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
                var billingTransactions = billingTransactionDataManager.GetFilteredBillingTransactions(billingTransactionQuery);

                List<AccountStatementItem> accountStatementItems = new List<AccountStatementItem>();
                decimal previousBalance = 0;
                foreach (var billingTransaction in billingTransactions)
                {
                    var transactionType = billingTransactionTypeManager.GetBillingTransactionType(billingTransaction.TransactionTypeId);
                    var convertedAmmount = billingTransaction.CurrencyId != currencyId ? currencyExchangeRateManager.ConvertValueToCurrency(billingTransaction.Amount, billingTransaction.CurrencyId, currencyId, billingTransaction.TransactionTime) : billingTransaction.Amount;

                    if (transactionType.IsCredit)
                    {
                        _currenctBalance += convertedAmmount;
                        if (billingTransaction.TransactionTime < fromDate)
                        {
                            previousBalance += convertedAmmount;
                        }

                    }
                    else
                    {
                        _currenctBalance -= convertedAmmount;
                        if (billingTransaction.TransactionTime < fromDate)
                        {
                            previousBalance -= convertedAmmount;
                        }
                    }
                }
                var balance = previousBalance;
                foreach (var billingTransaction in billingTransactions)
                {
                    var billingTransactionType = billingTransactionTypeManager.GetBillingTransactionType(billingTransaction.TransactionTypeId);
                    var amount = billingTransaction.CurrencyId != currencyId ? currencyExchangeRateManager.ConvertValueToCurrency(billingTransaction.Amount, billingTransaction.CurrencyId, currencyId, billingTransaction.TransactionTime) : billingTransaction.Amount;
                    AccountStatementItem accountStatementItem = new AccountStatementItem
                    {
                        TransactionTime = billingTransaction.TransactionTime,
                        Description = billingTransaction.Notes,
                        TransactionType = billingTransactionType.Name,
                    };
                    if (billingTransaction.TransactionTime > fromDate)
                    {
                        if (billingTransactionType.IsCredit)
                        {
                            balance += amount;
                            accountStatementItem.Balance = balance;
                            accountStatementItem.Credit = amount;
                        }
                        else
                        {
                            balance -= amount;
                            accountStatementItem.Balance = balance;
                            accountStatementItem.Debit = amount;
                        }
                        accountStatementItems.Add(accountStatementItem);
                    }
                }

                accountStatementItems.Insert(0,new AccountStatementItem
                {
                    Balance = previousBalance,
                    Description = "Previous Balance",
                });
                return accountStatementItems;

            }
            private AccountStatementItem GetPendingAcountUsages(Guid accountTypeId, long accountId,int currencyId)
            {
                AccountTypeManager accountTypeManager = new AccountTypeManager();
                BillingTransactionTypeManager billingTransactionTypeManager = new BillingTransactionTypeManager();
                CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();

                IAccountUsageDataManager accountUsageDataManager = AccountBalanceDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
                var pendingAccountUsages = accountUsageDataManager.GetPendingAccountUsages(accountTypeId, accountId);
                AccountStatementItem accountStatementItem = new AccountStatementItem();
                var trasactionId = accountTypeManager.GetUsageTransactionTypeId(accountTypeId);
                var transactionType = billingTransactionTypeManager.GetBillingTransactionType(trasactionId); 
                accountStatementItem.TransactionType = transactionType.Name;
                accountStatementItem.Description = "Pending Usage";
                bool isCredit = transactionType.IsCredit;
                foreach(var pendingAccountUsage in pendingAccountUsages)
                {
                    var amount = pendingAccountUsage.CurrencyId != currencyId ? currencyExchangeRateManager.ConvertValueToCurrency(pendingAccountUsage.UsageBalance, pendingAccountUsage.CurrencyId, currencyId, pendingAccountUsage.PeriodEnd) : pendingAccountUsage.UsageBalance;
                    if(isCredit)
                    {
                        accountStatementItem.Credit +=amount;
                    }
                        else
                        {
                            accountStatementItem.Debit+=amount;
                        }
                    
                }
                return accountStatementItem;
            }

        }
        #endregion

    }
}
