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

namespace Vanrise.Fzero.DevRuntime.Tasks
{
    public class IsmailTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Ismail Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Vanrise.Fzero.CDRImport.Business.CDRQueueFactory.GetImportedCDRQueue("DS1");
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                ProcessName = "CDRImportProcess",
                InputArguments = new Vanrise.Fzero.CDRImport.BP.Arguments.CDRImportProcessInput
                {
                }
            });

            Console.ReadKey();
        }
    }
}
