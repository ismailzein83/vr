using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace XBooster.Runtime.Tasks
{
    class IsmailTask : ITask
    {
        public void Execute()
        {
            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService businessProcessService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };

            runtimeServices.Add(businessProcessService);
            runtimeServices.Add(bpRegulatorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
        }
    }
}
