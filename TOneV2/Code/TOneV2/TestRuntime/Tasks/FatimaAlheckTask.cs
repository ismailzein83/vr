using System;
using System.Collections.Generic;
using TestRuntime;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TOne.WhS.Runtime.Tasks
{
    public class FatimaAlheckTask : ITask
    {
        public void Execute()
        {
              var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

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
            //var dbReplicationDatabaseDefinitions = new Dictionary<Guid, DBReplicationDatabaseDefinition>();
            //dbReplicationDatabaseDefinitions.Add(Guid.NewGuid(), new DBReplicationDatabaseDefinition()
            //{
            //    Name = "TOneConfiguration",
            //    Tables = new List<DBReplicationTableDefinition>()
            //    {
            //        new DBReplicationTableDefinition(){TableName = "StatusDefinition", TableSchema = "Common"},
            //        new DBReplicationTableDefinition(){TableName = "VRAppVisibility", TableSchema = "Common"},
            //        new DBReplicationTableDefinition(){TableName = "BPDefinition", TableSchema = "bp"}
            //    }
            //});
            //dbReplicationDatabaseDefinitions.Add(Guid.NewGuid(), new DBReplicationDatabaseDefinition()
            //{
            //    Name = "TOneV2_Dev",
            //    Tables = new List<DBReplicationTableDefinition>()
            //    {
            //        new DBReplicationTableDefinition(){TableName = "Currency", TableSchema = "Common"},
            //        new DBReplicationTableDefinition(){TableName = "AccountManager", TableSchema = "TOneWhS_BE"},
            //        new DBReplicationTableDefinition(){TableName = "ANumberGroup", TableSchema = "TOneWhS_BE"}
            //    }
            //});

            ////Name = "DBReplicationDefinition",
            ////VRComponentTypeId = Guid.NewGuid(),
            //DBReplicationDefinitionSettings dbReplicationDefinitionSettings = new DBReplicationDefinitionSettings()
            //{
            //    DatabaseDefinitions = dbReplicationDatabaseDefinitions
            //};

            //string serializedDBReplicationDefinition = Vanrise.Common.Serializer.Serialize(dbReplicationDefinitionSettings);
        }
    }
}