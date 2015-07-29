using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace Vanrise.Fzero.DevRuntime.Tasks
{
    public class IsmailTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Ismail Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            //QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            //runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput
                {
                    FromDate = DateTime.Parse("2014-01-01"),
                    ToDate = DateTime.Parse("2015-01-01"),
                    StrategyIds = new List<int> { 22, 23, 26 },// 27, 28, 29 },//3, 13, 14, 15, 16 },//hourly
                    //StrategyIds = new List<int> { 2 , 4, 5 , 6, 7, 8, 9, 10, 11, 12 },//daily
                    PeriodId = 1
                }
            });

            //Console.ReadKey();
        }
    }
}
