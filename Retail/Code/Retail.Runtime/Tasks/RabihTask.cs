using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.RingoExtensions;
using Vanrise.BEBridge.BP.Arguments;
using Vanrise.BEBridge.Entities;
using Vanrise.BEBridge.MainExtensions.SourceBEReaders;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Configuration;

namespace Retail.Runtime.Tasks
{
    public class RabihTask : ITask
    {
        public void Execute()
        {
            
            BEReceiveDefinitionSettings setting = new BEReceiveDefinitionSettings
            {
                SourceBEReader = new FileSourceReader
                {
                    Setting = new FileSourceReaderSetting
                    {
                        Directory = @"c:\RingoSubscriberFiles",
                        Mask = "",
                        Extension = ".csv"
                    }
                },
                EntitySyncDefinitions = new List<EntitySyncDefinition>
                {
                    new EntitySyncDefinition
                    {
                      TargetBEConvertor = new RingoFileAccountConvertor
                      {
                         
                      },
                       TargetBESynchronizer = new AccountSynchronizer
                       {
                           
                       }
                    },
                    new EntitySyncDefinition
                    {
                         TargetBESynchronizer =  new AgentSynchronizer
                         {
                             
                         },
                         TargetBEConvertor = new AgentConvertor
                         {
                             
                         }
                    }
                }
            };

            var str = Serializer.Serialize(setting);

            var runtimeServices = new List<Vanrise.Runtime.RuntimeService>();
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            BPRegulatorRuntimeService bpRegulatorService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };

            QueueActivationRuntimeService queueActivationRuntimeService = new QueueActivationRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            TransactionLockRuntimeService transactionLockRuntimeService = new TransactionLockRuntimeService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(transactionLockRuntimeService);
            runtimeServices.Add(queueActivationService);
            runtimeServices.Add(schedulerService);
            runtimeServices.Add(dsRuntimeService);
            runtimeServices.Add(queueActivationRuntimeService);
            runtimeServices.Add(bpRegulatorService);
            runtimeServices.Add(queueRegulatorService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            RunBESyncProcess();
            Console.ReadKey();

        }

        void RunBESyncProcess()
        {

            BPInstanceManager bpClient = new BPInstanceManager();
            bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new SourceBESyncProcessInput()
                {
                    BEReceiveDefinitionIds = new List<Guid>() { Guid.Parse("79348eb0-82fd-481e-9c72-d935065dd1cc") },
                    UserId = 1
                }
            });
        }

        const string query = @"
DELETE FROM [runtime].[LockService]
DELETE FROM [runtime].[RunningProcess]
DELETE FROM [runtime].[RuntimeManager]
DELETE FROM [runtime].[RuntimeServiceInstance]
";
    }
}
