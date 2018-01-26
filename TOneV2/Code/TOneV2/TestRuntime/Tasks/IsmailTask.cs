using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TOne.Business;
using TOne.CDR.Entities;
using TOne.CDR.QueueActivators;
using TOne.Entities;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
//using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace TestRuntime
{
    public class IsmailTask : ITask
    {
        public void Execute()
        {
            var billingMeasureExternalSource = new Vanrise.Analytic.Entities.AnalyticMeasureExternalSourceConfig
                {
                    ExtendedSettings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.AnalyticTableMeasureExternalSource
                    {
                        AnalyticTableId = new Guid("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                        DimensionMappingRules = new List<Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule>
                        {
                            new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                            { 
                                Settings= new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SameDimensionName 
                                { 
                                    Type = Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SameDimensionNameType.SpecificDimensions, 
                                    DimensionNames = new List<string> { "MasterZone", "Customer", "CustomerProfile", "Supplier", "SupplierProfile", "CDRType", "Switch" } 
                                }
                            }
                        },
                        MeasureMappingRules = new List<Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule>
                        {
                             new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                             {
                                  Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                                  {
                                       MeasureName = "PricedCalls",
                                       MappedMeasures = new List<string> {"NumberOfCalls"}
                                  }
                             },
                             new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                             {
                                  Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                                  {
                                       MeasureName = "SaleNet",
                                       MappedMeasures = new List<string> {"SaleNetNotNULL"}
                                  }
                             },
                             new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                             {
                                  Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                                  {
                                       MeasureName = "CostNet",
                                       MappedMeasures = new List<string> {"CostNetNotNULL"}
                                  }
                             },
                             new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                             {
                                  Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                                  {
                                       MeasureName = "Profit",
                                       MappedMeasures = new List<string> {"SaleNetNotNULL", "CostNetNotNULL"}
                                  }
                             },
                             new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                             {
                                  Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                                  {
                                       MeasureName = "PercentageProfit",
                                       MappedMeasures = new List<string> {"SaleNetNotNULL", "CostNetNotNULL"}
                                  }
                             },
                             new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                             {
                                  Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                                  {
                                       MeasureName = "MarkupPercentage",
                                       MappedMeasures = new List<string> {"SaleNetNotNULL", "CostNetNotNULL"}
                                  }
                             },
                             new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                             {
                                  Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                                  {
                                       MeasureName = "SaleDuration",
                                       MappedMeasures = new List<string> {"SaleDuration"}
                                  }
                             },
                             new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                             {
                                  Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                                  {
                                       MeasureName = "CostDuration",
                                       MappedMeasures = new List<string> {"CostDuration"}
                                  }
                             },
                             new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                             {
                                  Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                                  {
                                       MeasureName = "CostRate_DurAvg",
                                       MappedMeasures = new List<string> {"CostRate_DurAvg"}
                                  }
                             },
                             new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                             {
                                  Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                                  {
                                       MeasureName = "SaleRate_DurAvg",
                                       MappedMeasures = new List<string> {"SaleRate_DurAvg"}
                                  }
                             }
                        }
                    }
                };

            var serializedBillingMeasureExternalSource = Vanrise.Common.Serializer.Serialize(billingMeasureExternalSource);

            //var services = Vanrise.Common.Serializer.Deserialize<List<TOne.WhS.BusinessEntity.Entities.ZoneService>>("{\"$values\":[{\"ServiceId\":1},{\"ServiceId\":2},{\"ServiceId\":3}]}");
            //string inIP = null;
            //while (true)
            //{

            //    Uri url;
            //    System.Net.IPAddress ip;
            //    if (Uri.TryCreate(String.Format("http://{0}", inIP), UriKind.Absolute, out url) && System.Net.IPAddress.TryParse(url.Host, out ip))
            //    {
            //        Console.WriteLine("valid IP");
            //    }
            //    else
            //        Console.WriteLine("Invalid");
            //    inIP = Console.ReadLine();
            //}
            ////TestCacheCleaner();

            //var saleZoneDataManager = new SaleZoneDataManager();
            //object lastReceivedTimeStamp = null;
            //while(true)
            //{
            //    bool isDataUpdated = saleZoneDataManager.IsDataUpdated(ref lastReceivedTimeStamp);
            //    Console.WriteLine(isDataUpdated);
            //    Console.ReadKey();
            //}
            ////TOne.LCR.Data.SQL.CodeDataManager codeDataManager = new TOne.LCR.Data.SQL.CodeDataManager();
            ////TOne.LCR.Data.SQL.CodeMatchDataManager codeMatchDataManager = new TOne.LCR.Data.SQL.CodeMatchDataManager();
            ////List<SupplierCodeInfo> suppliersCodeInfo = codeDataManager.GetActiveSupplierCodeInfo(DateTime.Today, DateTime.Today);
            ////List<string> distinctCodes = codeDataManager.GetDistinctCodes(false);
            ////codeMatchDataManager.FillCodeMatchesFromCodes(new CodeList(distinctCodes), suppliersCodeInfo, DateTime.Today);
            ////Console.ReadKey();
            ////return;
            System.Threading.ThreadPool.SetMaxThreads(10000, 10000);
            var runtimeServices = new List<RuntimeService>();

            //var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            //Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("DemoRawCDR");

            //Vanrise.Queueing.PersistentQueueFactory.Default.CreateQueueIfNotExists<TOne.CDR.Entities.CDRBatch>(0, "testCDRQueue");
            //var queue = Vanrise.Queueing.PersistentQueueFactory.Default.GetQueue("testCDRQueue");

            //Vanrise.Caching.Runtime.CachingDistributorRuntimeService cachingDistributorRuntimeService = new Vanrise.Caching.Runtime.CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(cachingDistributorRuntimeService);
            //Vanrise.Caching.Runtime.CachingRuntimeService cachingRuntimeService = new Vanrise.Caching.Runtime.CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(cachingRuntimeService);
            BPRegulatorRuntimeService bpRegulatorService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorService);
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

            //Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(bigDataService);
            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);







            //DataGroupingDistributorRuntimeService dataGroupingDistributorService = new DataGroupingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 1) };
            //runtimeServices.Add(dataGroupingDistributorService);
            //DataGroupingExecutorRuntimeService dataGroupingExecutorService = new DataGroupingExecutorRuntimeService { Interval = new TimeSpan(0, 0, 1) };
            //runtimeServices.Add(dataGroupingExecutorService);




            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();
            Parallel.For(0, 10, (i) =>
                {
                    Lock(String.Format("Transaction {0}", i % 3), 2);
                });
            Lock("transaction X");
            //var storeCDRRawsStage = new QueueStageExecutionActivity { StageName = "Store CDR Raws", QueueName = "CDRRaw", QueueTypeFQTN = typeof(CDRBatch).AssemblyQualifiedName , QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StoreCDRRawsActivator).AssemblyQualifiedName} };
            //var processRawCDRsStage = new QueueStageExecutionActivity { StageName = "Process Raw CDRs", QueueName = "CDRRawForBilling", QueueTypeFQTN = typeof(CDRBatch).AssemblyQualifiedName, QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(ProcessRawCDRsActivator).AssemblyQualifiedName } };
            //QueueExecutionFlowTree queueFlowTree = new QueueExecutionFlowTree
            //{
            //    Activities = new List<BaseExecutionActivity>
            //    {
            //        new ParallelExecutionActivity {
            //             Activities = new List<BaseExecutionActivity>
            //             {
            //                  storeCDRRawsStage,
            //                  new SequenceExecutionActivity{ 
            //                      Activities = new List<BaseExecutionActivity>                              
            //                      {
            //                          processRawCDRsStage,
            //                          new ParallelExecutionActivity
            //                          {
            //                              Activities = new List<BaseExecutionActivity>
            //                              {
            //                                  new SequenceExecutionActivity 
            //                                  {
            //                                        Activities = new List<BaseExecutionActivity>
            //                                        {
            //                                            new QueueStageExecutionActivity  { StageName = "Process Billing CDRs", QueueName ="CDRBilling", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName},
            //                                            new SplitExecutionActivity 
            //                                            { 
            //                                                Activities = new List<BaseExecutionActivity>
            //                                                {
            //                                                    new QueueStageExecutionActivity{ StageName = "Store CDR Main",  QueueName = "CDRMain", QueueTypeFQTN = typeof(CDRMainBatch).AssemblyQualifiedName},
            //                                                    new QueueStageExecutionActivity  { StageName = "Store CDR Invalid",  QueueName = "CDRInvalid", QueueTypeFQTN = typeof(CDRInvalidBatch).AssemblyQualifiedName}
            //                                                }
            //                                            }
            //                                        }
            //                                  },
            //                                  new SequenceExecutionActivity
            //                                  {
            //                                      Activities = new List<BaseExecutionActivity>
            //                                      {
            //                                          new QueueStageExecutionActivity { StageName = "Process Traffic Statistics",QueueName = "CDRBillingForTrafficStats", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName},
            //                                          new QueueStageExecutionActivity  { StageName = "Store Traffic Statistics", QueueName = "TrafficStatistics", QueueTypeFQTN = typeof(TrafficStatisticBatch).AssemblyQualifiedName}
            //                                      }
            //                                  },
            //                                  new SequenceExecutionActivity
            //                                  {
            //                                      Activities = new List<BaseExecutionActivity>
            //                                      {
            //                                          new QueueStageExecutionActivity { StageName = "Process Daily Traffic Statistics",  QueueName = "CDRBillingForDailyTrafficStats", QueueTypeFQTN = typeof(CDRBillingBatch).AssemblyQualifiedName},
            //                                          new QueueStageExecutionActivity  { StageName = "Store Daily Traffic Statistics", QueueName = "TrafficStatisticsDaily", QueueTypeFQTN = typeof(TrafficStatisticDailyBatch).AssemblyQualifiedName}
            //                                      }
            //                                  }
            //                              }
            //                          }
            //                      }
            //                  }
            //             }
            //        }
            //    }
            //};
            //var tree = Vanrise.Common.Serializer.Serialize(queueFlowTree);



            //QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
            //var queuesByStages = executionFlowManager.GetQueuesByStages(7);
            //TOne.CDR.Entities.CDRBatch cdrBatch = new CDRBatch();
            //cdrBatch.CDRs = new List<TABS.CDR> { 
            // new TABS.CDR {  AttemptDateTime = DateTime.Now, CDPN = "343565436", CGPN = "5465436547", ConnectDateTime = DateTime.Now, Duration = new TimeSpan(0,2,4), IN_CARRIER="Trer", OUT_CARRIER = "6546"
            // }
            //};
            //cdrBatch.SwitchId = 8;
            //while (true)
            //{
            //    Console.ReadKey();
            //    queuesByStages["Store CDR Raws"].Queue.EnqueueObject(cdrBatch);
            //}



            ////QueueGroupType queueGroupType = new QueueGroupType() { ChildItems = new Dictionary<string, QueueGroupTypeItem>() };
            ////var cdrRaw = new QueueGroupTypeItem(typeof(CDRBatch).AssemblyQualifiedName);
            ////queueGroupType.ChildItems.Add("CDR Raw", cdrRaw);
            ////var cdrRawForBilling = new QueueGroupTypeItem (typeof(CDRBatch).AssemblyQualifiedName );            
            ////queueGroupType.ChildItems.Add("CDR Raw for Billing", cdrRawForBilling);
            ////var cdrBilling = new QueueGroupTypeItem(typeof(CDRBillingBatch).AssemblyQualifiedName);
            ////cdrRawForBilling.ChildItems.Add("CDR Billing", cdrBilling);
            ////var cdrMain = new QueueGroupTypeItem(typeof(CDRMainBatch).AssemblyQualifiedName);
            ////cdrBilling.ChildItems.Add("CDR Main", cdrMain);
            //Console.ReadKey();
            //host.Stop();
            //Console.ReadKey();
            //BusinessProcessRuntime.Current.TerminatePendingProcesses();
            //Timer timer = new Timer(1000);
            //timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            //timer.Start();

            //////System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
            //////    {
            //////        for (DateTime d = DateTime.Parse(ConfigurationManager.AppSettings["RepricingFrom"]); d <= DateTime.Parse(ConfigurationManager.AppSettings["RepricingTo"]); d = d.AddDays(1))
            //////        {
            //////            TriggerProcess(d);
            //////            System.Threading.Thread.Sleep(30000);
            //////        }
            //////    });
            //////t.Start();

            ////BPClient bpClient = new BPClient();
            //////bpClient.CreateNewProcess(new CreateProcessInput
            //////{
            //////    ProcessName = "RoutingProcess",
            //////    InputArguments = new TOne.LCRProcess.Arguments.RoutingProcessInput
            //////    {
            //////        DivideProcessIntoSubProcesses = true,
            //////        EffectiveTime = DateTime.Now,
            //////        IsFuture = false,
            //////        IsLcrOnly = false
            //////    }
            //////});

            ////bpClient.CreateNewProcess(new CreateProcessInput
            ////{
            ////    InputArguments = new TOne.CDRProcess.Arguments.DailyRepricingProcessInput
            ////    {
            ////        RepricingDay = DateTime.Parse("2013-03-29")//,
            ////       // DivideProcessIntoSubProcesses = true
            ////    }
            ////});

            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    ProcessName = "RoutingProcess",
            //    InputArguments = new TOne.LCRProcess.Arguments.RoutingProcessInput
            //    {
            //        EffectiveTime = DateTime.Now,
            //        IsFuture = true
            //    }
            //});

            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    ProcessName = "UpdateCodeZoneMatchProcess",
            //    InputArguments = new TOne.LCRProcess.Arguments.UpdateCodeZoneMatchProcessInput
            //    {
            //        IsFuture = false,
            //        CodeEffectiveOn = DateTime.Now
            //    }
            //});

            //processManager.CreateNewProcess(new CreateProcessInput
            //{
            //    ProcessName = "UpdateZoneRateProcess",
            //    InputArguments = new TOne.LCRProcess.Arguments.UpdateZoneRateProcessInput
            //    {
            //        IsFuture = false,
            //        ForSupplier = true,
            //        RateEffectiveOn = DateTime.Now
            //    }
            //});
        }

        private static void Lock(string transactionName, int maxConcurrency = 1)
        {
            Vanrise.Runtime.TransactionLocker locker = TransactionLocker.Instance;
            bool isLocked = locker.TryLock(transactionName, maxConcurrency, () =>
            {
                Console.WriteLine("Transaction '{0}' Locked", transactionName);
                Console.ReadKey();
            });
            if (isLocked)
                Console.WriteLine("Transaction '{0}' unlocked", transactionName);
            else
                Console.WriteLine("Cannot lock Transaction '{0}'", transactionName);
            Console.ReadKey();
        }

        private void TestCacheCleaner()
        {
            int itemsAdded = 0;
            while (true)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManagerSmall>().GetOrCreateObject(Guid.NewGuid().ToString(),
                       () => GenerateBigList(100));
                }
                for (int i = 0; i < 3; i++)
                {
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager1>().GetOrCreateObject(Guid.NewGuid().ToString(),
                       () => GenerateBigList(10000));
                }

                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManagerLarge>(Guid.NewGuid()).GetOrCreateObject(Guid.NewGuid().ToString(),
                    () => GenerateBigList(1000000));

                // System.Threading.Thread.Sleep(50);
                Console.WriteLine(++itemsAdded);

            }
        }

        private List<string> GenerateBigList(int count)
        {
            List<string> lst = new List<string>();
            for (int i = 0; i < count; i++)
            {
                lst.Add(String.Format("Item Nb : {0}", i));
            }
            return lst;
        }

        private class CacheManager1 : Vanrise.Caching.BaseCacheManager
        {

        }

        private class CacheManagerLarge : Vanrise.Caching.BaseCacheManager
        {
            public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
            {
                get
                {
                    return Vanrise.Caching.CacheObjectSize.Large;
                }
            }
        }

        private class CacheManagerSmall : Vanrise.Caching.BaseCacheManager
        {
            public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
            {
                get
                {
                    return Vanrise.Caching.CacheObjectSize.Small;
                }
            }
        }

        //static bool _isRunning;
        //static object _lockObj = new object();
        //static void timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    lock (_lockObj)
        //    {
        //        if (_isRunning)
        //            return;
        //        _isRunning = true;
        //    }
        //    try
        //    {
        //        //BusinessProcessRuntime.Current.LoadAndExecutePendings();

        //        BusinessProcessRuntime.Current.ExecutePendings();
        //        BusinessProcessRuntime.Current.TriggerPendingEvents();
        //    }
        //    finally
        //    {
        //        lock (_lockObj)
        //        {
        //            _isRunning = false;
        //        }
        //    }
        //}

        private static void TriggerProcess(DateTime date)
        {
            TOne.CDRProcess.Arguments.DailyRepricingProcessInput inputArguments = new TOne.CDRProcess.Arguments.DailyRepricingProcessInput { RepricingDay = date };
            CreateProcessInput input = new CreateProcessInput
            {
                InputArguments = inputArguments
            };
            BPInstanceManager processManager = new BPInstanceManager();
            processManager.CreateNewProcess(input);
        }
    }

    public class SaleZoneDataManager : Vanrise.Data.SQL.BaseSQLDataManager
    {
        public SaleZoneDataManager()
            : base("TOneV2DBConnString")
        {

        }


        public bool IsDataUpdated(ref object lastReceivedTimeStamp)
        {
            return IsDataUpdated("TOneWhS_BE.SaleZone", ref lastReceivedTimeStamp);
        }
    }
}