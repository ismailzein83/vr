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
            var runtimeServices = new List<RuntimeService>();
            TransactionLockRuntimeService transactionLockRuntimeService = new TransactionLockRuntimeService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(transactionLockRuntimeService);
            BPRegulatorRuntimeService bpRegulatorService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorService);
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            
            runtimeServices.Add(bpService);

        }
    }
}
