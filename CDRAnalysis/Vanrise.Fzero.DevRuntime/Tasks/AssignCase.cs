using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    class AssignCase : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Walid Assign Case Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            BPInstanceManager bpClient = new BPInstanceManager();
            var input = new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.AssignStrategyCasesProcessInput
                {
                   UserId=1
                }
            };
            bpClient.CreateNewProcess(input);
        }
    }
}
