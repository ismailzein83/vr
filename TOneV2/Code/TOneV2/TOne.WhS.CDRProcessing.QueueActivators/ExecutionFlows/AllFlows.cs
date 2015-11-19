using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.QueueActivators.ExecutionFlows
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
                                                  new SequenceExecutionActivity 
                                                  {
                                                      Activities = new List<BaseExecutionActivity>
                                                      {
                                                                  generateStats,
                                                                  storeStats,
                                                                  new SequenceExecutionActivity
                                                                  { 
                                                                      Activities = new List<BaseExecutionActivity>   
                                                                        {
                                                                              generateDailyStats,
                                                                              storeDailyStats
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
