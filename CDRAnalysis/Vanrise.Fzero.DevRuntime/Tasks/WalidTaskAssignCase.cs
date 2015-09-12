using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    class WalidTaskAssignCase : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Walid Assign Case Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            BPClient bpClient = new BPClient();
            var input = new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.AssignStrategyCasesProcessInput
                {
                  
                }
            };
            bpClient.CreateNewProcess(input);
        }
    }
}
