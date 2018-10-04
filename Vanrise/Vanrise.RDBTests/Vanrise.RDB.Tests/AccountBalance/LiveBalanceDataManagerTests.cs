using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.AccountBalance.Data;
using Vanrise.RDBTests.Common;
using Vanrise.Notification.Entities;
using Vanrise.AccountBalance.Entities;
using System.Collections.Generic;

namespace Vanrise.RDB.Tests.AccountBalance
{
    [TestClass]
    public class LiveBalanceDataManagerTests
    {
        ILiveBalanceDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
        ILiveBalanceDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();

        [TestMethod]
        public void AddAndSelectLiveBalance()
        {
            Guid sqlAccountTypeId = Guid.NewGuid();
            Guid rdbAccountTypeId = Guid.NewGuid();

            string accountId = "account1";

            var sqlLiveBalance = _sqlDataManager.TryAddLiveBalanceAndGet(accountId, sqlAccountTypeId, 0, 1, 0, null, null, Entities.VRAccountStatus.Active, false);
            var rdbLiveBalance = _rdbDataManager.TryAddLiveBalanceAndGet(accountId, rdbAccountTypeId, 0, 1, 0, null, null, Entities.VRAccountStatus.Active, false);

            long sqlLiveBalanceId = sqlLiveBalance.LiveBalanceId;
            long rdbLiveBalanceId = rdbLiveBalance.LiveBalanceId;

            sqlLiveBalance.LiveBalanceId = default(long);
            rdbLiveBalance.LiveBalanceId = default(long);
            UTAssert.ObjectsAreSimilar(sqlLiveBalance, rdbLiveBalance);

            sqlLiveBalance = _sqlDataManager.TryAddLiveBalanceAndGet(accountId, sqlAccountTypeId, 0, 1, 0, null, null, Entities.VRAccountStatus.Active, false);
            rdbLiveBalance = _rdbDataManager.TryAddLiveBalanceAndGet(accountId, rdbAccountTypeId, 0, 1, 0, null, null, Entities.VRAccountStatus.Active, false);
            sqlLiveBalance.LiveBalanceId = default(long);
            rdbLiveBalance.LiveBalanceId = default(long);
            UTAssert.ObjectsAreSimilar(sqlLiveBalance, rdbLiveBalance);

            AssertLiveBalancesAreSimilar(sqlAccountTypeId, rdbAccountTypeId, accountId);
            TestUpdateAccountStatus(sqlAccountTypeId, rdbAccountTypeId, accountId);
            TestUpdateAccountEffectiveDate(sqlAccountTypeId, rdbAccountTypeId, accountId);

            TestUpdateAccountRuleInfo(sqlAccountTypeId, rdbAccountTypeId, accountId);

            TestUpdateAccountLastAlertInfo(sqlAccountTypeId, rdbAccountTypeId, accountId);

            TestGetLiveBalances(sqlAccountTypeId, rdbAccountTypeId);

            _sqlDataManager.TryAddLiveBalanceAndGet(accountId + " 2", sqlAccountTypeId, 0, 1, 0, null, null, Entities.VRAccountStatus.Active, false);
            _rdbDataManager.TryAddLiveBalanceAndGet(accountId + " 2", rdbAccountTypeId, 0, 1, 0, null, null, Entities.VRAccountStatus.Active, false);

            DateTime? newBED = DateTime.Now;
            _sqlDataManager.TryAddLiveBalanceAndGet(accountId + " 3", sqlAccountTypeId, 0, 1, 0, newBED, null, Entities.VRAccountStatus.Active, false);
            _rdbDataManager.TryAddLiveBalanceAndGet(accountId + " 3", rdbAccountTypeId, 0, 1, 0, newBED, null, Entities.VRAccountStatus.Active, false);

            _sqlDataManager.TryAddLiveBalanceAndGet(accountId + " 4", sqlAccountTypeId, 0, 1, 0, null, null, Entities.VRAccountStatus.InActive, false);
            _rdbDataManager.TryAddLiveBalanceAndGet(accountId + " 4", rdbAccountTypeId, 0, 1, 0, null, null, Entities.VRAccountStatus.InActive, false);

            TestGetLiveBalances(sqlAccountTypeId, rdbAccountTypeId);

        }

        private void TestGetLiveBalances(Guid sqlAccountTypeId, Guid rdbAccountTypeId)
        {
            var sqlResponse = new List<LiveBalance>();
            var rdbResponse = new List<LiveBalance>();

            _sqlDataManager.GetLiveBalanceAccounts(sqlAccountTypeId, (bal) =>
            {
                bal.AccountTypeId = default(Guid);
                sqlResponse.Add(bal);
            });
            _rdbDataManager.GetLiveBalanceAccounts(sqlAccountTypeId, (bal) =>
            {
                bal.AccountTypeId = default(Guid);
                rdbResponse.Add(bal);
            });

            UTAssert.ObjectsAreSimilar(sqlResponse, rdbResponse);

            var sqlResponse2 = _sqlDataManager.GetLiveBalanceAccountsInfo(sqlAccountTypeId);
            var rdbResponse2 = _rdbDataManager.GetLiveBalanceAccountsInfo(rdbAccountTypeId);
            if(sqlResponse2 != null)
            {
                foreach(var r in sqlResponse2)
                {
                    r.LiveBalanceId = default(long);
                }
            }
            if (rdbResponse2 != null)
            {
                foreach (var r in rdbResponse2)
                {
                    r.LiveBalanceId = default(long);
                }
            }

            UTAssert.ObjectsAreSimilar(sqlResponse2, rdbResponse2);
        }

