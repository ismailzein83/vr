using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.AccountBalance.Data;
using Vanrise.RDBTests.Common;
using Vanrise.Notification.Entities;
using Vanrise.AccountBalance.Entities;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.RDB.Tests.AccountBalance
{
    [TestClass]
    public class LiveBalanceDataManagerTests
    {
        const string DBTABLE_NAME_LIVEBALANCE = "LiveBalance";
        ILiveBalanceDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
        ILiveBalanceDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();

        IBillingTransactionDataManager _rdbBillingTransactionDataManager = RDBDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
        IBillingTransactionDataManager _sqlBillingTransactionDataManager = SQLDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();

        IAccountUsageDataManager _rdbAccountUsageDataManager = RDBDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
        IAccountUsageDataManager _sqlAccountUsageDataManager = SQLDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
        
        IBalanceUsageQueueDataManager _rdbBalanceUsageQueueDataManager = RDBDataManagerFactory.GetDataManager<IBalanceUsageQueueDataManager>();
        IBalanceUsageQueueDataManager _sqlBalanceUsageQueueDataManager = SQLDataManagerFactory.GetDataManager<IBalanceUsageQueueDataManager>();

        [TestMethod]
        public void TestAddAndSelectLiveBalance()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, DBTABLE_NAME_LIVEBALANCE);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, BillingTransactionDataManagerTests.DBTABLE_NAME_BILLINGTRANSACTION);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, AccountUsageDataManagerTests.DBTABLE_NAME_ACCOUNTUSAGE);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, AccountUsageOverrideDataManagerTests.DBTABLE_NAME_ACCOUNTUSAGEOVERRIDE);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, BalanceUsageQueueDataManagerTests.DBTABLE_NAME_BALANCEUSAGEQUEUE);

            Guid accountTypeId1 = Guid.NewGuid();
            TestAddAndSelectLiveBalance(accountTypeId1);
            Guid accountTypeId2 = Guid.NewGuid();
            TestAddAndSelectLiveBalance(accountTypeId2);
            Guid accountTypeId3 = Guid.NewGuid();
            TestAddAndSelectLiveBalance(accountTypeId3);
        }

        private void TestAddAndSelectLiveBalance(Guid accountTypeId)
        {
            List<Guid> accountTypeIds = new List<Guid> { accountTypeId };
            List<string> accountIds = new List<string> { "account1", "account 2", Guid.NewGuid().ToString() };
            List<int> currencyIds = new List<int> { 1, 2 };
            List<DateTime?> beds = new List<DateTime?> { null, DateTime.Now, DateTime.Today.AddDays(-10) };
            List<DateTime?> eeds = new List<DateTime?> { null, DateTime.Now, DateTime.Today.AddDays(-10), DateTime.Today.AddDays(10) };
            List<VRAccountStatus> statuses = new List<VRAccountStatus> { VRAccountStatus.Active, VRAccountStatus.InActive };
            List<Guid> transactionTypeIds = new List<Guid> { Guid.NewGuid(), new Guid("AF7A2D82-1416-4833-8B6E-033474BC2D20"), Guid.NewGuid() };
            List<long> liveBalanceIds = new List<long>();
            TestGetFiltered(accountTypeId, accountIds);
            UTUtilities.CallActionIteratively(
                (accountId, currencyId, bed, eed, status) => TryAddLiveBalanceAndCompare(liveBalanceIds, accountTypeId, accountId, currencyId, bed, eed, status),
                accountIds, currencyIds, beds, eeds, statuses);
            UTUtilities.CallActionIteratively(
                (accountId, currencyId, bed, eed, status) => TryAddLiveBalanceAndCompare(liveBalanceIds, accountTypeId, accountId, currencyId, bed, eed, status),
                accountIds, currencyIds, beds, eeds, statuses);

            TestGetFiltered(accountTypeId, accountIds);
            UTUtilities.CallActionIteratively(TestUpdateAccountStatus, accountTypeIds, accountIds);

            UTUtilities.CallActionIteratively(TestUpdateAccountEffectiveDate, accountTypeIds, accountIds);

            TestUpdateAccountRuleInfo(accountTypeId, accountIds);

            TestUpdateAccountLastAlertInfo(accountTypeId, accountIds);
            TestGetFiltered(accountTypeId, accountIds);
            TestGetBalancesForAlertOperations(accountTypeId);
            List<LiveBalanceToUpdate> balancesToUpdate = new List<LiveBalanceToUpdate>
            {
                new LiveBalanceToUpdate { LiveBalanceId = liveBalanceIds[0], Value = 33.54M},
                new LiveBalanceToUpdate { LiveBalanceId = liveBalanceIds[1], Value = 543.4M},
                new LiveBalanceToUpdate { LiveBalanceId = liveBalanceIds[2], Value = -345.54M},
            };

            List<AccountUsageInfo> accountUsages;
            TestUpdateLiveBalancesFromAccountUsages(balancesToUpdate, accountTypeId, accountIds, transactionTypeIds, currencyIds, out accountUsages);
            TestUpdateLiveBalancesFromBillingTransactions(balancesToUpdate, accountTypeId, accountIds, accountUsages, transactionTypeIds, currencyIds);
            TestGetBalancesForAlertOperations(accountTypeId);

            TestUpdateLiveBalancesFromAccountUsages(balancesToUpdate, accountTypeId, accountIds, transactionTypeIds, currencyIds, out accountUsages);
            TestUpdateLiveBalancesFromBillingTransactions(balancesToUpdate, accountTypeId, accountIds, accountUsages, transactionTypeIds, currencyIds);
            TestGetBalancesForAlertOperations(accountTypeId);

            TestUpdateLiveBalancesFromAccountUsages(balancesToUpdate, accountTypeId, accountIds, transactionTypeIds, currencyIds, out accountUsages);
            TestUpdateLiveBalancesFromBillingTransactions(balancesToUpdate, accountTypeId, accountIds, accountUsages, transactionTypeIds, currencyIds);
            TestGetBalancesForAlertOperations(accountTypeId);
        }

        private void TestUpdateLiveBalancesFromBillingTransactions(List<LiveBalanceToUpdate> balancesToUpdate, Guid accountTypeId, List<string> accountIds, 
            List<AccountUsageInfo> accountUsages, List<Guid> transactionTypeIds, List<int> currencyIds)
        {
            List<BillingTransaction> billingTransactions = InsertBillingTransactions(accountTypeId, accountIds, transactionTypeIds, currencyIds);
           
            
            Random random = new Random();

            List<long> billingTransactionIds = new List<long> { random.Next(billingTransactions.Count), random.Next(billingTransactions.Count), random.Next(billingTransactions.Count) };
            List<long> billingTransactionToDeleteIds = new List<long> { random.Next(billingTransactions.Count), random.Next(billingTransactions.Count), random.Next(billingTransactions.Count) };
        
            List<long> accountUsageToOverrideIds = new List<long> { random.Next(accountUsages.Count), random.Next(accountUsages.Count), random.Next(accountUsages.Count) };
            DateTime now = DateTime.Now;
            List<AccountUsageOverride> usageOverrides = new List<AccountUsageOverride>
            {
                new AccountUsageOverride
                {
                    AccountTypeId = accountTypeId,
                    AccountId = accountIds[0],
                    OverriddenByTransactionId = billingTransactions[0].AccountBillingTransactionId,
                    TransactionTypeId = billingTransactions[0].TransactionTypeId,
                    PeriodStart = DateTime.Today.AddDays(-1),
                    PeriodEnd = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second)
                },
                new AccountUsageOverride
                {
                    AccountTypeId = accountTypeId,
                    AccountId = accountIds[0],
                    OverriddenByTransactionId = billingTransactions[1].AccountBillingTransactionId,
                    TransactionTypeId = billingTransactions[1].TransactionTypeId,
                    PeriodStart = DateTime.Today.AddDays(-5),
                    PeriodEnd = DateTime.Today.AddDays(-1)
                }
            };

            List<long> overriddenUsagesToRollback = new List<long> { 1, 2 };


            bool sqlResult = _sqlDataManager.UpdateLiveBalancesFromBillingTransactions(balancesToUpdate, billingTransactionIds, accountUsageToOverrideIds, usageOverrides, overriddenUsagesToRollback, billingTransactionToDeleteIds);
            bool rdbResult = _rdbDataManager.UpdateLiveBalancesFromBillingTransactions(balancesToUpdate, billingTransactionIds, accountUsageToOverrideIds, usageOverrides, overriddenUsagesToRollback, billingTransactionToDeleteIds);

            UTUtilities.AssertValuesAreEqual(sqlResult, rdbResult);

            AssertAllTablesAreSimilar();
        }

        private void TestUpdateLiveBalancesFromAccountUsages(List<LiveBalanceToUpdate> balancesToUpdate, Guid accountTypeId, List<string> accountIds,
            List<Guid> transactionTypeIds, List<int> currencyIds, out List<AccountUsageInfo> accountUsages)
        {
            accountUsages = InsertAccountUsages(accountTypeId, accountIds, transactionTypeIds, currencyIds);
            var updateUsageBalancePayload = new UpdateUsageBalancePayload { TransactionTypeId = transactionTypeIds[0], UpdateUsageBalanceItems = new List<UpdateUsageBalanceItem> { } };
            _sqlBalanceUsageQueueDataManager.UpdateUsageBalance<UpdateUsageBalancePayload>(accountTypeId, BalanceUsageQueueType.UpdateUsageBalance, updateUsageBalancePayload);
            _rdbBalanceUsageQueueDataManager.UpdateUsageBalance<UpdateUsageBalancePayload>(accountTypeId, BalanceUsageQueueType.UpdateUsageBalance, updateUsageBalancePayload);
            var payload2 = updateUsageBalancePayload.VRDeepCopy();
            _sqlBalanceUsageQueueDataManager.UpdateUsageBalance<UpdateUsageBalancePayload>(accountTypeId, BalanceUsageQueueType.UpdateUsageBalance, payload2);
            _rdbBalanceUsageQueueDataManager.UpdateUsageBalance<UpdateUsageBalancePayload>(accountTypeId, BalanceUsageQueueType.UpdateUsageBalance, payload2);
            var payload3 = updateUsageBalancePayload.VRDeepCopy();
            _sqlBalanceUsageQueueDataManager.UpdateUsageBalance<UpdateUsageBalancePayload>(accountTypeId, BalanceUsageQueueType.UpdateUsageBalance, payload3);
            _rdbBalanceUsageQueueDataManager.UpdateUsageBalance<UpdateUsageBalancePayload>(accountTypeId, BalanceUsageQueueType.UpdateUsageBalance, payload3);
            Random random = new Random();
            List<AccountUsageToUpdate> usagesToUpdate = new List<AccountUsageToUpdate>
            {
                new AccountUsageToUpdate
                {
                    AccountUsageId = random.Next(accountUsages.Count),
                    Value = 343.5345M
                },
                new AccountUsageToUpdate
                {
                    AccountUsageId = random.Next(accountUsages.Count),
                    Value = -543.546M
                }//,
                //new AccountUsageToUpdate
                //{
                //    AccountUsageId = random.Next(accountUsages.Count),
                //    Value = -4.534M
                //}
            };

            UTUtilities.AssertValuesAreEqual(_sqlDataManager.UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(1, balancesToUpdate, usagesToUpdate, null),
            _rdbDataManager.UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(1, balancesToUpdate, usagesToUpdate, null));

            AssertAllTablesAreSimilar();
            Guid correctionProcessId = Guid.NewGuid();
            UTUtilities.AssertValuesAreEqual(_sqlDataManager.UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(2, balancesToUpdate, usagesToUpdate, correctionProcessId),
            _rdbDataManager.UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(2, balancesToUpdate, usagesToUpdate, correctionProcessId));

            AssertAllTablesAreSimilar();


            UTUtilities.AssertValuesAreEqual(_sqlDataManager.UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(22, balancesToUpdate, usagesToUpdate, null),
            _rdbDataManager.UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(22, balancesToUpdate, usagesToUpdate, null));

            AssertAllTablesAreSimilar();

        }

        private List<AccountUsageInfo> InsertAccountUsages(Guid accountTypeId, List<string> accountIds, List<Guid> transactionTypeIds, List<int> currencyIds)
        {
            List<AccountUsageInfo> accountUsages = new List<AccountUsageInfo>();

            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        
            List<DateTime> periodStarts = new List<DateTime> { now, now.AddDays(-2), now.AddDays(-10) };
            List<DateTime> periodEnds = new List<DateTime> { now, now.AddDays(-2), now.AddDays(-10), now.AddDays(2), now.AddDays(10) };

            UTUtilities.CallActionIteratively((accountId, transactionTypeId, currencyId, periodStart, periodEnd) =>
            {
                var sqlAccountUsage = _sqlAccountUsageDataManager.TryAddAccountUsageAndGet(accountTypeId, transactionTypeId, accountId, periodStart, periodEnd, currencyId, 0);
                var rdbAccountUsage = _rdbAccountUsageDataManager.TryAddAccountUsageAndGet(accountTypeId, transactionTypeId, accountId, periodStart, periodEnd, currencyId, 0);

                UTUtilities.AssertObjectsAreSimilar(sqlAccountUsage, rdbAccountUsage);
                accountUsages.Add(rdbAccountUsage);
            }, accountIds, transactionTypeIds, currencyIds, periodStarts, periodEnds);
            return accountUsages;
        }

        private List<BillingTransaction> InsertBillingTransactions(Guid accountTypeId, List<string> accountIds, List<Guid> transactionTypeIds, List<int> currencyIds)
        {
            List<BillingTransaction> billingTransactions = new List<BillingTransaction>();
            
            List<Decimal> amounts = new List<decimal> { 3.35M, 64.5435M, 3452.23M };
            List<DateTime> transactionTimes = new List<DateTime> { DateTime.Now, DateTime.Now.AddDays(-4), DateTime.Now.AddDays(2) };

            UTUtilities.CallActionIteratively((accountId, transactionTypeId, amount, currencyId, transactionTime) =>
            {
                var billingTransaction = new BillingTransaction
                {
                    AccountTypeId = accountTypeId,
                    AccountId = accountId,
                    TransactionTypeId = transactionTypeId,
                    Amount = amount,
                    CurrencyId = currencyId,
                    TransactionTime = transactionTime
                };

                long rdbTransactionId;
                long sqlTransactionId;
                UTUtilities.AssertValuesAreEqual(_sqlBillingTransactionDataManager.Insert(billingTransaction, out sqlTransactionId),
                    _rdbBillingTransactionDataManager.Insert(billingTransaction, out rdbTransactionId));
                UTUtilities.AssertValuesAreEqual(sqlTransactionId, rdbTransactionId);
                billingTransaction.AccountBillingTransactionId = rdbTransactionId;
                billingTransactions.Add(billingTransaction);
            }, accountIds, transactionTypeIds, amounts, currencyIds, transactionTimes);
            return billingTransactions;
        }
        
        private void TryAddLiveBalanceAndCompare(List<long> liveBalanceIds, Guid accountTypeId, string accountId, int currencyId, DateTime? bed, DateTime? eed, VRAccountStatus status)
        {
            var sqlLiveBalance = _sqlDataManager.TryAddLiveBalanceAndGet(accountId, accountTypeId, 0, currencyId, 0, bed, eed, status, false);
            var rdbLiveBalance = _rdbDataManager.TryAddLiveBalanceAndGet(accountId, accountTypeId, 0, currencyId, 0, bed, eed, status, false);

            UTUtilities.AssertValuesAreEqual(sqlLiveBalance.LiveBalanceId, rdbLiveBalance.LiveBalanceId);
            if (!liveBalanceIds.Contains(rdbLiveBalance.LiveBalanceId))
                liveBalanceIds.Add(rdbLiveBalance.LiveBalanceId);
            AssertLiveBalanceIsSimilar(accountTypeId, accountId);
        }

        private void TestUpdateAccountStatus(Guid accountTypeId, string accountId)
        {
            var statuses = new List<VRAccountStatus> { VRAccountStatus.Active, VRAccountStatus.InActive , VRAccountStatus.Active};
            UTUtilities.CallActionIteratively((status) =>
            {
                UTUtilities.AssertValuesAreEqual(_sqlDataManager.TryUpdateLiveBalanceStatus(accountId, accountTypeId, status, false), _rdbDataManager.TryUpdateLiveBalanceStatus(accountId, accountTypeId, status, false));

                AssertLiveBalanceIsSimilar(accountTypeId, accountId);
            }, statuses);
        }

        private void TestUpdateAccountEffectiveDate(Guid accountTypeId, string accountId)
        {
            List<DateTime?> beds = new List<DateTime?> { null, DateTime.Now, DateTime.Today.AddMonths(-5) };
            List<DateTime?> eeds = new List<DateTime?> { null, DateTime.Now, DateTime.Today.AddMonths(5) };
            UTUtilities.CallActionIteratively((bed, eed) =>
            {
                UTUtilities.AssertValuesAreEqual(_sqlDataManager.TryUpdateLiveBalanceEffectiveDate(accountId, accountTypeId, bed, eed), _rdbDataManager.TryUpdateLiveBalanceEffectiveDate(accountId, accountTypeId, bed, eed));
                AssertLiveBalanceIsSimilar(accountTypeId, accountId);
            }, beds, eeds);
        }

        private void TestUpdateAccountRuleInfo(Guid accountTypeId, List<string> accountIds)
        {
            List<long?> alertRuleIds = new List<long?> { null, 3, 0, null, 32 };
            List<decimal?> nextThresholds = new List<decimal?> { null, 3.5M, null, 544.32M };

            UTUtilities.CallActionIteratively((alertRuleId, nextThreshold) =>
            {
                var updateInput = accountIds.Select(accountId =>
                new Vanrise.AccountBalance.Entities.LiveBalanceNextThresholdUpdateEntity
                {
                    AccountTypeId = accountTypeId,
                    AccountId = accountId,
                    AlertRuleId = alertRuleId,
                    NextAlertThreshold = nextThreshold
                }
                ).ToList();
                _sqlDataManager.UpdateBalanceRuleInfos(updateInput);
                _rdbDataManager.UpdateBalanceRuleInfos(updateInput);

                AssertAllAccountsAreSimilar(accountTypeId);
            }, alertRuleIds, nextThresholds);
        }

        private void TestUpdateAccountLastAlertInfo(Guid accountTypeId, List<string> accountIds)
        {
            List<decimal?> lastThresholds = new List<decimal?> { null, 3.5M, null, 544.32M };
            List<VRBalanceActiveAlertInfo> activeAlertsInfos = new List<VRBalanceActiveAlertInfo>
            {
                null,
                new VRBalanceActiveAlertInfo
                    {
                        ActiveAlertsThersholds = new System.Collections.Generic.List<VRBalanceActiveAlertThreshold>
                        {
                            new VRBalanceActiveAlertThreshold
                            {
                                AlertRuleId = 5,
                               Threshold = 55.543M
                            },
                            new VRBalanceActiveAlertThreshold
                            {
                                AlertRuleId = 654,
                               Threshold = 59084.44M
                            }
                        }
                    },
                null,
                new VRBalanceActiveAlertInfo
                    {
                        ActiveAlertsThersholds = new System.Collections.Generic.List<VRBalanceActiveAlertThreshold>
                        {
                            new VRBalanceActiveAlertThreshold
                            {
                                AlertRuleId = 2,
                               Threshold = 589084.44M
                            }
                        }
                    }
            };

            UTUtilities.CallActionIteratively((lastThreshold, activeAlertInfo) =>
            {
                var updateInput = accountIds.Select(accountId =>
                new Vanrise.AccountBalance.Entities.LiveBalanceLastThresholdUpdateEntity
                {
                    AccountTypeId = accountTypeId,
                    AccountId = accountId,
                    LastExecutedActionThreshold = lastThreshold,
                    ActiveAlertsInfo = activeAlertInfo
                }
                ).ToList();
                _sqlDataManager.UpdateBalanceLastAlertInfos(updateInput);
                _rdbDataManager.UpdateBalanceLastAlertInfos(updateInput);

                AssertAllAccountsAreSimilar(accountTypeId);
            }, lastThresholds, activeAlertsInfos);
        }

        private void TestGetBalancesForAlertOperations(Guid accountTypeId)
        {
            UTUtilities.AssertValuesAreEqual(_sqlDataManager.HasLiveBalancesUpdateData(accountTypeId), _rdbDataManager.HasLiveBalancesUpdateData(accountTypeId));

            List<LiveBalance> sqlBalancesToAlert = new List<LiveBalance>();
            List<LiveBalance> rdbBalancesToAlert = new List<LiveBalance>();
            _sqlDataManager.GetLiveBalancesToAlert(accountTypeId, (bal) => sqlBalancesToAlert.Add(bal));
            _rdbDataManager.GetLiveBalancesToAlert(accountTypeId, (bal) => rdbBalancesToAlert.Add(bal));
            AssertLiveBalancesAreSimilar(sqlBalancesToAlert, rdbBalancesToAlert);

            List<LiveBalance> sqlBalancesToClearAlert = new List<LiveBalance>();
            List<LiveBalance> rdbBalancesToClearAlert = new List<LiveBalance>();
            _sqlDataManager.GetLiveBalancesToClearAlert(accountTypeId, (bal) => sqlBalancesToClearAlert.Add(bal));
            _rdbDataManager.GetLiveBalancesToClearAlert(accountTypeId, (bal) => rdbBalancesToClearAlert.Add(bal));
            AssertLiveBalancesAreSimilar(sqlBalancesToClearAlert, rdbBalancesToClearAlert);
        }

        private void AssertLiveBalanceIsSimilar(Guid accountTypeId, string accountId)
        {
            var sqlLiveBalance = _sqlDataManager.GetLiveBalance(accountTypeId, accountId);
            var rdbLiveBalance = _rdbDataManager.GetLiveBalance(accountTypeId, accountId);

            UTUtilities.AssertObjectsAreSimilar(sqlLiveBalance, rdbLiveBalance);

            AssertAllAccountsAreSimilar(accountTypeId);
        }

        private void AssertAllAccountsAreSimilar(Guid accountTypeId)
        {
            TestGetLiveBalances(accountTypeId);

            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, DBTABLE_NAME_LIVEBALANCE);
        }

        private void TestGetLiveBalances(Guid accountTypeId)
        {
            var sqlResponse = new List<LiveBalance>();
            var rdbResponse = new List<LiveBalance>();

            _sqlDataManager.GetLiveBalanceAccounts(accountTypeId, (bal) => sqlResponse.Add(bal));
            _rdbDataManager.GetLiveBalanceAccounts(accountTypeId, (bal) => rdbResponse.Add(bal));

            UTUtilities.AssertObjectsAreSimilar(sqlResponse, rdbResponse);

            var sqlResponse2 = _sqlDataManager.GetLiveBalanceAccountsInfo(accountTypeId);
            var rdbResponse2 = _rdbDataManager.GetLiveBalanceAccountsInfo(accountTypeId);            

            UTUtilities.AssertObjectsAreSimilar(sqlResponse2, rdbResponse2);
        }

        private void TestGetFiltered(Guid accountTypeId, List<string> accountIds)
        {
            List<List<string>> accountIdsList = new List<List<string>> { new List<string>(), accountIds };
            List<string> signs = new List<string> { ">", ">=", "<", "<=" };
             List<Decimal> balances = new List<decimal> { 0, 200 };
            List<DateTime?> effectiveDates = new List<DateTime?> { null, DateTime.Now};

            UTUtilities.CallActionIteratively((accountIdList, sign, balance, status,effectiveDate,  isEffectiveInFuture) =>
            {
                var query = new AccountBalanceQuery
                {
                    AccountTypeId = accountTypeId,
                    AccountsIds = accountIdList,
                    Sign = sign,
                    Top = 1000,
                    Balance = balance,
                    Status = status,
                    EffectiveDate = effectiveDate,
                    IsEffectiveInFuture = isEffectiveInFuture
                };
                AssertAccountBalancesAreSimilar(_sqlDataManager.GetFilteredAccountBalances(query), _rdbDataManager.GetFilteredAccountBalances(query));

            }, accountIdsList, signs,balances, UTUtilities.GetEnumListForTesting<VRAccountStatus?>(), 
            effectiveDates, UTUtilities.GetNullableBoolListForTesting());
        }

        private void AssertAccountBalancesAreSimilar(IEnumerable<Vanrise.AccountBalance.Entities.AccountBalance> sqlBalances, IEnumerable<Vanrise.AccountBalance.Entities.AccountBalance> rdbBalances)
        {
            UTUtilities.AssertObjectsAreSimilar(sqlBalances, rdbBalances);
        }

        private void AssertLiveBalancesAreSimilar(IEnumerable<LiveBalance> sqlBalances, IEnumerable<LiveBalance> rdbBalances)
        {
            UTUtilities.AssertObjectsAreSimilar(sqlBalances, rdbBalances);
        }

        private void AssertAllTablesAreSimilar()
        {
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, DBTABLE_NAME_LIVEBALANCE);
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, BillingTransactionDataManagerTests.DBTABLE_NAME_BILLINGTRANSACTION);
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, AccountUsageDataManagerTests.DBTABLE_NAME_ACCOUNTUSAGE);
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, AccountUsageOverrideDataManagerTests.DBTABLE_NAME_ACCOUNTUSAGEOVERRIDE);
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, BalanceUsageQueueDataManagerTests.DBTABLE_NAME_BALANCEUSAGEQUEUE);
        }
    }
}
