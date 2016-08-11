using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.AccountBalance.MainExtensions.BalancePeriod;
using Vanrise.BusinessProcess;
using Vanrise.Runtime;

namespace Retail.Runtime.Tasks
{
    public class SamerTask : ITask
    {
        public void Execute()
        {
            System.Threading.ThreadPool.SetMaxThreads(10000, 10000);

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(bpService);



            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();


            //AccountBalanceConfig AccountBalanceConfig = new Vanrise.AccountBalance.Entities.AccountBalanceConfig
            //{
            //    AccountBusinessEntityDefinitionId = -2001,
            //    UsageTransactionTypeId = Guid.Parse("007869D9-6DC2-4F56-88A4-18C8C442E49E"),
            //    BalancePeriod = new MonthlyBalancePeriodSettings
            //    {
            //        ConfigId = 1,
            //        DayOfMonth = 1
            //    },

            //};
            //var serialized = Vanrise.Common.Serializer.Serialize(AccountBalanceConfig);



            UsageBalanceManager manager = new UsageBalanceManager();


            UsageBalanceUpdate usageBalanceUpdate1 = new UsageBalanceUpdate
            {
                AccountId = 171,
                CurrencyId = 1,
                Value = 1000,
                EffectiveOn = DateTime.MinValue
            };
            UsageBalanceUpdate usageBalanceUpdate2 = new UsageBalanceUpdate
            {
                AccountId = 171,
                CurrencyId = 1,
                Value = 20,
                EffectiveOn = DateTime.MinValue
            };
            UsageBalanceUpdate usageBalanceUpdate3 = new UsageBalanceUpdate
            {
                AccountId = 171,
                CurrencyId = 1,
                Value = 1000,
                EffectiveOn = DateTime.MinValue

            };
            UsageBalanceUpdate usageBalanceUpdate4 = new UsageBalanceUpdate
            {
                AccountId = 171,
                CurrencyId = 1,
                Value = 1000,
                EffectiveOn = DateTime.MinValue
            };

            BalanceUsageDetail BalanceUsageDetail = new BalanceUsageDetail
            {
                UsageBalanceUpdates = new List<UsageBalanceUpdate>() { usageBalanceUpdate1, usageBalanceUpdate2, usageBalanceUpdate3, usageBalanceUpdate4 }
            };
            manager.UpdateUsageBalance(BalanceUsageDetail);
        }
    }
}
