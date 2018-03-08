using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TOne.Runtime
{
    public class MainService
    {
        RuntimeHost _host;

        public void Start()
         {
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            BPRegulatorRuntimeService bpRegulatorService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            var runtimeServices = new List<RuntimeService> { queueActivationService, bpService, bpRegulatorService};

            _host = new RuntimeHost(runtimeServices);
            _host.Start();
        }

        public void Stop()
        {
            if (_host != null)
                _host.Stop();
        }
    }
}
