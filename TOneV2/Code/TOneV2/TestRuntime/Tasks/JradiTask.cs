using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
using Vanrise.Integration.Business;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TestRuntime.Tasks
{
    public class JradiTask : ITask
    {
        public void Execute()
        {
            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            QueueActivationRuntimeService queueActivationRuntimeService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationRuntimeService);

            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            DataSourceRuntimeService dsRuntimeService = new DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            BigDataRuntimeService bigDataService = new BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);

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
    }
}
