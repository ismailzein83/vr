using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    public class AliBalloukTask : ITask
    {
        public void Execute()
        {
            var runtimeServices = new List<RuntimeService>();

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            //SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            //runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();
        }
    }
}
