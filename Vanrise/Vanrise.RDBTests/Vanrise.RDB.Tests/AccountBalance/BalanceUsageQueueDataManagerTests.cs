using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.AccountBalance.Data;
using Vanrise.RDBTests.Common;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using System.Collections.Generic;

namespace Vanrise.RDB.Tests.AccountBalance
{
    [TestClass]
    public class BalanceUsageQueueDataManagerTests
    {
        internal const string DBTABLE_NAME_BALANCEUSAGEQUEUE = "BalanceUsageQueue";

        IBalanceUsageQueueDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IBalanceUsageQueueDataManager>();
        IBalanceUsageQueueDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IBalanceUsageQueueDataManager>();
        
        [TestMethod]
        public void TestInsertSelectBalanceUsageQueue()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, DBTABLE_NAME_BALANCEUSAGEQUEUE);

            Guid accountTypeId1 = Guid.NewGuid();
            TestInsertSelect(accountTypeId1);
            Guid accountTypeId2 = Guid.NewGuid();
            TestInsertSelect(accountTypeId2);
            Guid accountTypeId3 = Guid.NewGuid();
            TestInsertSelect(accountTypeId3);

            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, DBTABLE_NAME_BALANCEUSAGEQUEUE);
        }

        private void TestInsertSelect(Guid accountTypeId)
        {
            TestHasUsage(accountTypeId);
            TestLoadUsages(accountTypeId);
            TestInsertUpdateUsageQueues(accountTypeId);
            TestInsertCorrectUsageQueues(accountTypeId);
            TestHasUsage(accountTypeId);
            TestLoadUsages(accountTypeId);
            TestInsertUpdateUsageQueues(accountTypeId);
            TestInsertCorrectUsageQueues(accountTypeId);
            TestHasUsage(accountTypeId);
            TestLoadUsages(accountTypeId);
        }

        private void TestLoadUsages(Guid accountTypeId)
        {
            TestLoadUpdateBalances(accountTypeId);
            TestLoadCorrectBalances(accountTypeId);
        }

        private void TestLoadUpdateBalances(Guid accountTypeId)
        {
            List<BalanceUsageQueue<UpdateUsageBalancePayload>> sqlUpdateUsages = new List<BalanceUsageQueue<UpdateUsageBalancePayload>>();
            List<BalanceUsageQueue<UpdateUsageBalancePayload>> rdbUpdateUsages = new List<BalanceUsageQueue<UpdateUsageBalancePayload>>();

            _sqlDataManager.LoadUsageBalance<UpdateUsageBalancePayload>(accountTypeId, BalanceUsageQueueType.UpdateUsageBalance, (itm) => sqlUpdateUsages.Add(itm));
            _rdbDataManager.LoadUsageBalance<UpdateUsageBalancePayload>(accountTypeId, BalanceUsageQueueType.UpdateUsageBalance, (itm) => rdbUpdateUsages.Add(itm));

            UTUtilities.AssertObjectsAreSimilar(sqlUpdateUsages, rdbUpdateUsages);
        }

        private void TestLoadCorrectBalances(Guid accountTypeId)
        {
            List<BalanceUsageQueue<CorrectUsageBalancePayload>> sqlUpdateUsages = new List<BalanceUsageQueue<CorrectUsageBalancePayload>>();
            List<BalanceUsageQueue<CorrectUsageBalancePayload>> rdbUpdateUsages = new List<BalanceUsageQueue<CorrectUsageBalancePayload>>();

            _sqlDataManager.LoadUsageBalance<CorrectUsageBalancePayload>(accountTypeId, BalanceUsageQueueType.CorrectUsageBalance, (itm) => sqlUpdateUsages.Add(itm));
            _rdbDataManager.LoadUsageBalance<CorrectUsageBalancePayload>(accountTypeId, BalanceUsageQueueType.CorrectUsageBalance, (itm) => rdbUpdateUsages.Add(itm));

            UTUtilities.AssertObjectsAreSimilar(sqlUpdateUsages, rdbUpdateUsages);
        }

        private void TestHasUsage(Guid accountTypeId)
        {
            UTUtilities.AssertValuesAreEqual(_sqlDataManager.HasUsageBalanceData(accountTypeId), _rdbDataManager.HasUsageBalanceData(accountTypeId));
        }

        private void TestInsertUpdateUsageQueues(Guid accountTypeId)
        {
            var updateUsagePayload = new UpdateUsageBalancePayload
            {
                TransactionTypeId = Guid.NewGuid(),
                UpdateUsageBalanceItems = new System.Collections.Generic.List<UpdateUsageBalanceItem>
               {
                   new UpdateUsageBalanceItem
                   {
                       AccountId = "3254",
                       EffectiveOn = DateTime.Now
                   }
               }
            };
            TestInsertBalanceQueue(accountTypeId, BalanceUsageQueueType.UpdateUsageBalance, updateUsagePayload);
            for (int i = 0; i < 10; i++)
            {
                updateUsagePayload = updateUsagePayload.VRDeepCopy();
                updateUsagePayload.UpdateUsageBalanceItems.Add(new UpdateUsageBalanceItem { AccountId = Guid.NewGuid().ToString() });
                TestInsertBalanceQueue(accountTypeId, BalanceUsageQueueType.UpdateUsageBalance, updateUsagePayload);
            }
        }

        private void TestInsertCorrectUsageQueues(Guid accountTypeId)
        {
            var correctUsagePayload = new CorrectUsageBalancePayload
            { 
                CorrectionProcessId = Guid.NewGuid(),
                TransactionTypeId= Guid.NewGuid(),
                CorrectUsageBalanceItems = new List<CorrectUsageBalanceItem>
                {
                    new CorrectUsageBalanceItem
                    {
                        AccountId = Guid.NewGuid().ToString()
                    }
                }
            };
            TestInsertBalanceQueue(accountTypeId, BalanceUsageQueueType.CorrectUsageBalance, correctUsagePayload);
            for (int i = 0; i < 10; i++)
            {
                correctUsagePayload = correctUsagePayload.VRDeepCopy();
                correctUsagePayload.CorrectUsageBalanceItems.Add(new CorrectUsageBalanceItem { AccountId = Guid.NewGuid().ToString() });
                TestInsertBalanceQueue(accountTypeId, BalanceUsageQueueType.CorrectUsageBalance, correctUsagePayload);
            }
        }

        private void TestInsertBalanceQueue<T>(Guid accountTypeId, BalanceUsageQueueType queueType, T payload)
        {
            UTUtilities.AssertValuesAreEqual(_sqlDataManager.UpdateUsageBalance(accountTypeId, queueType, payload),
                            _rdbDataManager.UpdateUsageBalance(accountTypeId, queueType, payload));
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_ACCOUNTBALANCE, DBTABLE_NAME_BALANCEUSAGEQUEUE);
        }
    }
}
