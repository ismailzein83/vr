using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    public class AnthonyTask : ITask
    {
        public void Execute()
        {
            var runtimeServices = new List<RuntimeService>();
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            TransactionLockRuntimeService transactionLockRuntimeService = new TransactionLockRuntimeService() { Interval = new TimeSpan(0, 0, 1) };
           
            //runtimeServices.Add(queueActivationService);
            //runtimeServices.Add(schedulerService);
            //runtimeServices.Add(dsRuntimeService);
            //runtimeServices.Add(transactionLockRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();
        }
    }
}
