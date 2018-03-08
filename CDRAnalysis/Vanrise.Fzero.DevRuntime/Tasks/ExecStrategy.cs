using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Linq;
using Vanrise.BusinessProcess.Business;
using Vanrise.Runtime.Entities;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    class ExecStrategy : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Walid Exec Strategy Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            List<int> StrategyIds = new List<int>();
            StrategyIds.Add(6);
            BPInstanceManager bpClient = new BPInstanceManager();
            var input = new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    FromDate = DateTime.Parse("2016-03-14 00:00:00"),
                    ToDate = DateTime.Parse("2016-03-15 00:00:00"),
                    IncludeWhiteList = false,
                    StrategyIds = StrategyIds,
                    UserId = 1
                }
            };
            bpClient.CreateNewProcess(input);
        }
    }
}
