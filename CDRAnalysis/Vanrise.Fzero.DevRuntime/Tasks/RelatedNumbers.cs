using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    class RelatedNumbers : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Walid Find Related Numbers Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            BPInstanceManager bpClient = new BPInstanceManager();
            var input = new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.FindRelatedNumbersProcessInput
                {
                    FromDate = DateTime.Parse("2016-03-14 00:00:00"),
                    ToDate = DateTime.Parse("2016-03-15 00:00:00"),
                }
            };
            bpClient.CreateNewProcess(input);
        }
    }
}
