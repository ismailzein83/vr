using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace CarrierPortal.DevRuntime.Tasks
{
    public class MHaririTask : ITask
    {
        public void Execute()
        {
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
        }
    }
}
