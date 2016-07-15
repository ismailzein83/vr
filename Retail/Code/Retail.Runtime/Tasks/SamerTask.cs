using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        }
    }
}
