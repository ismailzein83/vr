using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using TOne.WhS.CDRProcessing.QueueActivators;
using Vanrise.Queueing.Entities;

namespace TestRuntime.ExecutionFlows
{
    public class AllFlows
    {
        public static string GetImportCDRFlow()
        {
            var storeRawCDRsStage = new QueueStageExecutionActivity { StageName = "Store Raw CDRs", QueueName = "RawCDRs", QueueTypeFQTN = typeof(CDRBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreRawCDRActivator).AssemblyQualifiedName } };
            var generateBillingCDRs = new QueueStageExecutionActivity { StageName = "Generate Billing CDRs", QueueName = "BillingCDRs", QueueTypeFQTN = typeof(CDRBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(GenerateBillingCDRActivator).AssemblyQualifiedName } };
            var storeInvalidCDRs = new QueueStageExecutionActivity { StageName = "Store Invalid CDRs", QueueName = "InvalidCDRs", QueueTypeFQTN = typeof(CDRInvalidBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreCDRInvalidActivator).AssemblyQualifiedName } };
            var storeFailedCDRs = new QueueStageExecutionActivity { StageName = "Store Failed CDRs", QueueName = "FailedCDRs", QueueTypeFQTN = typeof(CDRFailedBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreCDRFailedActivator).AssemblyQualifiedName } };
            var generateDailyStats = new QueueStageExecutionActivity { StageName = "Generate Daily Stats", QueueName = "DailyStats", QueueTypeFQTN = typeof(TrafficStatisticByIntervalBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(GenerateDailyStatsActivator).AssemblyQualifiedName } };
            var generateStats = new QueueStageExecutionActivity { StageName = "Generate Stats", QueueName = "Stats", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(GenerateStatsActivator).AssemblyQualifiedName } };
            var storeStats = new QueueStageExecutionActivity { StageName = "Store Stats", QueueName = "StoreStats", QueueTypeFQTN = typeof(TrafficStatisticByIntervalBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreStatsActivator).AssemblyQualifiedName } };
            var storeDailyStats = new QueueStageExecutionActivity { StageName = "Store Daily Stats", QueueName = "StoreDailyStats", QueueTypeFQTN = typeof(TrafficStatisticDailyBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreDailyStatsActivator).AssemblyQualifiedName } };
            var generateCDRPrices = new QueueStageExecutionActivity { StageName = "Generate CDR Prices", QueueName = "CDRPrices", QueueTypeFQTN = typeof(CDRMainBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(GenerateCDRPricesActivator).AssemblyQualifiedName } };
            var storeMainCDRs = new QueueStageExecutionActivity { StageName = "Store Main CDRs", QueueName = "MainCDRs", QueueTypeFQTN = typeof(CDRMainBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreCDRMainActivator).AssemblyQualifiedName } };
            var generateBillingStats = new QueueStageExecutionActivity { StageName = "Generate BillingStats", QueueName = "BillingStats", QueueTypeFQTN = typeof(CDRMainBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(GenerateBillingStatsActivator).AssemblyQualifiedName } };
            var storeBillingStats = new QueueStageExecutionActivity { StageName = "Store BillingStats", QueueName = "StoreBillingStats", QueueTypeFQTN = typeof(BillingStatisticBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreBillingStatsActivator).AssemblyQualifiedName } };
            var generateStatsByCode = new QueueStageExecutionActivity { StageName = "Generate StatsByCode", QueueName = "StatsByCode", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(GenerateStatsByCodeActivator).AssemblyQualifiedName } };
            var storeStatsByCode = new QueueStageExecutionActivity { StageName = "Store StatsByCode", QueueName = "StoreStatsByCode", QueueTypeFQTN = typeof(TrafficStatisticByCodeBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreStatsByCodeActivator).AssemblyQualifiedName } };
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
                                                           new ParallelExecutionActivity {
                                                                Activities = new List<BaseExecutionActivity>
                                                                 {
                                                                    storeMainCDRs,
                                                                    new SequenceExecutionActivity
                                                                          { 
                                                                              Activities = new List<BaseExecutionActivity>   
                                                                              {
                                                                                  generateBillingStats,
                                                                                  storeBillingStats,
                                                                              }
                                                                          }
                                                                    
                                                                 }
                                                           }
                                                          
                                                      }
                                                  },
                                                  new ParallelExecutionActivity {
                                                      Activities = new List<BaseExecutionActivity>
                                                      {
                                                          new SequenceExecutionActivity 
                                                          {
                                                              Activities = new List<BaseExecutionActivity>
                                                              {
                                                                  generateStats,
                                                                  new ParallelExecutionActivity {
                                                                      Activities = new List<BaseExecutionActivity>
                                                                      {
                                                                          storeStats,
                                                                          new SequenceExecutionActivity
                                                                          { 
                                                                              Activities = new List<BaseExecutionActivity>   
                                                                              {
                                                                                  generateDailyStats,
                                                                                  storeDailyStats,
                                                                              }
                                                                          }
                                                                      }
                                                                  },
                                                              }
                                                          },
                                                          new SequenceExecutionActivity 
                                                          {
                                                              Activities = new List<BaseExecutionActivity>
                                                              {
                                                                  generateStatsByCode,
                                                                  storeStatsByCode
                                                              }
                                                          },
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
            return Vanrise.Common.Serializer.Serialize(queueFlowTree);
        }
    }
}
