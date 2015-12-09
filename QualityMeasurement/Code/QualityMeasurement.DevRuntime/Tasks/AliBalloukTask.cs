using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;

namespace QualityMeasurement.DevRuntime.Tasks
{
    public class AliBalloukTask : ITask
    {
        public void Execute()
        {
            //BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            // QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            //runtimeServices.Add(queueActivationService);

            //runtimeServices.Add(bpService);

            runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
        }
    }
}
