using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace TestRuntime
{
    public class RabihTask : ITask
    {
        public void Execute()
        {

            //IServiceDataManager datamanager = BEDataManagerFactory.GetDataManager<IServiceDataManager>();


            //string result = datamanager.GetServicesDisplayList(15);
            //return;

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            Console.WriteLine("Hello from Rabih!");

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            runtimeServices.Add(bpService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            //RunCompleteRouteBuild();
            //RunCompleteProductRouteBuild();
            //RunPartialRouteBuild();
        }

        private static void RunCompleteProductRouteBuild()
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new TOne.WhS.Routing.BP.Arguments.RPRoutingProcessInput
                {
                    EffectiveOn = DateTime.Now,
                    RoutingDatabaseType = TOne.WhS.Routing.Entities.RoutingDatabaseType.Current,
                    CodePrefixLength = 1,
                    IsFuture = false,
                    SaleZoneRange = 1000,
                    SupplierZoneRPOptionPolicies = new List<SupplierZoneToRPOptionPolicy>() { new SupplierZoneToRPOptionHighestRatePolicy() { ConfigId = 27, IsDefault = true }, new SupplierZoneToRPOptionLowestRatePolicy() { ConfigId = 29 } },
                    RoutingProcessType = RoutingProcessType.RoutingProductRoute
                }
            });
        }

        private static void RunCompleteRouteBuild()
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new TOne.WhS.Routing.BP.Arguments.RoutingProcessInput
                {
                    CodePrefixLength = 2,
                    EffectiveTime = DateTime.Now,
                    RoutingDatabaseType = RoutingDatabaseType.Current,
                    RoutingProcessType = RoutingProcessType.CustomerRoute,
                    DivideProcessIntoSubProcesses = false
                }

            });
        }

        private static void RunPartialRouteBuild()
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new TOne.LCRProcess.Arguments.DifferentialRoutingProcessInput()

            });
        }
    }
}
