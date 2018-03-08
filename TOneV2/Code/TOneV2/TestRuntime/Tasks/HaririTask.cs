using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRuntime;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TOne.WhS.Runtime.Tasks
{
    public class HaririTask : ITask
    {
        public void Execute()
        {
            var runtimeServices = new List<RuntimeService>();
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            BPRegulatorRuntimeService regulatorService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(regulatorService);

            //Test1_AddData();
            //Test2_AddData();
            //Test3_AddData();

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();
        }

        #region Test 1

        //private void Test1_AddData()
        //{
        //    var accountTypeId = new Guid("5488a9a2-0200-4895-b6ed-13f35c94d54c");
        //    string accountId = "183";

        //    Test1_AddBillingTransaction(accountTypeId, accountId);
        //    Test1_AddAccountUsages(accountTypeId, accountId);
        //    Test1_AddLiveBalance(accountTypeId, accountId);
        //}
        //private void Test1_AddBillingTransaction(Guid accountTypeId, string accountId)
        //{
        //    var billingTransaction = new BillingTransaction()
        //    {
        //        AccountTypeId = accountTypeId,
        //        AccountId = accountId,
        //        TransactionTypeId = new Guid("ACD5923A-8057-4C21-9E7B-EBC76B9CE1DF"),
        //        Amount = 1175,
        //        CurrencyId = 1,
        //        TransactionTime = new DateTime(2017, 5, 20),
        //        Settings = new BillingTransactionSettings()
        //        {
        //            UsageOverrides = new List<BillingTransactionUsageOverride>()
        //            {
        //                new BillingTransactionUsageOverride()
        //                {
        //                    TransactionTypeId = new Guid("ACD5923A-8057-4C21-9E7B-EBC76B9CE1DF"),
        //                    FromDate = new DateTime(2017, 5, 10),
        //                    ToDate = new DateTime(2017, 5, 20)
        //                }
        //            }
        //        },
        //        IsBalanceUpdated = false
        //    };
        //    new BillingTransactionManager().AddBillingTransaction(billingTransaction);
        //}
        //private void Test1_AddAccountUsages(Guid accountTypeId, string accountId)
        //{
        //    var accountUsageDataManager = AccountBalanceDataManagerFactory.GetDataManager<IAccountUsageDataManager>();

        //    var accountUsage1 = new AccountUsage()
        //    {
        //        AccountTypeId = accountTypeId,
        //        AccountId = accountId,
        //        TransactionTypeId = new Guid("ACD5923A-8057-4C21-9E7B-EBC76B9CE1DF"),
        //        UsageBalance = 500,
        //        CurrencyId = 1,
        //        PeriodStart = new DateTime(2017, 5, 1),
        //        PeriodEnd = new DateTime(2017, 5, 1)
        //    };

        //    accountUsageDataManager.TryAddAccountUsageAndGet
        //    (
        //        accountUsage1.AccountTypeId,
        //        accountUsage1.TransactionTypeId,
        //        accountUsage1.AccountId,
        //        accountUsage1.PeriodStart,
        //        accountUsage1.PeriodEnd,
        //        accountUsage1.CurrencyId,
        //        accountUsage1.UsageBalance
        //    );

        //    var accountUsage2 = new AccountUsage()
        //    {
        //        AccountTypeId = accountTypeId,
        //        AccountId = accountId,
        //        TransactionTypeId = new Guid("ACD5923A-8057-4C21-9E7B-EBC76B9CE1DF"),
        //        UsageBalance = 300,
        //        CurrencyId = 1,
        //        PeriodStart = new DateTime(2017, 5, 10),
        //        PeriodEnd = new DateTime(2017, 5, 10)
        //    };

        //    accountUsageDataManager.TryAddAccountUsageAndGet
        //    (
        //        accountUsage2.AccountTypeId,
        //        accountUsage2.TransactionTypeId,
        //        accountUsage2.AccountId,
        //        accountUsage2.PeriodStart,
        //        accountUsage2.PeriodEnd,
        //        accountUsage2.CurrencyId,
        //        accountUsage2.UsageBalance
        //    );

        //    var accountUsage3 = new AccountUsage()
        //    {
        //        AccountTypeId = accountTypeId,
        //        AccountId = accountId,
        //        TransactionTypeId = new Guid("ACD5923A-8057-4C21-9E7B-EBC76B9CE1DF"),
        //        UsageBalance = 700,
        //        CurrencyId = 1,
        //        PeriodStart = new DateTime(2017, 5, 20),
        //        PeriodEnd = new DateTime(2017, 5, 20)
        //    };

        //    accountUsageDataManager.TryAddAccountUsageAndGet
        //    (
        //        accountUsage3.AccountTypeId,
        //        accountUsage3.TransactionTypeId,
        //        accountUsage3.AccountId,
        //        accountUsage3.PeriodStart,
        //        accountUsage3.PeriodEnd,
        //        accountUsage3.CurrencyId,
        //        accountUsage3.UsageBalance
        //    );
        //}
        //private void Test1_AddLiveBalance(Guid accountTypeId, string accountId)
        //{
        //    var liveBalanceDataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
        //    liveBalanceDataManager.TryAddLiveBalanceAndGet(accountId, accountTypeId, 0, 1, -1500);
        //}

        #endregion

        #region Test 2

        private void Test2_AddData()
        {
            var accountTypeId = new Guid("5488a9a2-0200-4895-b6ed-13f35c94d54c");
            string accountId = "183";

            Test2_AddBalanceUsageQueues(accountTypeId, accountId);
        }
        private void Test2_AddBalanceUsageQueues(Guid accountTypeId, string accountId)
        {
            var usageBalanceManager = new Vanrise.AccountBalance.Business.UsageBalanceManager();

            usageBalanceManager.UpdateUsageBalance(accountTypeId, new UpdateUsageBalancePayload()
            {
                TransactionTypeId = new Guid("ACD5923A-8057-4C21-9E7B-EBC76B9CE1DF"),
                UpdateUsageBalanceItems = new List<UpdateUsageBalanceItem>()
                {
                    new UpdateUsageBalanceItem()
                    {
                        AccountId = accountId,
                        Value = 500,
                        CurrencyId = 1,
                        EffectiveOn = new DateTime(2017, 5, 10)
                    },
                    new UpdateUsageBalanceItem()
                    {
                        AccountId = accountId,
                        Value = 500,
                        CurrencyId = 1,
                        EffectiveOn = new DateTime(2017, 5, 20)
                    }
                }
            });
        }

        #endregion

        #region Test 3

        private void Test3_AddData()
        {
            var accountTypeId = new Guid("5488a9a2-0200-4895-b6ed-13f35c94d54c");
            string accountId = "183";

            Test3_AddBalanceUsageQueues(accountTypeId, accountId);
        }
        private void Test3_AddBalanceUsageQueues(Guid accountTypeId, string accountId)
        {
            var usageBalanceManager = new Vanrise.AccountBalance.Business.UsageBalanceManager();

            usageBalanceManager.CorrectUsageBalance(accountTypeId, new CorrectUsageBalancePayload()
            {
                CorrectionProcessId = new Guid("F8B47E97-0019-4924-9FC6-3F0B37080D1E"),
                TransactionTypeId = new Guid("ACD5923A-8057-4C21-9E7B-EBC76B9CE1DF"),
                PeriodDate = new DateTime(2017, 5, 10),
                IsLastBatch = true,
                CorrectUsageBalanceItems = new List<CorrectUsageBalanceItem>()
                {
                    new CorrectUsageBalanceItem()
                    {
                        AccountId = accountId,
                        Value = 700,
                        CurrencyId = 1
                    }
                }
            });
        }

        #endregion
    }
}
