using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.TelesRadius;
using TOne.WhS.RouteSync.TelesRadius.SQL;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Business.Extensions;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
//using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace TestRuntime
{
    public class RabihTask : ITask
    {
        public void Execute()
        {

            RadiusDataManagerConfig settings = new RadiusDataManagerConfig
            {
                RadiusDataManager = new RadiusSQLDataManager()
            };

            RadiusSQLDataManager dataManager = new RadiusSQLDataManager();

            RouteSyncTechnicalSettings sett = new RouteSyncTechnicalSettings() { SwitchInfoGetter = new RouteSyncSwitchGetter() };
            var ser = Serializer.Serialize(settings);

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            Console.WriteLine("Hello from Rabih!");

            var runtimeServices = new List<RuntimeService>();
            BusinessProcessService bpservice = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpservice);

            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            TransactionLockRuntimeService transactionLockRuntimeService = new TransactionLockRuntimeService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(transactionLockRuntimeService);
            runtimeServices.Add(queueActivationService);
            runtimeServices.Add(schedulerService);
            runtimeServices.Add(dsRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            RunRouteSync();
            //RunCompleteRouteBuild();
            //RunCompleteProductRouteBuild();
            //RunPartialRouteBuild();
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
                    SupplierZoneRPOptionPolicies = new List<SupplierZoneToRPOptionPolicy>() { new SupplierZoneToRPOptionHighestRatePolicy() { ConfigId = 27, IsDefault = true }, new SupplierZoneToRPOptionLowestRatePolicy() { ConfigId = 29 } },
                    RoutingProcessType = RoutingProcessType.RoutingProductRoute
                }
            });
        }

        private static void RunCompleteRouteBuild()
        {
            BPInstanceManager bpClient = new BPInstanceManager();
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
            BPInstanceManager bpClient = new BPInstanceManager();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new TOne.LCRProcess.Arguments.DifferentialRoutingProcessInput()

            });
        }

        private static void RunRouteSync()
        {
            BPInstanceManager bpClient = new BPInstanceManager();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new TOne.WhS.RouteSync.BP.Arguments.RouteSyncProcessInput()
                {
                    RouteSyncDefinitionId = 1,
                    UserId = 1
                }
            });
        }
    }
}
