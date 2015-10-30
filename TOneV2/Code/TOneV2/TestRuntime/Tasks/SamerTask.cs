using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using TOne.WhS.CDRProcessing.QueueActivators;
using TOne.WhS.CodePreparation.BP.Arguments;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    class SamerTask : ITask
    {
        public void Execute()
        {
            System.Threading.ThreadPool.SetMaxThreads(10000, 10000);

            //BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            runtimeServices.Add(queueActivationService);

            //runtimeServices.Add(bpService);

            runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            var storeRawCDRsStage = new QueueStageExecutionActivity { StageName = "Store Raw CDRs", QueueName = "RawCDRs", QueueTypeFQTN = typeof(CDRBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreRawCDRActivator).AssemblyQualifiedName } };
            var generateBillingCDRs = new QueueStageExecutionActivity { StageName = "Generate Billing CDRs", QueueName = "BillingCDRs", QueueTypeFQTN = typeof(CDRBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(GenerateBillingCDRActivator).AssemblyQualifiedName } };
            var storeInvalidCDRs = new QueueStageExecutionActivity { StageName = "Store Invalid CDRs", QueueName = "InvalidCDRs", QueueTypeFQTN = typeof(CDRInvalidBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreCDRInvalidActivator).AssemblyQualifiedName } };
            var storeFailedCDRs = new QueueStageExecutionActivity { StageName = "Store Failed CDRs", QueueName = "FailedCDRs", QueueTypeFQTN = typeof(CDRFailedBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreCDRFailedActivator).AssemblyQualifiedName } };
            var generateDailyStats = new QueueStageExecutionActivity { StageName = "Generate Daily Stats", QueueName = "DailyStats", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(GenerateDailyStatsActivator).AssemblyQualifiedName } };
            var generateStats = new QueueStageExecutionActivity { StageName = "Generate Stats", QueueName = "Stats", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(GenerateStatsActivator).AssemblyQualifiedName } };
            var storeStats = new QueueStageExecutionActivity { StageName = "Store Stats", QueueName = "StoreStats", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreStatsActivator).AssemblyQualifiedName } };
            var storeDailyStats = new QueueStageExecutionActivity { StageName = "Store Daily Stats", QueueName = "StoreDailyStats", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreDailyStatsActivator).AssemblyQualifiedName } };
            var generateCDRPrices = new QueueStageExecutionActivity { StageName = "Generate CDR Prices", QueueName = "CDRPrices", QueueTypeFQTN = typeof(CDRMainBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(GenerateCDRPricesActivator).AssemblyQualifiedName } };
            var storeMainCDRs = new QueueStageExecutionActivity { StageName = "Store Main CDRs", QueueName = "MainCDRs", QueueTypeFQTN = typeof(CDRMainBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreCDRMainActivator).AssemblyQualifiedName } };
            QueueExecutionFlowTree queueFlowTree = new QueueExecutionFlowTree
            {
                Activities = new List<BaseExecutionActivity>
                {
                    new ParallelExecutionActivity {
                         Activities = new List<BaseExecutionActivity>
                         {
                              storeRawCDRsStage,
                              new SequenceExecutionActivity{ 
                                  Activities = new List<BaseExecutionActivity>                              
                                  {
                                      generateBillingCDRs,

                                      new SplitExecutionActivity 
                                          {
                                              Activities = new List<BaseExecutionActivity>
                                              {
                                                  new SequenceExecutionActivity
                                                  { 
                                                      Activities = new List<BaseExecutionActivity>
                                                      {
                                                          generateCDRPrices,
                                                          storeMainCDRs
                                                      }
                                                  },
                                                  new ParallelExecutionActivity 
                                                  {
                                                      Activities = new List<BaseExecutionActivity>
                                                      {
                                                          new SequenceExecutionActivity
                                                          { 
                                                              Activities = new List<BaseExecutionActivity>  
                                                              {
                                                                  generateStats,
                                                                  storeStats
                                                              }
                                                          },
                                                          new SequenceExecutionActivity
                                                          { 
                                                              Activities = new List<BaseExecutionActivity>   
                                                              {
                                                                  generateDailyStats,
                                                                  storeDailyStats
                                                              }
                                                          }
                                                      }
                                                  },
                                                  storeInvalidCDRs,
                                                  storeFailedCDRs
                                              }
                                          }
                                  }
                              }
                         }
                    }
                }
            };
            var tree = Vanrise.Common.Serializer.Serialize(queueFlowTree);

            var tree1 = Vanrise.Common.Serializer.Serialize(queueFlowTree);
            //QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
            //var queuesByStages = executionFlowManager.GetQueuesByStages(2);
            //CDR cdr = new CDR
            //{
            //    ID=1,
            //    Name="test"
            //};
            //while (true)
            //{
            //    //Console.ReadKey();
            //    queuesByStages["Store CDR Raws"].Queue.EnqueueObject(cdr);
            //}
            





            //BPClient bpClient = new BPClient();
            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    InputArguments = new TOne.CDRProcess.Arguments.DailyRepricingProcessInput
            //    {
            //        RepricingDay = DateTime.Parse("2014-03-01")
            //    }
            //});

            

            //BPClient bpClient = new BPClient();
            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    InputArguments = new CodePreparationProcessInput{
            //        EffectiveDate=DateTime.Now,
            //        FileId=10,
            //        SellingNumberPlanId=1
            //    }
                
            //});

        }
    }
}
