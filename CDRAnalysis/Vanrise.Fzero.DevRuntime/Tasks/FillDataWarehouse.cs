using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;


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

            BPInstanceManager bpClient = new BPInstanceManager();
            var input = new CreateProcessInput
            {
                InputArguments = new Vanrise.Fzero.FraudAnalysis.BP.Arguments.FillDataWarehouseProcessInput
                {
                    FromDate = DateTime.Parse("2015-03-10 00:00:00"),
                    ToDate = DateTime.Parse("2016-03-20 00:59:59"),
                    UserId=1
                }
            };
            bpClient.CreateNewProcess(input);
        }
    }
}
