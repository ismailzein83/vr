using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    class FillDataWarehouse : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Walid Fill DWS started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            BPClient bpClient = new BPClient();
            var input = new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.FillDataWarehouseProcessInput
                {
                    FromDate = DateTime.Parse("2015-03-10"),
                    ToDate = DateTime.Parse("2015-03-20")
                }
            };
            bpClient.CreateNewProcess(input);
        }
    }
}
