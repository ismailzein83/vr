using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Integration.Business;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TestRuntime.Tasks
{
    class MortadaTask : ITask
    {
        public void Execute()
        {
            DefineSwapDealExternalMeasureSources();
            System.Threading.ThreadPool.SetMaxThreads(10000, 10000);

            var runtimeServices = new List<RuntimeService>();

            BPRegulatorRuntimeService regulatorRuntimeService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(regulatorRuntimeService);

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(schedulerService);

            DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            //RunCompleteProductRouteBuild();

            //BPClient bpClient = new BPClient();
            //bpClient.CreateNewProcess(new CreateProcessInput
            //{
            //    InputArguments = new TOne.WhS.Routing.BP.Arguments.BuildRoutesByCodePrefixInput
            //    {
            //        CodePrefix = "91",
            //        EffectiveOn = DateTime.Now,
            //        IsFuture = false
            //    }
            //});


        }

        private void DefineSwapDealExternalMeasureSources()
        {
            var saleBillingMeasureExternalSource = new Vanrise.Analytic.Entities.AnalyticMeasureExternalSourceConfig
            {
                ExtendedSettings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.AnalyticTableMeasureExternalSource
                {
                    AnalyticTableId = new Guid("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                    DimensionMappingRules = new List<Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule>
                    {
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings= new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.ExcludeDimensions 
                            { 
                                ExcludedDimensions = new List<string> { "IsSale"}
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping 
                            { 
                                DimensionName = "Deal",
                                MappedDimensionName = "OrigSaleDeal"
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping 
                            { 
                                DimensionName = "ZoneGroup",
                                MappedDimensionName = "OrigSaleDealZoneGroupName"
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping 
                            { 
                                DimensionName = "Zone",
                                MappedDimensionName = "SaleZoneName"
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping 
                            { 
                                DimensionName = "Day",
                                MappedDimensionName = "DayAsDate"
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
                                 MeasureName = "SaleDuration",
                                 MappedMeasures = new List<string> { "SaleDuration"}
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                        {
                            Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                            {
                                 MeasureName = "SaleAmount",
                                 MappedMeasures = new List<string> { "SaleNetNotNULL"}
                            }
                        }
                    }

                }
            };
            var serializedSaleBillingMeasureExternalSource = Vanrise.Common.Serializer.Serialize(saleBillingMeasureExternalSource);

            var costBillingMeasureExternalSource = new Vanrise.Analytic.Entities.AnalyticMeasureExternalSourceConfig
            {
                ExtendedSettings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.AnalyticTableMeasureExternalSource
                {
                    AnalyticTableId = new Guid("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                    DimensionMappingRules = new List<Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule>
                    {
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings= new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.ExcludeDimensions 
                            { 
                                ExcludedDimensions = new List<string> { "IsSale"}
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping 
                            { 
                                DimensionName = "Deal",
                                MappedDimensionName = "OrigCostDeal"
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping 
                            { 
                                DimensionName = "ZoneGroup",
                                MappedDimensionName = "OrigCostDealZoneGroupName"
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping 
                            { 
                                DimensionName = "Zone",
                                MappedDimensionName = "CostZoneName"
                            }
                        },
                        new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRule 
                        { 
                            Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SpecificDimensionMapping 
                            { 
                                DimensionName = "Day",
                                MappedDimensionName = "DayAsDate"
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
                                 MeasureName = "CostDuration",
                                 MappedMeasures = new List<string> { "CostDuration"}
                            }
                        },
                         new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.MeasureMappingRule
                        {
                            Settings = new Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.MeasureMappingRules.SpecificMapping
                            {
                                 MeasureName = "CostAmount",
                                 MappedMeasures = new List<string> { "CostNetNotNULL"}
                            }
                        }
                    }
                }
            };
            var serializedCostBillingMeasureExternalSource = Vanrise.Common.Serializer.Serialize(costBillingMeasureExternalSource);
        }

        

        //private static void RunCompleteProductRouteBuild()
        //{
        //    BPInstanceManager bpClient = new BPInstanceManager();
        //    bpClient.CreateNewProcess(new CreateProcessInput
        //    {
        //        InputArguments = new TOne.WhS.Routing.BP.Arguments.RPRoutingProcessInput
        //        {
        //            EffectiveOn = DateTime.Now,
        //            RoutingDatabaseType = TOne.WhS.Routing.Entities.RoutingDatabaseType.Current,
        //            //CodePrefixLength = 1,
        //            IsFuture = false,
        //            SaleZoneRange = 1000,
        //            SupplierZoneRPOptionPolicies = new List<SupplierZoneToRPOptionPolicy>() { 
        //                new SupplierZoneToRPOptionHighestRatePolicy() { ConfigId = 27 }, 
        //                new SupplierZoneToRPOptionLowestRatePolicy() { ConfigId = 29 }, 
        //                new SupplierZoneToRPOptionAverageRatePolicy() { ConfigId = 30 } },
        //            RoutingProcessType = RoutingProcessType.RoutingProductRoute
        //        }
        //    });
        //}
    }
}
