using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    class MortadaTask : ITask
    {
        public void Execute()
        {
            System.Threading.ThreadPool.SetMaxThreads(10000, 10000);

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            BPRegulatorRuntimeService regulatorRuntimeService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            TransactionLockRuntimeService transactionLockRuntimeService = new Vanrise.Runtime.TransactionLockRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService SchedulerService = new Vanrise.Runtime.SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            //QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            //SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 5) };

            var runtimeServices = new List<RuntimeService>();
            //runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);
            runtimeServices.Add(regulatorRuntimeService);
            runtimeServices.Add(transactionLockRuntimeService);
            runtimeServices.Add(SchedulerService);

            //runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            //RunCompleteProductRouteBuild();

            //BPClient bpClient = new BPClient();
            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    InputArguments = new TOne.WhS.Routing.BP.Arguments.BuildRoutesByCodePrefixInput
            //    {
            //        CodePrefix = "91",
            //        EffectiveOn = DateTime.Now,
            //        IsFuture = false
            //    }
            //});


        }

        private static void RunCompleteProductRouteBuild()
        {
            BPInstanceManager bpClient = new BPInstanceManager();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new TOne.WhS.Routing.BP.Arguments.RPRoutingProcessInput
                {
                    EffectiveOn = DateTime.Now,
                    RoutingDatabaseType = TOne.WhS.Routing.Entities.RoutingDatabaseType.Current,
                    CodePrefixLength = 1,
                    IsFuture = false,
                    SaleZoneRange = 1000,
                    SupplierZoneRPOptionPolicies = new List<SupplierZoneToRPOptionPolicy>() { 
                        new SupplierZoneToRPOptionHighestRatePolicy() { ConfigId = 27 }, 
                        new SupplierZoneToRPOptionLowestRatePolicy() { ConfigId = 29 }, 
                        new SupplierZoneToRPOptionAverageRatePolicy() { ConfigId = 30 } },
                    RoutingProcessType = RoutingProcessType.RoutingProductRoute
                }
            });
        }
    }
}
