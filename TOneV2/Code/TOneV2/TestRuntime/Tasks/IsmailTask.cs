using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;
using System.Linq;

namespace TestRuntime
{
    public class IsmailTask : ITask
    {
        public void Execute()
        {
            CallGetAnalyticRecords();
            var runtimeNodeConfigSettings = new Vanrise.Runtime.Entities.RuntimeNodeConfigurationSettings
            {
                Processes = new Dictionary<Guid, RuntimeProcessConfiguration>()
            };
            var processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 1, 
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("5F63B435-4A1C-47E3-9E4C-13DDC7B1D9C3"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Scheduler Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Runtime.SchedulerService{ Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("A8D9CA25-0954-4DE6-A994-6CA8A254D5EA"), new RuntimeProcessConfiguration
                {
                    Name = "Scheduler Service Process",
                    Settings = processSettings
                });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 1,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("A2A41FB2-74E9-4886-A6F9-9A55FE88B79C"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Business Process Regulator Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.BusinessProcess.BPRegulatorRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("F6F6E01D-48AA-494C-BDD6-2D90415C048D"), new RuntimeProcessConfiguration
            {
                Name = "Business Process Regulator Service Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 5,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("4A4A947D-E257-4612-96F9-EF45930A826E"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Business Process Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.BusinessProcess.BusinessProcessService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("4FBA5B9A-CDFC-46A6-94FF-9F4A5AA37E16"), new RuntimeProcessConfiguration
            {
                Name = "Business Process Services Process",
                Settings = processSettings
            });

            var serializedNodeConfigSettings1 = Vanrise.Common.Serializer.Serialize(runtimeNodeConfigSettings);

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 1,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("260B6F9A-729D-4D2F-BDCA-E5E3D99EBE7B"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Queue Regulator Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Queueing.QueueRegulatorRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("53070A2A-ADC6-4E77-B4E4-14A437E057C3"), new RuntimeProcessConfiguration
            {
                Name = "Queue Regulator Service Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 3,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("0A6B4086-AE03-426B-80B4-EB20C69FEC25"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Queue Activation Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Queueing.QueueActivationRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("4A54E50D-8132-457C-8FB2-432FE95C5ACB"), new RuntimeProcessConfiguration
            {
                Name = "Queue Activation Services Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 3,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("9D8FCA9E-452D-4765-B553-C4065CA14E55"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Summary Queue Activation Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Queueing.SummaryQueueActivationRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("760A013C-1DA2-4710-8385-9A8E9A2F9DA7"), new RuntimeProcessConfiguration
            {
                Name = "Summary Queue Activation Services Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 3,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("9574E0DC-E692-40EC-A852-488E0A8DC5D0"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Data Source Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("9574E0DC-E692-40EC-A852-488E0A8DC5D0"), new RuntimeProcessConfiguration
            {
                Name = "Data Source Services Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 2,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("34DA6828-65E7-48DE-8F3B-A2A2F91E3BEA"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Big Data Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("765D932D-53FD-49FF-9704-C0DD86B7312B"), new RuntimeProcessConfiguration
            {
                Name = "Big Data Services Process",
                Settings = processSettings
            });

            var serializedNodeConfigSettings = Vanrise.Common.Serializer.Serialize(runtimeNodeConfigSettings);

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
                         },
                         new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                         {
                              Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                              {
                                   MeasureName = "Netting",
                                   MappedMeasures = new List<string> {"Netting"}
                              }
                         },
                         new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                         {
                              Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                              {
                                   MeasureName = "GlobalProfit",
                                   MappedMeasures = new List<string> {"GlobalProfit"}
                              }
                         }
                    }
                }
            };
            var serializedBillingMeasureExternalSource = Vanrise.Common.Serializer.Serialize(billingMeasureExternalSource);


            var billingStatsToBillingStatsMeasureExternalSource = new Vanrise.Analytic.Entities.AnalyticMeasureExternalSourceConfig
            {
                ExtendedSettings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.AnalyticTableMeasureExternalSource
                {
                    AnalyticTableId = new Guid("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                    DimensionMappingRules = new List<Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule>
                    {
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings= new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping 
                            { 
                                DimensionName = "Customer", 
                                MappedDimensionName = "Supplier"
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings= new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping 
                            { 
                                DimensionName = "CustomerProfile", 
                                MappedDimensionName = "SupplierProfile"
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings= new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping 
                            { 
                                DimensionName = "Supplier", 
                                MappedDimensionName = "Customer"
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings= new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping 
                            { 
                                DimensionName = "SupplierProfile", 
                                MappedDimensionName = "CustomerProfile"
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings= new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SameDimensionName 
                            { 
                                  Type = Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SameDimensionNameType.AllDimensions
                            }
                        }
                    },
                    MeasureMappingRules = new List<Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule>
                    {
                         new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                         {
                              Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                              {
                                   MeasureName = "Netting",
                                   MappedMeasures = new List<string> {"SaleNetNotNULL", "CostNetNotNULL"}
                              }
                         },
                         new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                         {
                              Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                              {
                                   MeasureName = "GlobalProfit",
                                   MappedMeasures = new List<string> {"ProfitNotNULL"}
                              }
                         }
                    }
                }
            };
            var serializedBillingStatsToBillingStatsMeasureExternalSource = Vanrise.Common.Serializer.Serialize(billingStatsToBillingStatsMeasureExternalSource);


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


        private void CallGetAnalyticRecords()
        {
            var analyticManager = new Vanrise.Analytic.Business.AnalyticManager();
            var analyticQuery = new Vanrise.Analytic.Entities.AnalyticQuery
            {
                TableId = new Guid("58dd0497-498d-40f2-8687-08f8356c63cc"),
                FromTime = DateTime.Parse("2010-01-01"),
                DimensionFields = new List<string> { "Customer", "MasterZone" },
                MeasureFields = new List<string> { "Attempts" },
                SubTables = new List<Vanrise.Analytic.Entities.AnalyticQuerySubTable>
                {
                    new  Vanrise.Analytic.Entities.AnalyticQuerySubTable
                    {
                         Dimensions = new List<string> { "Supplier" },
                         Measures = new List<string> { "PDDInSeconds", "SaleNet"},
                         OrderType = Vanrise.Analytic.Entities.AnalyticQueryOrderType.ByAllMeasures
                    },                    
                    new  Vanrise.Analytic.Entities.AnalyticQuerySubTable
                    {
                         Dimensions = new List<string> { "CDRType" },
                         Measures = new List<string> { "Attempts"},
                         OrderType = Vanrise.Analytic.Entities.AnalyticQueryOrderType.ByAllMeasures
                    },                    
                    new  Vanrise.Analytic.Entities.AnalyticQuerySubTable
                    {
                         Dimensions = new List<string> { "Switch" },
                         Measures = new List<string> { "GlobalProfit", "Netting"},
                         OrderType = Vanrise.Analytic.Entities.AnalyticQueryOrderType.ByAllMeasures
                    }
                },
                WithSummary = true
            };
            Vanrise.Analytic.Entities.AnalyticRecord summaryRecord;
            List<Vanrise.Analytic.Entities.AnalyticResultSubTable> resultSubTables;
            var rslt = analyticManager.GetAllFilteredRecords(analyticQuery, out summaryRecord, out resultSubTables);
            string serializedResultSubTables = Serializer.Serialize(resultSubTables);
            string serializedRslt = Serializer.Serialize(rslt);

            //List<AnalyticCustomRecord> customRecords = new List<AnalyticCustomRecord>();
            //foreach (var record in rslt)
            //{
            //    var customRecord = new AnalyticCustomRecord
            //    {
            //        CountCDRs = (int)record.MeasureValues["CountCDRs"].Value,
            //        TotalDuration = (decimal)record.MeasureValues["TotalDuration"].Value,
            //        CalculatedCountCDRs = record.SubTables[0].MeasureValues.Sum(itm => (int)itm["CountCDRs"].Value),
            //        CalculatedTotalDuration = record.SubTables[0].MeasureValues.Sum(itm => (decimal)itm["TotalDuration"].Value)
            //    };
            //    if (customRecord.CountCDRs != customRecord.CalculatedCountCDRs || customRecord.TotalDuration - customRecord.CalculatedTotalDuration > 0.000000000001M)
            //        throw new Exception("Invalid SubTables Measures");
            //    customRecords.Add(customRecord);
            //}
            //string serializedCustomRecords = Serializer.Serialize(customRecords);

            //if (summaryRecord != null)
            //{
            //    string serializedSummary = Serializer.Serialize(summaryRecord);

            //    List<AnalyticCustomRecord> customVerticalRecords = new List<AnalyticCustomRecord>();
            //    int colIndex = 0;
            //    foreach (var subTableSummaryMeasures in summaryRecord.SubTables[0].MeasureValues)
            //    {
            //        var customRecord = new AnalyticCustomRecord
            //        {
            //            CountCDRs = (int)subTableSummaryMeasures["CountCDRs"].Value,
            //            TotalDuration = (decimal)subTableSummaryMeasures["TotalDuration"].Value,
            //            CalculatedCountCDRs = rslt.Sum(record => (int)record.SubTables[0].MeasureValues[colIndex]["CountCDRs"].Value),
            //            CalculatedTotalDuration = rslt.Sum(record => (decimal)record.SubTables[0].MeasureValues[colIndex]["TotalDuration"].Value),
            //        };
            //        if (customRecord.CountCDRs != customRecord.CalculatedCountCDRs || customRecord.TotalDuration - customRecord.CalculatedTotalDuration > 0.000000000001M)
            //            throw new Exception("Invalid SubTables Measures");
            //        customVerticalRecords.Add(customRecord);
            //        colIndex++;
            //    }
            //    string serializedCustomVerticalRecords = Serializer.Serialize(customVerticalRecords);
            //}


        }

        private class AnalyticCustomRecord
        {
            public int CountCDRs { get; set; }

            public int CalculatedCountCDRs { get; set; }

            public decimal TotalDuration { get; set; }

            public decimal CalculatedTotalDuration { get; set; }
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