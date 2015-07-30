using System;
using System.Collections.Generic;
using System.Timers;
using TOne.CDR.Entities;
using TOne.CDR.QueueActivators;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    public class MustaphaTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Host Started");

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService> {queueActivationService, bpService};

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            QueueExecutionFlowTree queueFlowTree = new QueueExecutionFlowTree
            {
                Activities = new List<BaseExecutionActivity>
                {
                    new ParallelExecutionActivity {
                         Activities = new List<BaseExecutionActivity>
                         {
                              new QueueStageExecutionActivity { StageName = "Store CDR Raws", QueueName = "CDRRaw", QueueTypeFQTN = typeof(CDRBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreCDRRawsActivator).AssemblyQualifiedName } },
                              new SequenceExecutionActivity{ 
                                  Activities = new List<BaseExecutionActivity>                              
                                  {
                                      new QueueStageExecutionActivity { StageName = "Process Raw CDRs", QueueName = "CDRRawForBilling", QueueTypeFQTN = typeof(CDRBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(ProcessRawCDRsActivator).AssemblyQualifiedName } },
                                      new ParallelExecutionActivity
                                      {
                                          Activities = new List<BaseExecutionActivity>
                                          {
                                              new SequenceExecutionActivity 
                                              {
                                                    Activities = new List<BaseExecutionActivity>
                                                    {
                                                        new QueueStageExecutionActivity  { StageName = "Process Billing CDRs", QueueName ="CDRBilling", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName , QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(ProcessBillingCDRsActivator).AssemblyQualifiedName}},
                                                        new SplitExecutionActivity 
                                                        { 
                                                            Activities = new List<BaseExecutionActivity>
                                                            {
                                                                new QueueStageExecutionActivity{ StageName = "Store CDR Main",  QueueName = "CDRMain", QueueTypeFQTN = typeof(CDRMainBatch).AssemblyQualifiedName , QueueSettings =  new QueueSettings{ QueueActivatorFQTN =  typeof(StoreMainCDRsActivator).AssemblyQualifiedName}}  ,
                                                                new QueueStageExecutionActivity  { StageName = "Store CDR Invalid",  QueueName = "CDRInvalid", QueueTypeFQTN = typeof(CDRInvalidBatch).AssemblyQualifiedName , QueueSettings =  new QueueSettings{ QueueActivatorFQTN =  typeof(StoreInvalidCDRsActivator).AssemblyQualifiedName}}
                                                            }
                                                        }
                                                    }
                                              },
                                              new SequenceExecutionActivity
                                              {
                                                  Activities = new List<BaseExecutionActivity>
                                                  {
                                                      new QueueStageExecutionActivity { StageName = "Process Traffic Statistics",QueueName = "CDRBillingForTrafficStats", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName , QueueSettings =  new QueueSettings{ QueueActivatorFQTN =  typeof(GenerateTrafficStatisticsActivator).AssemblyQualifiedName} },
                                                      new QueueStageExecutionActivity  { StageName = "Store Traffic Statistics", QueueName = "TrafficStatistics", QueueTypeFQTN = typeof(TrafficStatisticBatch).AssemblyQualifiedName , QueueSettings =  new QueueSettings{ QueueActivatorFQTN =  typeof(StoreStatisticsActivator).AssemblyQualifiedName} }
                                                  }
                                              },
                                              new SequenceExecutionActivity
                                              {
                                                  Activities = new List<BaseExecutionActivity>
                                                  {
                                                      new QueueStageExecutionActivity { StageName = "Process Daily Traffic Statistics",  QueueName = "CDRBillingForDailyTrafficStats", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName , QueueSettings =  new QueueSettings{ QueueActivatorFQTN =  typeof(GenerateDailyTrafficStatisticsActivator).AssemblyQualifiedName} },
                                                      new QueueStageExecutionActivity  { StageName = "Store Daily Traffic Statistics", QueueName = "TrafficStatisticsDaily", QueueTypeFQTN = typeof(TrafficStatisticDailyBatch).AssemblyQualifiedName  , QueueSettings =  new QueueSettings{ QueueActivatorFQTN =  typeof(StoreDailyStatisticsActivator).AssemblyQualifiedName} }
                                                  }
                                              }
                                          }
                                      }
                                  }
                              }
                         }
                    }
                }
            };
            var tree = Vanrise.Common.Serializer.Serialize(queueFlowTree);



            QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
            var queuesByStages = executionFlowManager.GetQueuesByStages(1);
            TOne.CDR.Entities.CDRBatch cdrBatch = new CDRBatch
            {
                CDRs = new List<TABS.CDR>
                {
                    new TABS.CDR()
                    {
                        Switch = (TABS.Switch.All.ContainsKey(8)) ? TABS.Switch.All[8] : null,
                        IDonSwitch = 8,
                        Tag = String.Empty,
                        AttemptDateTime = DateTime.Now,
                        AlertDateTime = DateTime.Now.AddSeconds(1),
                        ConnectDateTime = DateTime.Now.AddSeconds(1),
                        DisconnectDateTime = DateTime.Now.AddSeconds(2),
                        Duration = DateTime.Now.AddSeconds(2) - DateTime.Now.AddSeconds(1),
                        IN_TRUNK = "140",
                        IN_CIRCUIT = 23,
                        IN_CARRIER = "EAS",
                        IN_IP = String.Empty,
                        OUT_TRUNK = "21",
                        OUT_CIRCUIT = 31,
                        OUT_CARRIER = "C045",
                        OUT_IP = String.Empty,
                        CGPN = "97477658129",
                        CDPN = "21695679495",
                        CDPNOut = String.Empty,
                        CAUSE_FROM = "A",
                        CAUSE_FROM_RELEASE_CODE = "CAU_NCC",
                        CAUSE_TO = "B",
                        CAUSE_TO_RELEASE_CODE = "CAU_NCC",
                        Extra_Fields = String.Empty,
                        IsRerouted = false,
                    }
                },
                SwitchId = 8
            };
            while (true)
            {
                Console.WriteLine("Press key to import cdr");
                Console.ReadKey();
                queuesByStages["Store CDR Raws"].Queue.EnqueueObject(cdrBatch);
            }
            
            
        }

        static bool _isRunning;
        static object _lockObj = new object();
        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lockObj)
            {
                if (_isRunning)
                    return;
                _isRunning = true;
            }
            try
            {
                BusinessProcessRuntime.Current.ExecutePendings();
                BusinessProcessRuntime.Current.TriggerPendingEvents();
            }
            finally
            {
                lock (_lockObj)
                {
                    _isRunning = false;
                }
            }
        }
    }
}
