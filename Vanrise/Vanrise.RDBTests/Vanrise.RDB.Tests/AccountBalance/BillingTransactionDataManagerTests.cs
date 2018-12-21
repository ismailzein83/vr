using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.AccountBalance.Data;
using Vanrise.RDBTests.Common;
using System.Collections.Generic;
using Vanrise.AccountBalance.Entities;
using Vanrise.Entities;
using System.Linq;

namespace Vanrise.RDB.Tests.AccountBalance
{
    [TestClass]
    public class BillingTransactionDataManagerTests
    {
        internal const string DBTABLE_NAME_BILLINGTRANSACTION = "BillingTransaction";

        IBillingTransactionDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
        IBillingTransactionDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();

        ILiveBalanceDataManager _rdbLiveBalanceDataManager = RDBDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
        ILiveBalanceDataManager _sqlLiveBalanceDataManager = SQLDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();

        [TestMethod]
        public void TestInsertSelectBillingTransactions()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, DBTABLE_NAME_BILLINGTRANSACTION);
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
            List<DateTime> transactionTimes;
            List<Guid> transactionTypeIds;
            var billingTransactions = InsertBillingTransactions(accountTypeId, out accountIds, out currencyIds,
                out transactionTimes, out transactionTypeIds);

            AssertTablesAreSimilar();
            TestGetBillingTransactionsByBalanceUpdated(accountTypeId, billingTransactions);

            UTUtilities.AssertValuesAreEqual(_sqlDataManager.HasBillingTransactionData(accountTypeId), _rdbDataManager.HasBillingTransactionData(accountTypeId));
            TestGetBillingTransactionById(accountTypeId, billingTransactions);

