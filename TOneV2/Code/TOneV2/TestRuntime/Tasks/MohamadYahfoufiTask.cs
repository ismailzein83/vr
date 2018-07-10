using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TestRuntime
{
    public class MohamadYahfoufiTask : ITask
    {
        public void Execute()
        {
            #region Runtime
            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            //SchedulerService schedulerService = new SchedulerService { Interval = new TimeSpan(0, 0, 1) };
            //runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();
            #endregion
        }
    }
}
