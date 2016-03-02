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
            var storeRawCDRsStage = new QueueStageExecutionActivity { StageName = "Store Raw CDRs", QueueName = "RawCDRs", QueueTypeFQTN = typeof(CDRBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator = new StoreRawCDRActivator() } };
            var generateBillingCDRs = new QueueStageExecutionActivity { StageName = "Generate Billing CDRs", QueueName = "BillingCDRs", QueueTypeFQTN = typeof(CDRBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator = new GenerateBillingCDRActivator() } };
            var storeInvalidCDRs = new QueueStageExecutionActivity { StageName = "Store Invalid CDRs", QueueName = "InvalidCDRs", QueueTypeFQTN = typeof(CDRInvalidBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator = new StoreCDRInvalidActivator() } };
            var storeFailedCDRs = new QueueStageExecutionActivity { StageName = "Store Failed CDRs", QueueName = "FailedCDRs", QueueTypeFQTN = typeof(CDRFailedBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator = new StoreCDRFailedActivator() } };
            var generateDailyStats = new QueueStageExecutionActivity { StageName = "Generate Daily Stats", QueueName = "DailyStats", QueueTypeFQTN = typeof(TrafficStatisticByIntervalBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator = new GenerateDailyStatsActivator() } };
            var generateStats = new QueueStageExecutionActivity { StageName = "Generate Stats", QueueName = "Stats", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator = new GenerateStatsActivator() } };
            var storeStats = new QueueStageExecutionActivity { StageName = "Store Stats", QueueName = "StoreStats", QueueTypeFQTN = typeof(TrafficStatisticByIntervalBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator = new StoreStatsActivator() } };
            var storeDailyStats = new QueueStageExecutionActivity { StageName = "Store Daily Stats", QueueName = "StoreDailyStats", QueueTypeFQTN = typeof(TrafficStatisticDailyBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator =new StoreDailyStatsActivator() } };
            var generateCDRPrices = new QueueStageExecutionActivity { StageName = "Generate CDR Prices", QueueName = "CDRPrices", QueueTypeFQTN = typeof(CDRMainBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator = new GenerateCDRPricesActivator() } };
            var storeMainCDRs = new QueueStageExecutionActivity { StageName = "Store Main CDRs", QueueName = "MainCDRs", QueueTypeFQTN = typeof(CDRMainBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator =new StoreCDRMainActivator() } };
            var generateBillingStats = new QueueStageExecutionActivity { StageName = "Generate BillingStats", QueueName = "BillingStats", QueueTypeFQTN = typeof(CDRMainBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator = new GenerateBillingStatsActivator() } };
            var storeBillingStats = new QueueStageExecutionActivity { StageName = "Store BillingStats", QueueName = "StoreBillingStats", QueueTypeFQTN = typeof(BillingStatisticBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator = new StoreBillingStatsActivator() } };
            var generateStatsByCode = new QueueStageExecutionActivity { StageName = "Generate StatsByCode", QueueName = "StatsByCode", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator = new GenerateStatsByCodeActivator() } };
            var storeStatsByCode = new QueueStageExecutionActivity { StageName = "Store StatsByCode", QueueName = "StoreStatsByCode", QueueTypeFQTN = typeof(TrafficStatisticByCodeBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { Activator = new StoreStatsByCodeActivator() } };
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
