using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Business;
using Retail.Ringo.MainExtensions;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.BEBridge.BP.Arguments;
using Vanrise.BEBridge.Entities;
using Vanrise.BEBridge.MainExtensions.SourceBEReaders;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Notification.BP.Arguments;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Configuration;

namespace Retail.Runtime.Tasks
{
    public class RabihTask : ITask
    {
        public void Execute()
        {

            //UpdateUsageBalance();
            //var str = GetSerialisedSettings();

            var runtimeServices = new List<Vanrise.Runtime.Entities.RuntimeService>();
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);
            BPRegulatorRuntimeService bpRegulatorService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationRuntimeService queueActivationRuntimeService = new QueueActivationRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
           
            runtimeServices.Add(queueActivationService);
            runtimeServices.Add(schedulerService);
            runtimeServices.Add(dsRuntimeService);
            runtimeServices.Add(queueActivationRuntimeService);
            runtimeServices.Add(bpRegulatorService);
            runtimeServices.Add(queueRegulatorService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            //RunUpdateBalanceThresholdProcess();
            //RunBalanceAlertCheckerProcess();
            Console.ReadKey();

        }
        private static void UpdateUsageBalance()
        {
            UsageBalanceManager manager = new UsageBalanceManager();
            //CorrectUsageBalanceItem ub1 = new CorrectUsageBalanceItem
            //{
            //    Value = 20,
            //    AccountId = 381587,
            //    CurrencyId = 1
            //};
            ////CorrectUsageBalanceItem ub2 = new CorrectUsageBalanceItem
            ////{
            ////    Value = 21,
            ////    AccountId = 381587,
            ////    CurrencyId = 1
            ////};
            //manager.CorrectUsageBalance(new Guid("20B0C83E-6F53-49C7-B52F-828A19E6DC2A"), new CorrectUsageBalancePayload
            //{
            //    CorrectUsageBalanceItems = new List<CorrectUsageBalanceItem> { ub1 },
            //    TransactionTypeId = new Guid("007869D9-6DC2-4F56-88A4-18C8C442E49E),
            //    PeriodDate = DateTime.Parse("2016-11-21 00:00:00.000"),
            //    CorrectionProcessId = Guid.NewGuid(),
            //    IsLastBatch = true
            //});

            CorrectUsageBalanceItem ub3 = new CorrectUsageBalanceItem
            {
                Value = 20,
                AccountId = "381587",
                CurrencyId = 1
            };

            manager.CorrectUsageBalance(new Guid("20B0C83E-6F53-49C7-B52F-828A19E6DC2A"), new CorrectUsageBalancePayload
            {
                CorrectUsageBalanceItems = new List<CorrectUsageBalanceItem> { ub3 },
                TransactionTypeId = new Guid("007869D9-6DC2-4F56-88A4-18C8C442E49E"),
                PeriodDate = DateTime.Parse("2016-11-21 00:00:00.000"),
                CorrectionProcessId = Guid.NewGuid(),
                IsLastBatch = true
            });
        }
        private static void UpdateUsageBalance1()
        {
            UsageBalanceManager manager = new UsageBalanceManager();
            UpdateUsageBalanceItem ub1 = new UpdateUsageBalanceItem
            {
                Value = 200,
                AccountId = "381587",
                EffectiveOn = DateTime.Parse("2016-11-21 00:00:00.000"),
                CurrencyId = 1
            };
            UpdateUsageBalanceItem ub2 = new UpdateUsageBalanceItem
            {
                Value = 500,
                AccountId = "393486",
                EffectiveOn = DateTime.Parse("2016-11-21 00:00:00.000"),
                CurrencyId = 1
            };
            manager.UpdateUsageBalance(new Guid("20B0C83E-6F53-49C7-B52F-828A19E6DC2A"), new UpdateUsageBalancePayload
            {
                UpdateUsageBalanceItems = new List<UpdateUsageBalanceItem> { ub1, ub2 },
                TransactionTypeId = new Guid("007869D9-6DC2-4F56-88A4-18C8C442E49E")
            });

            UpdateUsageBalanceItem ub3 = new UpdateUsageBalanceItem
            {
                Value = 6980,
                AccountId = "384577",
                EffectiveOn = DateTime.Parse("2016-11-21 00:00:00.000"),
                CurrencyId = 1
            };
            //UsageBalanceUpdate ub4 = new UsageBalanceUpdate
            //{
            //    Value = 2560,
            //    AccountId = 170,
            //    EffectiveOn = DateTime.Now,
            //    CurrencyId = 1
            //};

            manager.UpdateUsageBalance(new Guid("20B0C83E-6F53-49C7-B52F-828A19E6DC2A"), new UpdateUsageBalancePayload
            {
                UpdateUsageBalanceItems = new List<UpdateUsageBalanceItem> { ub3 },
                TransactionTypeId = new Guid("007869D9-6DC2-4F56-88A4-18C8C442E49E")
            });
        }

