using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.AccountBalance.Data;
using Vanrise.RDBTests.Common;
using System.Collections.Generic;
using Vanrise.AccountBalance.Entities;
using Vanrise.Entities;

namespace Vanrise.RDB.Tests.AccountBalance
{
    [TestClass]
    public class AccountUsageDataManagerTests
    {
        internal const string DBTABLE_NAME_ACCOUNTUSAGE = "AccountUsage";

        IAccountUsageDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IAccountUsageDataManager>();
        IAccountUsageDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IAccountUsageDataManager>();

        ILiveBalanceDataManager _rdbLiveBalanceDataManager = RDBDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
        ILiveBalanceDataManager _sqlLiveBalanceDataManager = SQLDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();

        [TestMethod]
        public void TestInsertSelectAccountUsages()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, DBTABLE_NAME_ACCOUNTUSAGE);
            Guid accountTypeId1 = Guid.NewGuid();
            TestInsertSelect(accountTypeId1);
            Guid accountTypeId2 = Guid.NewGuid();
            TestInsertSelect(accountTypeId2);
            Guid accountTypeId3 = Guid.NewGuid();
            TestInsertSelect(accountTypeId3);
        }

        private void TestInsertSelect(Guid accountTypeId)
        {
            List<string> accountIds;
            List<int> currencyIds;
            List<DateTime> fromTimes;
            List<DateTime> toTimes;
            List<Guid> transactionTypeIds;

            var usageInfos = InsertAccountUsages(accountTypeId, out accountIds, out currencyIds, out transactionTypeIds, out fromTimes, out toTimes);

            AssertTablesAreSimilar();

            TestGetAccountUsages(accountTypeId, accountIds, fromTimes, toTimes, transactionTypeIds, usageInfos);
        }

        private void TestGetAccountUsages(Guid accountTypeId, List<string> accountIds, List<DateTime> fromTimes, List<DateTime> toTimes, List<Guid> transactionTypeIds, List<AccountUsageInfo> usageInfos)
        {
            List<DateTime> fromTimesToTest = new List<DateTime>(fromTimes);
            fromTimesToTest.Add(DateTime.Now);
            List<DateTime?> toTimesToTest = new List<DateTime?>(toTimes.Cast<DateTime?>());
            toTimesToTest.Add(DateTime.Now);
            toTimesToTest.Add(null);
            List<Guid> transactionTypeIdsToTest = new List<Guid>(transactionTypeIds);
            transactionTypeIdsToTest.Add(Guid.NewGuid());
            List<string> accountIdsToTest = new List<string>(accountIds);
            accountIdsToTest.Add(null);
            accountIdsToTest.Add("");

            List<List<string>> accountIdList = new List<List<string>>
            {
                null,
                new List<string>(),
                new List<string> { "erterwt", accountIds[0]},
                accountIds
            };
            List<List<Guid>> transactionTypeIdList = new List<List<Guid>>
            {
                null,
                new List<Guid>(),
                new List<Guid>{Guid.NewGuid(), transactionTypeIds[0]},
                transactionTypeIds
            };
            List<DateTime?> effectiveDates = new List<DateTime?> { null, DateTime.Now, DateTime.Now.AddDays(-100) };

            Random r = new Random();
            for (var i = 0; i < 4; i++)
            {
                var query = string.Format("Update [VR_AccountBalance].[AccountUsage] SET [IsOverridden] = 1 where ID = {0}", r.Next((int)usageInfos.Min(itm => itm.AccountUsageId), (int)usageInfos.Max(itm => itm.AccountUsageId)));
                UTUtilities.ExecuteDBNonQuery(Constants.CONNSTRING_NAME_CONFIG, query);
            }

            TestGetAccountsUsageInfoByPeriod(accountTypeId, fromTimesToTest, transactionTypeIdsToTest);

            List<Guid> correctionProcessIds = new List<Guid>();

            for (var i = 0; i < 4; i++)
            {
                Guid correctionProcessId = Guid.NewGuid();
                var query = string.Format("Update [VR_AccountBalance].[AccountUsage] SET [CorrectionProcessID] = '{0}' where ID = {1}", correctionProcessId, r.Next((int)usageInfos.Min(itm => itm.AccountUsageId), (int)usageInfos.Max(itm => itm.AccountUsageId)));
                UTUtilities.ExecuteDBNonQuery(Constants.CONNSTRING_NAME_CONFIG, query);
                correctionProcessIds.Add(correctionProcessId);
            }
            correctionProcessIds.Add(Guid.NewGuid());
            TestGetAccountUsageErrorData(accountTypeId, fromTimesToTest, transactionTypeIdsToTest, correctionProcessIds);
            TestGetAccountUsageForBillingTransactions(accountTypeId, fromTimesToTest, toTimesToTest, accountIdList, transactionTypeIdList, effectiveDates);
            TestGetAccountUsageForSpecificPeriodByAccountIds(accountTypeId, fromTimesToTest, transactionTypeIdsToTest, accountIdList);
            TestGetAccountUsagesByAccount(accountTypeId, accountIdsToTest, effectiveDates);
            TestGetAccountUsagesByAccountIds(accountTypeId, accountIdList, transactionTypeIdList);
            TestGetLastAccountUsage(accountTypeId, accountIdsToTest);

            TestGetAccountUsagesByTransactionTypes(accountTypeId, accountIds, toTimes, transactionTypeIds);
            TestGetAccountUsagesByTransactionAccountUsageQueries(accountTypeId, fromTimesToTest, toTimesToTest, transactionTypeIdsToTest, accountIdsToTest);

            TestGetOverridenAccountUsagesByDeletedTransactionIds(accountTypeId, accountIds, transactionTypeIds, r);
        }

        private void TestGetOverridenAccountUsagesByDeletedTransactionIds(Guid accountTypeId, List<string> accountIds, List<Guid> transactionTypeIds, Random r)
        {
            List<long> overriddenTransactionIds = new List<long>();
            long transactionId = r.Next();
            InsertAccountUsageOverride(accountTypeId, accountIds, transactionTypeIds, DateTime.Today.AddDays(-2), DateTime.Today, transactionId);
            overriddenTransactionIds.Add(transactionId);
            transactionId = r.Next();
            InsertAccountUsageOverride(accountTypeId, accountIds, transactionTypeIds, DateTime.Today.AddDays(-20), DateTime.Today.AddDays(-10), transactionId);
            overriddenTransactionIds.Add(transactionId);
            transactionId = r.Next();
            InsertAccountUsageOverride(accountTypeId, accountIds, transactionTypeIds, DateTime.Today.AddDays(-20), DateTime.Today.AddDays(+20), transactionId);
            overriddenTransactionIds.Add(transactionId);

            List<List<long>> deletedTransactionIdList = new List<List<long>>
            {
                null,
                new List<long>(),
                new List<long> { 4, overriddenTransactionIds[0], overriddenTransactionIds[1], overriddenTransactionIds[2]}
            };
            UTUtilities.CallActionIteratively(
                (deletedTransactionIds) =>
                {
                    var sqlUsages = _sqlDataManager.GetOverridenAccountUsagesByDeletedTransactionIds(deletedTransactionIds);
                    var rdbUsages = _rdbDataManager.GetOverridenAccountUsagesByDeletedTransactionIds(deletedTransactionIds);
                    if (sqlUsages != null)
                        sqlUsages = sqlUsages.OrderBy(u => u.AccountUsageId).ToList();
                    if (rdbUsages != null)
                        rdbUsages = rdbUsages.OrderBy(u => u.AccountUsageId).ToList();

                    AssertUsagesAreSimilar(sqlUsages, rdbUsages);
                }, deletedTransactionIdList);
        }

        private static void InsertAccountUsageOverride(Guid accountTypeId, List<string> accountIds, List<Guid> transactionTypeIds, 
            DateTime periodStart, DateTime periodEnd, long transactionId)
        {
            var query = string.Format(@"Insert INTO VR_AccountBalance.AccountUsageOverride
(AccountTypeID, AccountID, TransactionTypeID, PeriodStart, PeriodEnd, OverriddenByTransactionID )
Values ('{0}', '{1}', '{2}', '{3}', '{4}', {5})", accountTypeId, accountIds[0], transactionTypeIds[0], 
periodStart.ToString("yyyy-MM-dd HH:mm:ss"), periodEnd.ToString("yyyy-MM-dd HH:mm:ss"), transactionId);
            UTUtilities.ExecuteDBNonQuery(Constants.CONNSTRING_NAME_CONFIG, query);
        }

        private void TestGetAccountUsagesByTransactionAccountUsageQueries(Guid accountTypeId, List<DateTime> fromTimesToTest, List<DateTime?> toTimesToTest, List<Guid> transactionTypeIdsToTest, List<string> accountIdsToTest)
        {
            List<List<TransactionAccountUsageQuery>> accountUsageQueryList = new List<List<TransactionAccountUsageQuery>>
            {
                new List<TransactionAccountUsageQuery>(),
                new List<TransactionAccountUsageQuery>()
            };
            UTUtilities.CallActionIteratively(
                (transactionTypeId, accountId, fromTime, toTime) =>
                {
                    if (toTime.HasValue && accountId != null)
                    {
                        List<TransactionAccountUsageQuery> queries = accountUsageQueryList.Last();
                        if (queries.Count >= accountUsageQueryList.Count - 1)
                        {
                            queries = new List<TransactionAccountUsageQuery>();
                            accountUsageQueryList.Add(queries);
                        }

                        var query = new TransactionAccountUsageQuery
                        {
                            AccountTypeId = accountTypeId,
                            AccountId = accountId,
                            TransactionTypeId = transactionTypeId,
                            PeriodStart = fromTime,
                            PeriodEnd = toTime.Value
                        };
                        queries.Add(query);
                    }
                }, transactionTypeIdsToTest, accountIdsToTest, fromTimesToTest, toTimesToTest);

            foreach (var query in accountUsageQueryList)
            {
                var sqlUsages = _sqlDataManager.GetAccountUsagesByTransactionAccountUsageQueries(query);
                var rdbUsages = _rdbDataManager.GetAccountUsagesByTransactionAccountUsageQueries(query);
                if (sqlUsages != null)
                    sqlUsages = sqlUsages.OrderBy(u => u.AccountUsageId).ToList();
                if (rdbUsages != null)
                    rdbUsages = rdbUsages.OrderBy(u => u.AccountUsageId).ToList();
                AssertUsagesAreSimilar(sqlUsages, rdbUsages);
            }
        }

        private void TestGetAccountUsagesByTransactionTypes(Guid accountTypeId, List<string> accountIds, List<DateTime> toTimes, List<Guid> transactionTypeIds)
        {
            List<List<AccountUsageByTime>> usageByTimeList = new List<List<AccountUsageByTime>>
            {
                null,
                new List<AccountUsageByTime>(),
                new List<AccountUsageByTime>
                {
                    new AccountUsageByTime
                    {
                        AccountId = accountIds[1],
                        EndPeriod = toTimes[1]
                    },
                    new AccountUsageByTime
                    {
                        AccountId = "ferterwt",
                        EndPeriod = toTimes[1]
                    },
                    new AccountUsageByTime
                    {
                        AccountId = "fsedrferwterterw",
                        EndPeriod = DateTime.Now.AddDays(344)
                    }
                }
            };
            List<List<Guid>> transactionTypeIdList2 = new List<List<Guid>>
            {
                new List<Guid>{Guid.NewGuid(), transactionTypeIds[0]},
                transactionTypeIds
            };

            UTUtilities.CallActionIteratively(
                (usageByTime, transactionTypesIdsLocal) =>
                {
                    var sqlUsages = _sqlDataManager.GetAccountUsagesByTransactionTypes(accountTypeId, usageByTime, transactionTypesIdsLocal);
                    var rdbUsages = _rdbDataManager.GetAccountUsagesByTransactionTypes(accountTypeId, usageByTime, transactionTypesIdsLocal);

                    AssertUsagesAreSimilar(sqlUsages, rdbUsages);
                }, usageByTimeList, transactionTypeIdList2);
        }

        private void TestGetLastAccountUsage(Guid accountTypeId, List<string> accountIdsToTest)
        {
            UTUtilities.CallActionIteratively(
                            (accountId) =>
                            {
                                var sqlUsage = _sqlDataManager.GetLastAccountUsage(accountTypeId, accountId);
                                var rdbUsage = _rdbDataManager.GetLastAccountUsage(accountTypeId, accountId);

                                AssertUsageIsSimilar(sqlUsage, rdbUsage);
                            }, accountIdsToTest);
        }

        private void TestGetAccountUsagesByAccountIds(Guid accountTypeId, List<List<string>> accountIdList, List<List<Guid>> transactionTypeIdList)
        {
            UTUtilities.CallActionIteratively(
                            (transactionTypeIdsLocal, accountIdsLocal) =>
                            {
                                var sqlUsages = _sqlDataManager.GetAccountUsagesByAccountIds(accountTypeId, transactionTypeIdsLocal, accountIdsLocal);
                                var rdbUsages = _rdbDataManager.GetAccountUsagesByAccountIds(accountTypeId, transactionTypeIdsLocal, accountIdsLocal);

                                AssertUsagesAreSimilar(sqlUsages, rdbUsages);
                            }, transactionTypeIdList, accountIdList);
        }

        private void TestGetAccountUsagesByAccount(Guid accountTypeId, List<string> accountIdsToTest, List<DateTime?> effectiveDates)
        {
            UTUtilities.CallActionIteratively(
                            (accountId, status, effectiveDate, isEffectiveInFuture) =>
                            {
                                var sqlUsages = _sqlDataManager.GetAccountUsagesByAccount(accountTypeId, accountId, status, effectiveDate, isEffectiveInFuture);
                                var rdbUsages = _rdbDataManager.GetAccountUsagesByAccount(accountTypeId, accountId, status, effectiveDate, isEffectiveInFuture);

                                AssertUsagesAreSimilar(sqlUsages, rdbUsages);
                            }, accountIdsToTest, UTUtilities.GetEnumListForTesting<VRAccountStatus?>(), effectiveDates, UTUtilities.GetNullableBoolListForTesting());
        }

        private void TestGetAccountUsageForSpecificPeriodByAccountIds(Guid accountTypeId, List<DateTime> fromTimesToTest, List<Guid> transactionTypeIdsToTest, List<List<string>> accountIdList)
        {
            UTUtilities.CallActionIteratively(
                            (transactionTypeId, fromTime, accountIdsLocal) =>
                            {
                                var sqlUsages = _sqlDataManager.GetAccountUsageForSpecificPeriodByAccountIds(accountTypeId, transactionTypeId, fromTime, accountIdsLocal);
                                var rdbUsages = _rdbDataManager.GetAccountUsageForSpecificPeriodByAccountIds(accountTypeId, transactionTypeId, fromTime, accountIdsLocal);

                                AssertUsagesAreSimilar(sqlUsages, rdbUsages);
                            }, transactionTypeIdsToTest, fromTimesToTest, accountIdList);
        }

        private void TestGetAccountUsageForBillingTransactions(Guid accountTypeId, List<DateTime> fromTimesToTest, List<DateTime?> toTimesToTest, List<List<string>> accountIdList, List<List<Guid>> transactionTypeIdList, List<DateTime?> effectiveDates)
        {
            UTUtilities.CallActionIteratively(
                            (transactionTypeIdsLocal, accountIdsLocal, fromTime, toTime, status, effectiveDate, isEffectiveInFuture) =>
                            {
                                var sqlUsages = _sqlDataManager.GetAccountUsageForBillingTransactions(accountTypeId, transactionTypeIdsLocal, accountIdsLocal, fromTime, toTime, status, effectiveDate, isEffectiveInFuture);
                                var rdbUsages = _rdbDataManager.GetAccountUsageForBillingTransactions(accountTypeId, transactionTypeIdsLocal, accountIdsLocal, fromTime, toTime, status, effectiveDate, isEffectiveInFuture);

                                AssertUsagesAreSimilar(sqlUsages, rdbUsages);
                            }, transactionTypeIdList, accountIdList, fromTimesToTest, toTimesToTest, UTUtilities.GetEnumListForTesting<VRAccountStatus?>(), effectiveDates, UTUtilities.GetNullableBoolListForTesting());
        }

        private void TestGetAccountUsageErrorData(Guid accountTypeId, List<DateTime> fromTimesToTest, List<Guid> transactionTypeIdsToTest, List<Guid> correctionProcessIds)
        {
            UTUtilities.CallActionIteratively(
                (fromTime, transactionTypeId, correctionProcessId) =>
                {
                    var sqlUsages = _sqlDataManager.GetAccountUsageErrorData(accountTypeId, transactionTypeId, correctionProcessId, fromTime);
                    var rdbUsages = _rdbDataManager.GetAccountUsageErrorData(accountTypeId, transactionTypeId, correctionProcessId, fromTime);

                    AssertUsagesAreSimilar(sqlUsages, rdbUsages);
                }, fromTimesToTest, transactionTypeIdsToTest, correctionProcessIds);
        }

        private void TestGetAccountsUsageInfoByPeriod(Guid accountTypeId, List<DateTime> fromTimesToTest, List<Guid> transactionTypeIdsToTest)
        {
            UTUtilities.CallActionIteratively(
                (fromTime, transactionTypeId) =>
                {
                    var sqlUsages = _sqlDataManager.GetAccountsUsageInfoByPeriod(accountTypeId, fromTime, transactionTypeId);
                    var rdbUsages = _rdbDataManager.GetAccountsUsageInfoByPeriod(accountTypeId, fromTime, transactionTypeId);

                    AssertUsagesInfoAreSimilar(sqlUsages, rdbUsages);
                }, fromTimesToTest, transactionTypeIdsToTest);
        }

        private List<AccountUsageInfo> InsertAccountUsages(Guid accountTypeId, out List<string> accountIds, out List<int> currencyIds,
            out List<Guid> transactionTypeIds, out List<DateTime> fromTimes, out List<DateTime> toTimes)
        {
            List<AccountUsageInfo> usageInfos = new List<AccountUsageInfo>();

            accountIds = new List<string> { "account1", "account 2", Guid.NewGuid().ToString() };
            currencyIds = new List<int> { 1, 2 };
            transactionTypeIds = new List<Guid> { Guid.NewGuid(), new Guid("AF7A2D82-1416-4833-8B6E-033474BC2D20"), Guid.NewGuid() };
            List<Decimal> amounts = new List<decimal> { 0, 3.2M };
            fromTimes = new List<DateTime> { DateTime.Now, DateTime.Today.AddDays(-10) };
            toTimes = new List<DateTime> { DateTime.Now, DateTime.Today.AddDays(10) };


            UTUtilities.CallActionIteratively(
                (accountId) =>
                {
                    UTUtilities.AssertObjectsAreSimilar(
                        _sqlLiveBalanceDataManager.TryAddLiveBalanceAndGet(accountId, accountTypeId, 0, 1, 0, null, null, VRAccountStatus.Active, false),
                        _rdbLiveBalanceDataManager.TryAddLiveBalanceAndGet(accountId, accountTypeId, 0, 1, 0, null, null, VRAccountStatus.Active, false));
                }, accountIds);

            UTUtilities.CallActionIteratively(
                (accountId, currencyId, transactionTypeId, fromTime, toTime, amount) =>
                {
                    AccountUsageInfo sqlResponse = _sqlDataManager.TryAddAccountUsageAndGet(accountTypeId, transactionTypeId, accountId, fromTime, toTime, currencyId, amount);
                    AccountUsageInfo rdbResponse = _rdbDataManager.TryAddAccountUsageAndGet(accountTypeId, transactionTypeId, accountId, fromTime, toTime, currencyId, amount);

                    UTUtilities.AssertObjectsAreSimilar(sqlResponse, rdbResponse);

                    usageInfos.Add(rdbResponse);
                }, accountIds, currencyIds, transactionTypeIds, fromTimes, toTimes, amounts);

            return usageInfos;
        }

        private void AssertUsageIsSimilar(AccountUsage sqlUsage, AccountUsage rdbUsage)
        {
            if (sqlUsage == null && rdbUsage == null)
                return;
            AssertUsagesAreSimilar(new List<AccountUsage> { sqlUsage }, new List<AccountUsage> { rdbUsage });
        }

        private void AssertUsagesAreSimilar(IEnumerable<AccountUsage> sqlUsages, IEnumerable<AccountUsage> rdbUsages)
        {
            UTUtilities.AssertObjectsAreSimilar(sqlUsages, rdbUsages);
        }

        private void AssertUsagesInfoAreSimilar(IEnumerable<AccountUsageInfo> sqlUsages, IEnumerable<AccountUsageInfo> rdbUsages)
        {
            UTUtilities.AssertObjectsAreSimilar(sqlUsages, rdbUsages);
        }

        private void AssertTablesAreSimilar()
        {
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, DBTABLE_NAME_ACCOUNTUSAGE);
        }

    }
}
