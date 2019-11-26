using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Rules;
using Vanrise.Analytic.Business;
using TOne.WhS.RouteSync.Ericsson.Entities;
using System.Linq;
using TOne.WhS.RouteSync.Ericsson;
using System.Collections;

namespace TestRuntime.Tasks
{
    class AliSlaymanTask : ITask
    {
        public void Execute()
        {
            //TestingTrunckMode();

            Console.ReadKey();

            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            //SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            //runtimeServices.Add(schedulerService);

            Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);

            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            CachingRuntimeService cachingRuntimeService = new CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingRuntimeService);

            CachingDistributorRuntimeService cachingDistributorRuntimeService = new CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingDistributorRuntimeService);

            DataGroupingExecutorRuntimeService dataGroupingExecutorRuntimeService = new Vanrise.Common.Business.DataGroupingExecutorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingExecutorRuntimeService);

            DataGroupingDistributorRuntimeService dataGroupingDistributorRuntimeService = new Vanrise.Common.Business.DataGroupingDistributorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingDistributorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            Console.ReadKey();
        }

        //private static void TestingTrunckMode()
        //{
        //    var switchTarget = new SwitchManager().GetSwitch(6);
        //    var routeSynchronizer = switchTarget.Settings.RouteSynchronizer as EricssonSWSync;
        //    var carrierMappings = routeSynchronizer.CarrierMappings;

        //    var firstOption = carrierMappings.First(item => item.Value.CarrierId == 14);

        //    var ruleTree = new EricssonSWSync().BuildSupplierTrunkGroupTree(carrierMappings);

        //    var context1 = new EricssonTrunkBackupsModeContext()
        //    {
        //        NumberOfMappings = 8,
        //        CustomerId = "101",
        //        CodeGroupId = 11,
        //        CarrierMappings = carrierMappings,
        //        GroupId = 1,
        //        IsFirstSupplier = true,
        //        IsPercentageOption = true,
        //        RuleTree = ruleTree,
        //        OptionRouteCaseOptions = new List<RouteCaseOption>(),
        //        RouteCaseOptions = new List<RouteCaseOption>(),
        //        RouteCaseOptionWithSupplierList = new List<RouteCaseOptionWithSupplier>(),
        //        RouteCodeGroup = "1",
        //        CurrentNumberOfMappings = 0,
        //        RouteOption = new TOne.WhS.RouteSync.Entities.RouteOption()
        //        {
        //            Percentage = 50,
        //            SupplierId = "14",
        //            Backups = new List<TOne.WhS.RouteSync.Entities.BackupRouteOption>() { new TOne.WhS.RouteSync.Entities.BackupRouteOption() {
        //                     NumberOfTries =3, SupplierId ="53", SupplierRate = 2 } }
        //        },
        //        TrunkGroup = firstOption.Value.SupplierMapping.TrunkGroups.First(),
        //        OutTrunksById = firstOption.Value.SupplierMapping.OutTrunks.ToDictionary(item => item.TrunkId, item => item),
        //        BranchRoute = new RORangeBranchRoute() { From = 1, IncludeTrunkAsSwitch = false, OverflowOnFirstOptionOnly = true }
        //    };
        //    BackupsAfterAllTrunks backupsMode1 = new BackupsAfterAllTrunks();
        //    backupsMode1.EvaluateTrunks(context1);

        //    var evaluatedTrunks = context1.RouteCaseOptions.GroupBy(item => item.GroupID);

        //    var context2 = new EricssonTrunkBackupsModeContext()
        //    {
        //        NumberOfMappings = 8,
        //        CustomerId = "101",
        //        CodeGroupId = 11,
        //        CarrierMappings = carrierMappings,
        //        GroupId = 1,
        //        IsFirstSupplier = true,
        //        IsPercentageOption = true,
        //        RuleTree = ruleTree,
        //        OptionRouteCaseOptions = new List<RouteCaseOption>(),
        //        RouteCaseOptions = new List<RouteCaseOption>(),
        //        RouteCaseOptionWithSupplierList = new List<RouteCaseOptionWithSupplier>(),
        //        RouteCodeGroup = "1",
        //        CurrentNumberOfMappings = 0,
        //        RouteOption = new TOne.WhS.RouteSync.Entities.RouteOption()
        //        {
        //            Percentage = 50,
        //            SupplierId = "14",
        //            Backups = new List<TOne.WhS.RouteSync.Entities.BackupRouteOption>() { new TOne.WhS.RouteSync.Entities.BackupRouteOption() {
        //                     NumberOfTries =3, SupplierId ="53", SupplierRate = 2 } }
        //        },
        //        TrunkGroup = firstOption.Value.SupplierMapping.TrunkGroups.First(),
        //        OutTrunksById = firstOption.Value.SupplierMapping.OutTrunks.ToDictionary(item => item.TrunkId, item => item),
        //        BranchRoute = new RORangeBranchRoute() { From = 1, IncludeTrunkAsSwitch = false, OverflowOnFirstOptionOnly = true }
        //    };
        //    BackupsPerTrunk backupsMode2 = new BackupsPerTrunk();
        //    backupsMode2.EvaluateTrunks(context2);

        //    var evaluatedTrunks2 = context2.RouteCaseOptions.GroupBy(item => item.GroupID);
        //}
    }
}