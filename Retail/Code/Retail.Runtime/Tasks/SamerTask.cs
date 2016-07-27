using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Runtime;
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


            UsageBalanceManager manager = new UsageBalanceManager();


            UsageBalanceUpdate usageBalanceUpdate1 = new UsageBalanceUpdate
            {
                AccountId = 1,
                CurrencyId = 1,
                Value = 10
            };
            UsageBalanceUpdate usageBalanceUpdate2 = new UsageBalanceUpdate
            {
                AccountId = 2,
                CurrencyId = 1,
                Value = 20
            };
            UsageBalanceUpdate usageBalanceUpdate3 = new UsageBalanceUpdate
            {
                AccountId = 1,
                CurrencyId = 1,
                Value = 30
            };
            UsageBalanceUpdate usageBalanceUpdate4 = new UsageBalanceUpdate
            {
                AccountId = 2,
                CurrencyId = 1,
                Value = 40
            };

            BalanceUsageDetail BalanceUsageDetail = new BalanceUsageDetail
            {
                UsageBalanceUpdates = new List<UsageBalanceUpdate>() { usageBalanceUpdate1, usageBalanceUpdate2, usageBalanceUpdate3, usageBalanceUpdate4 }
            };
            manager.UpdateUsageBalance(BalanceUsageDetail);
        }
    }
}
