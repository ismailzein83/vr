using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace Vanrise.Fzero.DevRuntime.Tasks
{
    public class AnthonyTask : ITask
    {
        public void Execute()
        {
            var runtimeServices = new List<RuntimeService>();
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();
        }
    }
}
