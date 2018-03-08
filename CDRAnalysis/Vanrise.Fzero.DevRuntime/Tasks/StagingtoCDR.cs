using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    class StagingtoCDR : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Staging to CDR Process started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            BPInstanceManager bpClient = new BPInstanceManager();
            var input = new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.CDRImport.BP.Arguments.StagingtoCDRProcessInput
                {
                    FromDate = DateTime.Parse("1980-01-01"),
                    ToDate = DateTime.Parse("2020-01-02")
                }
            };
            bpClient.CreateNewProcess(input);
        }
    }
}
