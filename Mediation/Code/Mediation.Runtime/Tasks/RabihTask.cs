using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.BP.Arguments;
using Mediation.Generic.Entities;
using Mediation.Generic.MainExtensions.MediationOutputHandlers;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Caching.Runtime;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace Mediation.Runtime.Tasks
{
    public class RabihTask : ITask
    {
        public void Execute()
        {
            RunImportProcess();
        }

        void RunImportProcess()
        {
            MediationDefinition definition = new MediationDefinition
            {
                OutputHandlers = new List<MediationOutputHandlerDefinition>(),
                ParsedRecordTypeId = new Guid(),
                ParsedRecordIdentificationSetting = new ParsedRecordIdentificationSetting()
                {
                    StatusMappings = new List<StatusMapping>()
                }
            };
            definition.OutputHandlers.Add(new MediationOutputHandlerDefinition()
            {
                Handler = new StoreRecordsOutputHandler()
                {
                    DataRecordStorageId = new Guid("0B1837DF-C8CE-4B2A-B07E-1A9F75408741")
                },
                OutputRecordName = "cookedCDR"
            });

            string serielized = Vanrise.Common.Serializer.Serialize(definition);
            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            //SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(summaryQueueActivationService);

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            //Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(bigDataService);


            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            //CachingRuntimeService cachingRuntimeService = new CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(cachingRuntimeService);

            //CachingDistributorRuntimeService cachingDistributorRuntimeService = new CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(cachingDistributorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();
        }
    }
}
