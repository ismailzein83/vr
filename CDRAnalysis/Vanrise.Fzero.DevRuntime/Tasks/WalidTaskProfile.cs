using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    class WalidTaskProfile : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Walid Profiling Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            BPClient bpClient = new BPClient();
            var input = new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput
                {
                    FromDate = DateTime.Parse("2014-01-01"),
                    ToDate = DateTime.Parse("2014-01-02"),
                    Parameters = new Vanrise.Fzero.FraudAnalysis.Entities.NumberProfileParameters() { GapBetweenConsecutiveCalls = 10, MaxLowDurationCall = 10, GapBetweenFailedConsecutiveCalls = 10, MinimumCountofCallsinActiveHour = 3,
                        PeakHoursIds = new HashSet<int> { 1, 2, 3 } },
                    PeriodId = 1

                }
            };
            bpClient.CreateNewProcess(input);

            //Console.ReadKey();

        }
    }
}
