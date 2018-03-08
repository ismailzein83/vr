using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    public class ABaghdadiTask : ITask
    {

        public void Execute()
        {
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new Vanrise.Queueing.QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<Vanrise.Runtime.Entities.RuntimeService>();
            runtimeServices.Add(queueActivationService);
            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

        }

       
    }
}
