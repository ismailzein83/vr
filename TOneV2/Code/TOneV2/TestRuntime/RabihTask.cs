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

            RunCompleteProductRouteBuild();
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
                    SupplierZoneRPOptionPolicies = new List<SupplierZoneToRPOptionPolicy>() { new SupplierZoneToRPOptionHighestRatePolicy() { ConfigId = 1 }, new SupplierZoneToRPOptionLowestRatePolicy() { ConfigId = 2 }, new SupplierZoneToRPOptionAverageRatePolicy() { ConfigId = 3 } },
                    RoutingProcessType = RoutingProcessType.ProductRoute
                }
            });
        }

        private static void RunCompleteRouteBuild()
        {
            BPClient bpClient = new BPClient();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new TOne.LCRProcess.Arguments.RoutingProcessInput
                {
                    EffectiveTime = DateTime.Now,
                    IsFuture = false,
                    IsLcrOnly = false
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
