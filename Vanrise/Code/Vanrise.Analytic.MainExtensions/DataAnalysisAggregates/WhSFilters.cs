using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions;
using Vanrise.Analytic.MainExtensions.DARecordAggregates;
using Vanrise.Analytic.MainExtensions.TimeRangeFilters;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.MainExtensions.DataAnalysisAggregationFields
{
    public static class WhSFilters
    {
        static void CreateSuddenIncreaseToHighRateZones()
        {
            RecordProfilingOutputSettings recordAnalysis = new RecordProfilingOutputSettings();
            
            //FILTER
            recordAnalysis.RecordFilter = new RecordFilterGroup();
            recordAnalysis.TimeRangeFilter = new LastPeriodFilter { PeriodLength = TimeSpan.FromMinutes(30) };

            //GROUPING
            recordAnalysis.GroupingFields = new List<DAProfCalcGroupingField>();
            recordAnalysis.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "SaleZone" });
            recordAnalysis.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "SaleRateInSysCur" });
            
            //AggregationFields
            recordAnalysis.AggregationFields = new List<DAProfCalcAggregationField>();
            //recordAnalysis.AggregationFields.Add(new CountAggregate { AggregateName = "SumAttempts" });
            //recordAnalysis.AggregationFields.Add(new SumAggregate { AggregateName = "SumDurationInSeconds", SumFieldName = "DurationInSeconds" });
        }

        static void CreateSuddenIncreaseDecrease()
        {
            RecordProfilingOutputSettings recordAnalysis = new RecordProfilingOutputSettings();

            //FILTER
            recordAnalysis.RecordFilter = new RecordFilterGroup();
            
            //GROUPING
            recordAnalysis.GroupingFields = new List<DAProfCalcGroupingField>();
            recordAnalysis.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "SaleZone" });
            recordAnalysis.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "Customer" });

            //AggregationFields
            recordAnalysis.AggregationFields = new List<DAProfCalcAggregationField>();
            //recordAnalysis.AggregationFields.Add(new CountAggregate { AggregateName = "CurrentAttempts", TimeRangeFilter = new LastPeriodFilter { PeriodLength = TimeSpan.FromMinutes(30) } });
            //recordAnalysis.AggregationFields.Add(new SumAggregate { AggregateName = "CurrentSumDurationInSeconds", SumFieldName = "DurationInSeconds", TimeRangeFilter = new LastPeriodFilter { PeriodLength = TimeSpan.FromMinutes(30) } });
            //recordAnalysis.AggregationFields.Add(new CountAggregate { AggregateName = "PreviousAttempts", TimeRangeFilter = new PreviousPeriodFilter { PeriodLength = TimeSpan.FromMinutes(30), PeriodDistanceFromNow = TimeSpan.FromMinutes(30) } });
            //recordAnalysis.AggregationFields.Add(new SumAggregate { AggregateName = "PreviousSumDurationInSeconds", SumFieldName = "DurationInSeconds", TimeRangeFilter = new PreviousPeriodFilter { PeriodLength = TimeSpan.FromMinutes(30), PeriodDistanceFromNow = TimeSpan.FromMinutes(30) } });

            //CALCULATION
            recordAnalysis.CalculationFields = new List<DAProfCalcCalculationField>();
            recordAnalysis.CalculationFields.Add(new DAProfCalcCalculationField
            {
                FieldName = "% Attempts",
                //FieldType = new FieldNumberType()
                Expression = "return Math.Abs(context.GetAggregateValue(\"CurrentAttempts\") - context.GetAggregateValue(\"PreviousAttempts\")) * 100 / context.GetAggregateValue(\"PreviousAttempts\");"
            });
            recordAnalysis.CalculationFields.Add(new DAProfCalcCalculationField
            {
                FieldName = "% Duration",
                //FieldType = new FieldNumberType()
                Expression = "return Math.Abs(context.GetAggregateValue(\"CurrentSumDurationInSeconds\") - context.GetAggregateValue(\"PreviousSumDurationInSeconds\")) * 100 / context.GetAggregateValue(\"PreviousSumDurationInSeconds\");"
            });
        }

        static void CreateFAS()
        {
            RecordProfilingOutputSettings recordAnalysis = new RecordProfilingOutputSettings();

            //FILTER
            recordAnalysis.RecordFilter = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.And };
            recordAnalysis.RecordFilter.Filters.Add(new NumberRecordFilter { FieldName = "DurationInSeconds", CompareOperator = NumberRecordFilterOperator.Greater, Value = 0M});
            recordAnalysis.RecordFilter.Filters.Add(new NumberRecordFilter { FieldName = "DurationInSeconds", CompareOperator = NumberRecordFilterOperator.Less, Value = 5M });            

            //GROUPING
            recordAnalysis.GroupingFields = new List<DAProfCalcGroupingField>();
            recordAnalysis.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "Supplier" });
            recordAnalysis.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "SaleZone" });

            //AggregationFields
            recordAnalysis.AggregationFields = new List<DAProfCalcAggregationField>();
            recordAnalysis.AggregationFields.Add(new DAProfCalcAggregationField
            {
                RecordAggregate = new CountAggregate(),
                RecordFilter = new RecordFilterGroup { Filters = new List<RecordFilter> { new StringRecordFilter { FieldName = "ReleaseSource", Value = "A" } } }
            });
            //recordAnalysis.AggregationFields.Add(new CountAggregate
            //{
            //    AggregateName = "CountAll"
            //});

            //CALCULATION
            recordAnalysis.CalculationFields = new List<DAProfCalcCalculationField>();
            recordAnalysis.CalculationFields.Add(new DAProfCalcCalculationField
            {
                FieldName = "% AParty Disconnection",
                //FieldType = new FieldNumberType()
                Expression = "return context.GetAggregateValue(\"CountReleaseFromAParty\") * 100 / context.GetAggregateValue(\"CountAll\");"
            });
        }

        static void CreateLooping()
        {
            RecordProfilingOutputSettings recordAnalysis = new RecordProfilingOutputSettings();

            //FILTER
            recordAnalysis.RecordFilter = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.And };
            
            //GROUPING
            recordAnalysis.GroupingFields = new List<DAProfCalcGroupingField>();
            recordAnalysis.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "CDPN" });
            recordAnalysis.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "CGPN" });

            //AggregationFields
            recordAnalysis.AggregationFields = new List<DAProfCalcAggregationField>();
            //recordAnalysis.AggregationFields.Add(new CountAggregate
            //{
            //    AggregateName = "Attempts"
            //});
        }
                
        static void CreateRepeatedCDPN()
        {
            RecordProfilingOutputSettings recordAnalysis = new RecordProfilingOutputSettings();

            //FILTER
            recordAnalysis.RecordFilter = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.And };

            //GROUPING
            recordAnalysis.GroupingFields = new List<DAProfCalcGroupingField>();
            recordAnalysis.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "CDPN" });

            //AggregationFields
            recordAnalysis.AggregationFields = new List<DAProfCalcAggregationField>();
            //recordAnalysis.AggregationFields.Add(new DistinctCountAggregate
            //{
            //    AggregateName = "CountCGPN",
            //    CountFieldName = "CGPN"
            //});
        }

        static void CreateRepeatedCGPN()
        {
            RecordProfilingOutputSettings recordAnalysis = new RecordProfilingOutputSettings();

            //FILTER
            recordAnalysis.RecordFilter = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.And };

            //GROUPING
            recordAnalysis.GroupingFields = new List<DAProfCalcGroupingField>();
            recordAnalysis.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "CGPN" });

            //AggregationFields
            recordAnalysis.AggregationFields = new List<DAProfCalcAggregationField>();
            //recordAnalysis.AggregationFields.Add(new DistinctCountAggregate
            //{
            //    AggregateName = "CountCDPN",
            //    CountFieldName = "CDPN"
            //});
        }

        static void CreateToOutOfAgreedDestinations()
        {
            RecordProfilingOutputSettings recordAnalysis = new RecordProfilingOutputSettings();

            //FILTER
            recordAnalysis.RecordFilter = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.And };
            recordAnalysis.RecordFilter.Filters.Add(new EmptyRecordFilter { FieldName = "SaleRateInSysCur"});
            recordAnalysis.RecordFilter.Filters.Add(new EmptyRecordFilter { FieldName = "Supplier" });

            //GROUPING
            recordAnalysis.GroupingFields = new List<DAProfCalcGroupingField>();
            recordAnalysis.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "SaleZone" });
            recordAnalysis.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "Customer" });

            //AggregationFields
            recordAnalysis.AggregationFields = new List<DAProfCalcAggregationField>();
            //recordAnalysis.AggregationFields.Add(new CountAggregate
            //{
            //    AggregateName = "Attempts"
            //});
        }

        static void CreateUnBalancedTrafficExchange()
        {
            RecordProfilingOutputSettings recordAnalysisIn = new RecordProfilingOutputSettings();

            //FILTER
            recordAnalysisIn.RecordFilter = new RecordFilterGroup();
            recordAnalysisIn.RecordFilter.Filters.Add(new StringListRecordFilter { FieldName = "CustomerAccountType", Values = new List<string> { "1" } });//Exchange
            
            //GROUPING
            recordAnalysisIn.GroupingFields = new List<DAProfCalcGroupingField>();
            recordAnalysisIn.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "Customer" });

            //AggregationFields
            recordAnalysisIn.AggregationFields = new List<DAProfCalcAggregationField>();
            //recordAnalysisIn.AggregationFields.Add(new CountAggregate { AggregateName = "Attempts" });
            //recordAnalysisIn.AggregationFields.Add(new SumAggregate { AggregateName = "SumDurationInSeconds", SumFieldName = "DurationInSeconds"});
            //recordAnalysisIn.AggregationFields.Add(new SumAggregate { AggregateName = "SumNet", SumFieldName = "SaleNetInSysCurrency" });
            
            RecordProfilingOutputSettings recordAnalysisOut = new RecordProfilingOutputSettings();

            //FILTER
            recordAnalysisOut.RecordFilter = new RecordFilterGroup();
            recordAnalysisOut.RecordFilter.Filters.Add(new StringListRecordFilter { FieldName = "SupplierAccountType", Values = new List<string> { "1" } });//Exchange

            //GROUPING
            recordAnalysisOut.GroupingFields = new List<DAProfCalcGroupingField>();
            recordAnalysisOut.GroupingFields.Add(new DAProfCalcGroupingField { FieldName = "Supplier" });

            //AggregationFields
            recordAnalysisOut.AggregationFields = new List<DAProfCalcAggregationField>();
            //recordAnalysisOut.AggregationFields.Add(new CountAggregate { AggregateName = "Attempts" });
            //recordAnalysisOut.AggregationFields.Add(new SumAggregate { AggregateName = "SumDurationInSeconds", SumFieldName = "DurationInSeconds" });
            //recordAnalysisOut.AggregationFields.Add(new SumAggregate { AggregateName = "SumNet", SumFieldName = "CostNetInSysCurrency" });

            //RecordProfilingOutputSettingsGroup recordAnalysisOverall = new RecordProfilingOutputSettingsGroup();
            //recordAnalysisOverall.SourceItems = new List<RecordProfilingOutputSettingsGroupSourceItem>
            //{
            //    new RecordProfilingOutputSettingsGroupSourceItem { Name = "InRecord", OutputFromRecordDefinition = recordAnalysisIn},
            //    new RecordProfilingOutputSettingsGroupSourceItem { Name = "OutRecord", OutputFromRecordDefinition = recordAnalysisOut}
            //};
            
            //JOIN
            //recordAnalysisOverall.Joins = new List<RecordProfilingOutputSettingsGroupJoin>();
            //recordAnalysisOverall.Joins.Add(new RecordProfilingOutputSettingsGroupJoin
            //{
            //    FieldName = "CarrierAccount",
            //   // FieldType = new FieldBusinessEntityType { "CarrierAccountBE" },
            //    FirstRecordName = "InRecord",
            //    FirstRecordFieldName = "Customer",
            //    SecondRecordName = "OutRecord",
            //    SecondRecordFieldName = "Supplier"
            //});

            //CALCULATION
            //recordAnalysisOverall.CalculationFields = new List<DAProfCalcCalculationField>();
            //recordAnalysisOverall.CalculationFields.Add(new DAProfCalcCalculationField
            //{
            //    FieldName = "# Net",
            //    //FieldType = new FieldNumberType()
            //    Expression = "return context.GetRecordValue(\"InRecord\", \"SumNet\") - context.GetRecordValue(\"OutRecord\", \"SumNet\");"
            //});
            //recordAnalysisOverall.CalculationFields.Add(new DAProfCalcCalculationField
            //{
            //    FieldName = "# Attempts",
            //    //FieldType = new FieldNumberType()
            //    Expression = "return context.GetRecordValue(\"InRecord\", \"Attempts\") - context.GetRecordValue(\"OutRecord\", \"Attempts\");"
            //});
            //recordAnalysisOverall.CalculationFields.Add(new DAProfCalcCalculationField
            //{
            //    FieldName = "# Duration in Seconds",
            //    //FieldType = new FieldNumberType()
            //    Expression = "return context.GetRecordValue(\"InRecord\", \"SumDurationInSeconds\") - context.GetRecordValue(\"OutRecord\", \"SumDurationInSeconds\");"
            //});
        }


        static void CreateOverallTrafficVariation()
        {
            RecordProfilingOutputSettings recordAnalysis = new RecordProfilingOutputSettings();

            //FILTER
            recordAnalysis.RecordFilter = new RecordFilterGroup();

            //GROUPING
            recordAnalysis.GroupingFields = new List<DAProfCalcGroupingField>();

            //AggregationFields
            recordAnalysis.AggregationFields = new List<DAProfCalcAggregationField>();
            //recordAnalysis.AggregationFields.Add(new CountAggregate { AggregateName = "CurrentAttempts", TimeRangeFilter = new LastPeriodFilter { PeriodLength = TimeSpan.FromMinutes(30) } });
            //recordAnalysis.AggregationFields.Add(new SumAggregate { AggregateName = "CurrentSumDurationInSeconds", SumFieldName = "DurationInSeconds", TimeRangeFilter = new LastPeriodFilter { PeriodLength = TimeSpan.FromMinutes(30) } });
            //recordAnalysis.AggregationFields.Add(new CountAggregate { AggregateName = "PreviousAttempts", TimeRangeFilter = new PreviousPeriodFilter { PeriodLength = TimeSpan.FromMinutes(30), PeriodDistanceFromNow = TimeSpan.FromMinutes(30) } });
            //recordAnalysis.AggregationFields.Add(new SumAggregate { AggregateName = "PreviousSumDurationInSeconds", SumFieldName = "DurationInSeconds", TimeRangeFilter = new PreviousPeriodFilter { PeriodLength = TimeSpan.FromMinutes(30), PeriodDistanceFromNow = TimeSpan.FromMinutes(30) } });

            //RecordAnalysisCalculationField
            recordAnalysis.CalculationFields = new List<DAProfCalcCalculationField>();
            recordAnalysis.CalculationFields.Add(new DAProfCalcCalculationField
            {
                FieldName = "% Attempts",
                //FieldType = new FieldNumberType()
                Expression = "return Math.Abs(context.GetAggregateValue(\"CurrentAttempts\") - context.GetAggregateValue(\"PreviousAttempts\")) * 100 / context.GetAggregateValue(\"PreviousAttempts\");"
            });
            recordAnalysis.CalculationFields.Add(new DAProfCalcCalculationField
            {
                FieldName = "% Duration",
                //FieldType = new FieldNumberType()
                Expression = "return Math.Abs(context.GetAggregateValue(\"CurrentSumDurationInSeconds\") - context.GetAggregateValue(\"PreviousSumDurationInSeconds\")) * 100 / context.GetAggregateValue(\"PreviousSumDurationInSeconds\");"
            });
        }
    }
}
