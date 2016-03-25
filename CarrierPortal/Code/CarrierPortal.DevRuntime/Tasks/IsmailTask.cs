using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Runtime;

namespace CarrierPortal.DevRuntime.Tasks
{
    public class IsmailTask : ITask
    {
        public void Execute()
        {
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            var runtimeServices = new List<RuntimeService>();
           
            runtimeServices.Add(bpService);

            runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
        }
    }
}
