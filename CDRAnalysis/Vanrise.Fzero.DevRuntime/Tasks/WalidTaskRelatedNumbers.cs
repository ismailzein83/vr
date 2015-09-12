using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    class WalidTaskRelatedNumbers : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Walid Find Related Numbers Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            BPClient bpClient = new BPClient();
            var input = new CreateProcessInput
            {
                //InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.f
                //{

                //}
            };
            bpClient.CreateNewProcess(input);
        }
    }
}