        //private string GetSerialisedSettings()
        //{
        //    BEReceiveDefinitionSettings setting = new BEReceiveDefinitionSettings
        //    {
        //        SourceBEReader = new FileSourceReader
        //        {
        //            Setting = new FileSourceReaderSetting
        //            {
        //                Directory = @"c:\RingoSubscriberFiles",
        //                Mask = "",
        //                Extension = ".csv"
        //            },
        //            ConfigId = Guid.NewGuid()
        //        },
        //        EntitySyncDefinitions = new List<EntitySyncDefinition>
        //        {
        //            new EntitySyncDefinition
        //            {
        //                TargetBEConvertor = new AccountConvertor {ConfigId = Guid.NewGuid()},
        //                TargetBESynchronizer = new AccountSynchronizer {ConfigId = Guid.NewGuid()}
        //            }
        //            ,
        //            new EntitySyncDefinition
        //            {
        //                TargetBESynchronizer = new AccountSynchronizer {ConfigId = Guid.NewGuid()},
        //                TargetBEConvertor = new AgentConvertor {ConfigId = Guid.NewGuid()}
        //            }
        //            ,
        //            new EntitySyncDefinition
        //            {
        //                TargetBEConvertor = new DistributorConvertor {ConfigId = Guid.NewGuid()},
        //                TargetBESynchronizer = new AccountSynchronizer {ConfigId = Guid.NewGuid()}
        //            }
        //            ,
        //            new EntitySyncDefinition
        //            {
        //                TargetBESynchronizer = new AccountSynchronizer {ConfigId = Guid.NewGuid()},
        //                TargetBEConvertor = new PointOfSaleConvertor {ConfigId = Guid.NewGuid()}
        //            }
        //        }
        //    };

        //    var str = Serializer.Serialize(setting);
        //    return str;
        //}

        //void RunBESyncProcess()
        //{

        //    BPInstanceManager bpClient = new BPInstanceManager();
        //    bpClient.CreateNewProcess(new CreateProcessInput
        //    {
        //        InputArguments = new SourceBESyncProcessInput()
        //        {
        //            BEReceiveDefinitionIds = new List<Guid>()
        //            {
        //                //Guid.Parse("11ee1491-d953-49ff-8b49-6a4faf7cd169"), 
        //                //Guid.Parse("c6d84fbe-7709-4f37-9dfc-a44de77b529f"), 
        //                //Guid.Parse("a0e70e6b-65d9-45eb-9462-fe4934310c52"), 
        //                Guid.Parse("79348EB0-82FD-481E-9C72-D935065DD1CC")
        //            },
        //            UserId = 1
        //        }
        //    });
        //}

        //void RunUpdateBalanceThresholdProcess()
        //{

        //    BPInstanceManager bpClient = new BPInstanceManager();
        //    bpClient.CreateNewProcess(new CreateProcessInput
        //    {
        //        InputArguments = new BalanceAlertThresholdUpdateProcessInput()
        //        {
        //            AlertRuleTypeId = new Guid("B44BBBAD-C248-4C70-BA86-5022EADB9AEC"),
        //            UserId = 1
        //        }
        //    });
        //}

        //void RunBalanceAlertCheckerProcess()
        //{

        //    BPInstanceManager bpClient = new BPInstanceManager();
        //    bpClient.CreateNewProcess(new CreateProcessInput
        //    {
        //        InputArguments = new BalanceAlertCheckerProcessInput()
        //        {
        //            AlertRuleTypeId = new Guid("B44BBBAD-C248-4C70-BA86-5022EADB9AEC"),
        //            UserId = 1
        //        }
        //    });
        //}

        const string query = @"
DELETE FROM [runtime].[LockService]
DELETE FROM [runtime].[RunningProcess]
DELETE FROM [runtime].[RuntimeManager]
DELETE FROM [runtime].[RuntimeServiceInstance]
";
    }
}
