using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace TestRuntime
{
    public class ZeinabTask : ITask
    {
        public void Execute()
        {
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new TOne.LCRProcess.Arguments.RoutingProcessInput
                {
                    EffectiveTime = DateTime.Now,
                    IsFuture = false
                }
            });
        }
    }
}
