using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    class CancelStrategyExecution : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Cancel Strategy Execution Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            long strategyExecutionId = 1;

            BPInstanceManager bpClient = new BPInstanceManager();
            var input = new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.CancelStrategyExecutionProcessInput
                {
                    StrategyExecutionId = strategyExecutionId
                }
            };
            bpClient.CreateNewProcess(input);
        }
    }
}
