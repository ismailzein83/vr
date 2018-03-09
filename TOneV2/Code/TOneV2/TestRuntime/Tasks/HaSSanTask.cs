using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;

namespace TestRuntime
{
    public class HaSSanTask : ITask
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

            Console.ReadKey();
        }
    }
}