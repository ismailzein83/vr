﻿using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Linq;


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
            StrategyIds.Add(new StrategyManager().GetStrategies().FirstOrDefault().Id);
            BPClient bpClient = new BPClient();
            var input = new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    FromDate = DateTime.Parse("2015-03-10"),
                    ToDate = DateTime.Parse("2015-03-20"),
                    IncludeWhiteList =false,
                    StrategyIds = StrategyIds
                }
            };
            bpClient.CreateNewProcess(input);
        }
    }
}