        private void TestUpdateAccountStatus(Guid sqlAccountTypeId, Guid rdbAccountTypeId, string accountId)
        {
            _sqlDataManager.TryUpdateLiveBalanceStatus(accountId, sqlAccountTypeId, Entities.VRAccountStatus.InActive, false);
            UTAssert.ObjectsAreEqual(true, _rdbDataManager.TryUpdateLiveBalanceStatus(accountId, rdbAccountTypeId, Entities.VRAccountStatus.InActive, false));

            AssertLiveBalancesAreSimilar(sqlAccountTypeId, rdbAccountTypeId, accountId);

            _sqlDataManager.TryUpdateLiveBalanceStatus(accountId, sqlAccountTypeId, Entities.VRAccountStatus.Active, false);
            UTAssert.ObjectsAreEqual(true, _rdbDataManager.TryUpdateLiveBalanceStatus(accountId, rdbAccountTypeId, Entities.VRAccountStatus.Active, false));

            AssertLiveBalancesAreSimilar(sqlAccountTypeId, rdbAccountTypeId, accountId);
        }

        private void TestUpdateAccountEffectiveDate(Guid sqlAccountTypeId, Guid rdbAccountTypeId, string accountId)
        {
            DateTime? newBED = DateTime.Today.AddMonths(-5);
            DateTime? newEED = DateTime.Today.AddMonths(5);
            _sqlDataManager.TryUpdateLiveBalanceEffectiveDate(accountId, sqlAccountTypeId, newBED, newEED);
            UTAssert.ObjectsAreEqual(true, _rdbDataManager.TryUpdateLiveBalanceEffectiveDate(accountId, rdbAccountTypeId, newBED, newEED));

            AssertLiveBalancesAreSimilar(sqlAccountTypeId, rdbAccountTypeId, accountId);

            _sqlDataManager.TryUpdateLiveBalanceEffectiveDate(accountId, sqlAccountTypeId, null, null);
            UTAssert.ObjectsAreEqual(true, _rdbDataManager.TryUpdateLiveBalanceEffectiveDate(accountId, rdbAccountTypeId, null, null));

            AssertLiveBalancesAreSimilar(sqlAccountTypeId, rdbAccountTypeId, accountId);
        }

        private void TestUpdateAccountRuleInfo(Guid sqlAccountTypeId, Guid rdbAccountTypeId, string accountId)
        {
            _sqlDataManager.UpdateBalanceRuleInfos(new System.Collections.Generic.List<Vanrise.AccountBalance.Entities.LiveBalanceNextThresholdUpdateEntity>
            {
                new Vanrise.AccountBalance.Entities.LiveBalanceNextThresholdUpdateEntity
                {
                     AccountTypeId = sqlAccountTypeId,
                     AccountId = accountId,
                     AlertRuleId = 3,
                     NextAlertThreshold =45.44M
                }
            });

            _rdbDataManager.UpdateBalanceRuleInfos(new System.Collections.Generic.List<Vanrise.AccountBalance.Entities.LiveBalanceNextThresholdUpdateEntity>
            {
                new Vanrise.AccountBalance.Entities.LiveBalanceNextThresholdUpdateEntity
                {
                     AccountTypeId = rdbAccountTypeId,
                     AccountId = accountId,
                     AlertRuleId = 3,
                     NextAlertThreshold =45.44M
                }
            });

            AssertLiveBalancesAreSimilar(sqlAccountTypeId, rdbAccountTypeId, accountId);
        }

        private void TestUpdateAccountLastAlertInfo(Guid sqlAccountTypeId, Guid rdbAccountTypeId, string accountId)
        {
            _sqlDataManager.UpdateBalanceLastAlertInfos(new System.Collections.Generic.List<Vanrise.AccountBalance.Entities.LiveBalanceLastThresholdUpdateEntity>
            {
                new Vanrise.AccountBalance.Entities.LiveBalanceLastThresholdUpdateEntity
                {
                    AccountTypeId = sqlAccountTypeId,
                    AccountId = accountId,
                    LastExecutedActionThreshold = 65.5435M,
                    ActiveAlertsInfo =new VRBalanceActiveAlertInfo
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
                    }
                }
            });

            _rdbDataManager.UpdateBalanceLastAlertInfos(new System.Collections.Generic.List<Vanrise.AccountBalance.Entities.LiveBalanceLastThresholdUpdateEntity>
            {
                new Vanrise.AccountBalance.Entities.LiveBalanceLastThresholdUpdateEntity
                {
                    AccountTypeId = rdbAccountTypeId,
                    AccountId = accountId,
                    LastExecutedActionThreshold = 65.5435M,
                    ActiveAlertsInfo =new VRBalanceActiveAlertInfo
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
                    }
                }
            });

            AssertLiveBalancesAreSimilar(sqlAccountTypeId, rdbAccountTypeId, accountId);
        }

        private void AssertLiveBalancesAreSimilar(Guid sqlAccountTypeId, Guid rdbAccountTypeId, string accountId)
        {
            var sqlLiveBalance = _sqlDataManager.GetLiveBalance(sqlAccountTypeId, accountId);
            var rdbLiveBalance = _rdbDataManager.GetLiveBalance(rdbAccountTypeId, accountId);

            UTAssert.NotNullObject(sqlLiveBalance);
            UTAssert.NotNullObject(rdbLiveBalance);

            rdbLiveBalance.AccountTypeId = sqlLiveBalance.AccountTypeId;

            UTAssert.ObjectsAreSimilar(sqlLiveBalance, rdbLiveBalance);
        }
    }
}
