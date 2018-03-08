using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;


namespace Vanrise.Fzero.DevRuntime.Tasks
{
    class Profiling : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Walid Profiling Task started");
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            BPInstanceManager bpClient = new BPInstanceManager();
            var input = new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput
                {
                    FromDate = DateTime.Parse("2016-03-14"),
                    ToDate = DateTime.Parse("2016-03-15"),
                    Parameters = new Vanrise.Fzero.FraudAnalysis.Entities.NumberProfileParameters()
                    {
                        GapBetweenConsecutiveCalls = 10,
                        MaxLowDurationCall = 10,
                        GapBetweenFailedConsecutiveCalls = 10,
                        MinimumCountofCallsinActiveHour = 3,
                        PeakHoursIds = new HashSet<int> { 1, 2, 3 }
                    },
                    PeriodId = 1,
                    IncludeWhiteList = false

                }
            };
            bpClient.CreateNewProcess(input);

            //Console.ReadKey();

        }
    }
}
