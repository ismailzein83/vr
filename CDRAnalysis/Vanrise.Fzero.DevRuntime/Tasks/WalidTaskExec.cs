using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    class WalidTaskExec : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Walid Exec Strategy Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            List<int> StrategyIDs = new List<int>();
            StrategyIDs.Add(3);
            BPClient bpClient = new BPClient();
            var input = new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    FromDate = DateTime.Parse("2015-03-14"),
                    ToDate = DateTime.Parse("2015-03-15"),
                    OverridePrevious=false,
                    StrategyIds = StrategyIDs
                }
            };
            bpClient.CreateNewProcess(input);
        }
    }
}