            TestGetBillingTransactions(accountTypeId, accountIds, transactionTypeIds);
            TestGetBillingTransactionsByAccountId(accountTypeId, accountIds);
            TestGetBillingTransactionsByTransactionTypes(accountTypeId, accountIds, transactionTypeIds, transactionTimes);
        }

        private List<BillingTransaction> InsertBillingTransactions(Guid accountTypeId, out List<string> accountIds, out List<int> currencyIds,
            out List<DateTime> transactionTimes, out List<Guid> transactionTypeIds)
        {
            List<BillingTransaction> billingTransactions = new List<BillingTransaction>();

            accountIds = new List<string> { "account1", "account 2", Guid.NewGuid().ToString() };
            currencyIds = new List<int> { 1, 2 };
            transactionTimes = new List<DateTime> { DateTime.Now, DateTime.Today.AddDays(-10) };
            transactionTypeIds = new List<Guid> { Guid.NewGuid(), new Guid("AF7A2D82-1416-4833-8B6E-033474BC2D20"), Guid.NewGuid() };
            List<Decimal> amounts = new List<decimal> { 2.3M, 3.2M };
            List<BillingTransactionSettings> settings = new List<BillingTransactionSettings> { null, new BillingTransactionSettings { Attachments = new Vanrise.GenericData.Entities.AttachmentFieldTypeEntityCollection { new Vanrise.GenericData.Entities.AttachmentFieldTypeEntity { FileId = 3 } } } };
            List<string> notes = new List<string> { null, "fdsfds" };
            List<string> sourceIds = new List<string> { null, "fsdfwewewewew" };

            UTUtilities.CallActionIteratively(
                (accountId) =>
                {
                    UTUtilities.AssertObjectsAreSimilar(
                        _sqlLiveBalanceDataManager.TryAddLiveBalanceAndGet(accountId, accountTypeId, 0, 1, 0, null, null, VRAccountStatus.Active, false),
                        _rdbLiveBalanceDataManager.TryAddLiveBalanceAndGet(accountId, accountTypeId, 0, 1, 0, null, null, VRAccountStatus.Active, false));
                }, accountIds);

            UTUtilities.CallActionIteratively(
                (accountId, currencyId, transactionTime, transactionTypeId, amount, setting, note, sourceId) =>
                {
                    var billingTransaction = new BillingTransaction
                    {
                        AccountTypeId = accountTypeId,
                        AccountId = accountId,
                        CurrencyId = currencyId,
                        TransactionTime = transactionTime,
                        TransactionTypeId = transactionTypeId,
                        Amount = amount,
                        Settings = setting,
                        Notes = note,
                        Reference = note,
                        SourceId = sourceId
                    };
                    long sqlId;
                    long rdbId;
                    UTUtilities.AssertValuesAreEqual(_sqlDataManager.Insert(billingTransaction, out sqlId), _rdbDataManager.Insert(billingTransaction, out rdbId));
                    UTUtilities.AssertValuesAreEqual(sqlId, rdbId);
                    billingTransaction.AccountBillingTransactionId = rdbId;
                    billingTransactions.Add(billingTransaction);
                }, accountIds, currencyIds, transactionTimes, transactionTypeIds, amounts, settings, notes, sourceIds);
            
            return billingTransactions;
        }

        private void TestGetBillingTransactionsByTransactionTypes(Guid accountTypeId, List<string> accountIds, List<Guid> transactionTypeIds, List<DateTime> transactionTimes)
        {
            List<List<BillingTransactionByTime>> billingTransactionByTimeList = new List<List<BillingTransactionByTime>>
            {
                new List<BillingTransactionByTime>(),
                new List<BillingTransactionByTime>
                {
                    new BillingTransactionByTime { AccountId = "dsfgsdgf", TransactionTime = DateTime.Now},
                    new BillingTransactionByTime { AccountId = accountIds[1], TransactionTime = DateTime.Now},
                    new BillingTransactionByTime { AccountId = accountIds[2], TransactionTime = transactionTimes[1]},
                    new BillingTransactionByTime { AccountId = "fsdeweew", TransactionTime = transactionTimes[1]}
                }
            };

            List<List<Guid>> transactionTypeIdList = new List<List<Guid>>
            {
                new List<Guid>{Guid.NewGuid(), transactionTypeIds[0]},
                transactionTypeIds
            };

            UTUtilities.CallActionIteratively(
                (billingTransactionByTime, transactionTypeIdLocal) =>
                {
                    var sqlResponse = _sqlDataManager.GetBillingTransactionsByTransactionTypes(accountTypeId, billingTransactionByTime, transactionTypeIdLocal);
                    var rdbResponse = _rdbDataManager.GetBillingTransactionsByTransactionTypes(accountTypeId, billingTransactionByTime, transactionTypeIdLocal);
                    UTUtilities.AssertObjectsAreSimilar(sqlResponse, rdbResponse);
                }, billingTransactionByTimeList, transactionTypeIdList);

        }

        private void TestGetBillingTransactionsByBalanceUpdated(Guid accountTypeId, List<BillingTransaction> billingTransactions)
        {
            List<BillingTransaction> sqlBillingTransactions = new List<BillingTransaction>();
            List<BillingTransaction> rdbBillingTransactions = new List<BillingTransaction>();

            _sqlDataManager.GetBillingTransactionsByBalanceUpdated(accountTypeId, (t) => sqlBillingTransactions.Add(t));
            _rdbDataManager.GetBillingTransactionsByBalanceUpdated(accountTypeId, (t) => rdbBillingTransactions.Add(t));

            AssertBillingTransactionsAreSimilar(sqlBillingTransactions, rdbBillingTransactions);

            string updateQuery = string.Format("UPDATE [VR_AccountBalance].BillingTransaction SET IsBalanceUpdated = 1 WHERE ID in ({0}, {1}, {2})", billingTransactions[0].AccountBillingTransactionId, billingTransactions[14].AccountBillingTransactionId, billingTransactions[231].AccountBillingTransactionId);
            UTUtilities.ExecuteDBNonQuery(Constants.CONNSTRING_NAME_CONFIG, updateQuery);

            updateQuery = string.Format("UPDATE [VR_AccountBalance].BillingTransaction SET IsBalanceUpdated = 1, IsDeleted = 1 WHERE ID in ({0}, {1}, {2})", billingTransactions[3].AccountBillingTransactionId, billingTransactions[123].AccountBillingTransactionId, billingTransactions[65].AccountBillingTransactionId);
            UTUtilities.ExecuteDBNonQuery(Constants.CONNSTRING_NAME_CONFIG, updateQuery);

            sqlBillingTransactions = new List<BillingTransaction>();
            rdbBillingTransactions = new List<BillingTransaction>();

            _sqlDataManager.GetBillingTransactionsByBalanceUpdated(accountTypeId, (t) => sqlBillingTransactions.Add(t));
            _rdbDataManager.GetBillingTransactionsByBalanceUpdated(accountTypeId, (t) => rdbBillingTransactions.Add(t));

            AssertBillingTransactionsAreSimilar(sqlBillingTransactions, rdbBillingTransactions);


            updateQuery = string.Format("UPDATE [VR_AccountBalance].BillingTransaction SET IsSubtractedFromBalance = 1 WHERE ID in ({0}, {1}, {2})", billingTransactions[10].AccountBillingTransactionId, billingTransactions[104].AccountBillingTransactionId, billingTransactions[124].AccountBillingTransactionId);
            UTUtilities.ExecuteDBNonQuery(Constants.CONNSTRING_NAME_CONFIG, updateQuery);

            sqlBillingTransactions = new List<BillingTransaction>();
            rdbBillingTransactions = new List<BillingTransaction>();

            _sqlDataManager.GetBillingTransactionsByBalanceUpdated(accountTypeId, (t) => sqlBillingTransactions.Add(t));
            _rdbDataManager.GetBillingTransactionsByBalanceUpdated(accountTypeId, (t) => rdbBillingTransactions.Add(t));

            AssertBillingTransactionsAreSimilar(sqlBillingTransactions, rdbBillingTransactions);
        }

        private void TestGetBillingTransactionsByAccountId(Guid accountTypeId, List<string> accountIds)
        {
            List<string> accountIdsToTest = new List<string> { null, "", "edrfpoihewrfikjhdsf", accountIds[0], accountIds[1] };
            List<DateTime?> effectiveDates = new List<DateTime?> { null, DateTime.Now, DateTime.Now.AddDays(-10) };
            UTUtilities.CallActionIteratively(
                (accountId, status, effectiveDate, isEffectiveInFuture) =>
                {
                    var sqlBillingTransactions = _sqlDataManager.GetBillingTransactionsByAccountId(accountTypeId, accountId, status, effectiveDate, isEffectiveInFuture);
                    var rdbBillingTransactions = _rdbDataManager.GetBillingTransactionsByAccountId(accountTypeId, accountId, status, effectiveDate, isEffectiveInFuture);
                    AssertBillingTransactionsAreSimilar(sqlBillingTransactions, rdbBillingTransactions);
                }, accountIdsToTest, UTUtilities.GetEnumListForTesting<VRAccountStatus?>(), effectiveDates, UTUtilities.GetNullableBoolListForTesting());

            UTUtilities.CallActionIteratively(
                (accountId) =>
                {
                    var sqlBillingTransaction = _sqlDataManager.GetLastBillingTransaction(accountTypeId, accountId);
                    var rdbBillingTransaction = _rdbDataManager.GetLastBillingTransaction(accountTypeId, accountId);
                    AssertBillingTransactionIsSimilar(sqlBillingTransaction, rdbBillingTransaction);
                }, accountIdsToTest);
        }

        private void TestGetBillingTransactions(Guid accountTypeId, List<string> accountIds, List<Guid> transactionTypeIds)
        {
            List<List<Guid>> accountTypeIdList = new List<List<Guid>>
            {
                null,
                new List<Guid>(),
                new List<Guid>{accountTypeId},
                new List<Guid>{Guid.NewGuid(), accountTypeId}
            };
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

            List<DateTime> fromDates = new List<DateTime> { DateTime.Now, DateTime.Now.AddDays(-100) };
            List<DateTime?> toDates = new List<DateTime?> { null, DateTime.Now, DateTime.Now.AddDays(100) };
            List<DateTime?> effectiveDates = new List<DateTime?> { null, DateTime.Now, DateTime.Now.AddDays(-100) };

            UTUtilities.CallActionIteratively(
                (accountTypeIds, accountIdsLocal, transactionTypeIdsLocal, fromDate, toDate) =>
                {
                    var sqlBillingTransactions = _sqlDataManager.GetBillingTransactions(accountTypeIds, accountIdsLocal, transactionTypeIdsLocal, fromDate, toDate);
                    var rdbBillingTransactions = _rdbDataManager.GetBillingTransactions(accountTypeIds, accountIdsLocal, transactionTypeIdsLocal, fromDate, toDate);
                    AssertBillingTransactionsAreSimilar(sqlBillingTransactions, rdbBillingTransactions);
                }, accountTypeIdList , accountIdList, transactionTypeIdList, fromDates, toDates);

            UTUtilities.CallActionIteratively(
                (accountIdsLocal, transactionTypeIdsLocal) =>
                {
                    var sqlBillingTransactions = _sqlDataManager.GetBillingTransactionsByAccountIds(accountTypeId, transactionTypeIdsLocal, accountIdsLocal);
                    var rdbBillingTransactions = _rdbDataManager.GetBillingTransactionsByAccountIds(accountTypeId, transactionTypeIdsLocal, accountIdsLocal);
                    UTUtilities.AssertObjectsAreSimilar(sqlBillingTransactions, rdbBillingTransactions);
                }, accountIdList, transactionTypeIdList);

            UTUtilities.CallActionIteratively(
               (transactionTypeIdsLocal) =>
               {
                   var sqlBillingTransactions = _sqlDataManager.GetBillingTransactionsForSynchronizerProcess(transactionTypeIdsLocal != null ? transactionTypeIdsLocal.ToList() : null, accountTypeId);
                   var rdbBillingTransactions = _rdbDataManager.GetBillingTransactionsForSynchronizerProcess(transactionTypeIdsLocal != null ? transactionTypeIdsLocal.ToList() : null, accountTypeId);
                   AssertBillingTransactionsAreSimilar(sqlBillingTransactions, rdbBillingTransactions);
               }, transactionTypeIdList);

            UTUtilities.CallActionIteratively(
                (accountIdsLocal, transactionTypeIdsLocal, fromDate, toDate, status, effectiveDate, isEffectiveInFuture) =>
                {
                    var query = new BillingTransactionQuery
                    {
                        AccountTypeId = accountTypeId,
                        AccountsIds = accountIdsLocal,
                        TransactionTypeIds = transactionTypeIdsLocal,
                        FromTime = fromDate,
                        ToTime = toDate,
                        Status = status,
                        EffectiveDate = effectiveDate,
                        IsEffectiveInFuture = isEffectiveInFuture
                    };

                    var sqlResponse = _sqlDataManager.GetFilteredBillingTransactions(query);
                    var rdbResponse = _rdbDataManager.GetFilteredBillingTransactions(query);

                    AssertBillingTransactionsAreSimilar(sqlResponse, rdbResponse);
                }, accountIdList, transactionTypeIdList, fromDates, toDates, UTUtilities.GetEnumListForTesting<VRAccountStatus?>(), effectiveDates, UTUtilities.GetNullableBoolListForTesting());
        }

        private void TestGetBillingTransactionById(Guid accountTypeId, List<BillingTransaction> billingTransactions)
        {
            List<long> ids = new List<long> { 2143324, 3333, billingTransactions[0].AccountBillingTransactionId, billingTransactions[134].AccountBillingTransactionId };
            UTUtilities.CallActionIteratively(
                (billingTransactionId) =>
                {
                    var sqlBillingTransaction = _sqlDataManager.GetBillingTransactionById(billingTransactionId);
                    var rdbBillingTransaction = _rdbDataManager.GetBillingTransactionById(billingTransactionId);
                    AssertBillingTransactionIsSimilar(sqlBillingTransaction, rdbBillingTransaction);
                }, ids);
        }

        private void AssertBillingTransactionIsSimilar(BillingTransaction sqlBillingTransaction, BillingTransaction rdbBillingTransaction)
        {
            if (sqlBillingTransaction == null && rdbBillingTransaction == null)
                return;
            AssertBillingTransactionsAreSimilar(new List<BillingTransaction> { sqlBillingTransaction }, new List<BillingTransaction> { rdbBillingTransaction });
        }

        private void AssertBillingTransactionsAreSimilar(IEnumerable<BillingTransaction> sqlBillingTransactions, IEnumerable<BillingTransaction> rdbBillingTransactions)
        {
            UTUtilities.AssertObjectsAreSimilar(sqlBillingTransactions, rdbBillingTransactions);
        }

        private void AssertTablesAreSimilar()
        {
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, DBTABLE_NAME_BILLINGTRANSACTION);
        }
    }
}

