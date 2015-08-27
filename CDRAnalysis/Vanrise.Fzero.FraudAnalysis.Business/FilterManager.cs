using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Linq;
using Vanrise.Fzero.Business;
using Vanrise.Fzero.Entities;


namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class FilterManager
    {

       

        public Decimal GetCriteriaValue(FilterDefinition criteria, NumberProfile numberProfile)
        {            
            return s_criteriaDefinitions[criteria.FilterId].Expression(numberProfile);
        }

        static Dictionary<int, FilterDefinition> s_criteriaDefinitions = BuildAndGetCriteriaDefinitions();
        
        static Dictionary<int, FilterDefinition> BuildAndGetCriteriaDefinitions()
        {
            Dictionary<int, FilterDefinition> dictionary = new Dictionary<int, FilterDefinition>();

            dictionary.Add(1, new FilterDefinition()  { FilterId = 1,  OperatorTypeAllowed = CommonEnums.OperatorType.Mobile,   ExcludeHourly = false, MinValue = 0.01F, MaxValue = 0.99F,          DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 0.99, DecimalPrecision = 2",           Label = "Ratio",  Description = "Ratio Incoming Calls vs Outgoing Calls ",                                  CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioIncomingCallsvsOutgoingCalls });
            dictionary.Add(2, new FilterDefinition()  { FilterId = 2,  OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Distinct Destinations", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofDistinctDestinations });
            dictionary.Add(3, new FilterDefinition()  { FilterId = 3,  OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 1,     MaxValue = int.MaxValue,   DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0",      Label = "Count",  Description = "Count outgoing calls",                                                     CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountOutgoingCalls });
            dictionary.Add(4, new FilterDefinition()  { FilterId = 4,  OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 1,     MaxValue = int.MaxValue,   DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0",      Label = "Count",  Description = "Count of Total BTS Per MSISDN",                                            CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateCountofTotalBTSPerMSISDN });
            dictionary.Add(5, new FilterDefinition()  { FilterId = 5,  OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = float.MaxValue, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2", Label = "Volume", Description = "Total Originated Volume",                                                  CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateTotalOriginatedVolume });
            dictionary.Add(6, new FilterDefinition()  { FilterId = 6,  OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 0,     MaxValue = 100,            DecimalPrecision = 0, ToolTip = "MinValue = 0, MaxValue = 100, DecimalPrecision = 0",               Label = "Count",  Description = "Count of Total IMEI Per MSISDN",                                           CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofTotalIMEIPerMSISDN });
            dictionary.Add(7, new FilterDefinition()  { FilterId = 7,  OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 0.99F,          DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 0.99, DecimalPrecision = 2",           Label = "Ratio",  Description = "Ratio Average Incoming Duration vs Average Outgoing Duration",             CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration });
            dictionary.Add(8, new FilterDefinition()  { FilterId = 8,  OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = float.MaxValue, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2", Label = "Ratio",  Description = "Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls",                  CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls });
            dictionary.Add(9, new FilterDefinition()  { FilterId = 9,  OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = true,  MinValue = 1,     MaxValue = 24,             DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = 24, DecimalPrecision = 0",                Label = "Count",  Description = "Count of  daily active hours",                                             CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofDailyActiveHours });
            dictionary.Add(10, new FilterDefinition() { FilterId = 10, OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = true,  MinValue = 1,     MaxValue = int.MaxValue,   DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0",      Label = "Count",  Description = "Distinct Destination of Night Calls",                                      CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateDistinctDestinationofNightCalls });
            dictionary.Add(11, new FilterDefinition() { FilterId = 11, OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 0,     MaxValue = int.MaxValue,   DecimalPrecision = 0, ToolTip = "MinValue = 0, MaxValue = int.MaxValue, DecimalPrecision = 0",      Label = "Count",  Description = "Voice-Only Service Usage",                                                 CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateVoiceOnlyServiceUsage });
            dictionary.Add(12, new FilterDefinition() { FilterId = 12, OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 1.00F,          DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2",           Label = "Ratio",  Description = "Ratio of Distinct Destination vs Total Number of Calls",                   CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioofDistinctDestinationvsTotalNumberofCalls });
            dictionary.Add(13, new FilterDefinition() { FilterId = 13, OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 1.00F,          DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2",           Label = "Ratio",  Description = "Ratio International Originated Vs Outgoing Calls",                         CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioInternationalOriginatedvsOutgoingCalls });
            dictionary.Add(14, new FilterDefinition() { FilterId = 14, OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = true,  MinValue = 1,     MaxValue = int.MaxValue,   DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0",      Label = "Count",  Description = "Count of outgoing calls during peak hours ",                               CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateCountofOutgoingDuringPeakHours });
            dictionary.Add(15, new FilterDefinition() { FilterId = 15, OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = float.MaxValue, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2", Label = "Volume", Description = "Data Usage",                                                               CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateDataUsage });
            dictionary.Add(16, new FilterDefinition() { FilterId = 16, OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 1,     MaxValue = int.MaxValue,   DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0",      Label = "Count",  Description = "Consecutive Calls",                                                        CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateConsecutiveCalls });
            dictionary.Add(17, new FilterDefinition() { FilterId = 17, OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 1,     MaxValue = int.MaxValue,   DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0",      Label = "Count",  Description = "Fail Consecutive Calls",                                                   CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateFailConsecutiveCalls });
            dictionary.Add(18, new FilterDefinition() { FilterId = 18, OperatorTypeAllowed = CommonEnums.OperatorType.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 1.00F,          DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2",           Label = "Ratio",  Description = "Ratio (Count Incoming “low duration” Calls)  Vs (Count Incoming Calls)",   CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioCountIncominglowdurationCallsVsCountIncomingCalls });
            return dictionary.Where(x => x.Value.OperatorTypeAllowed == ConfigParameterManager.GetOperatorType() || x.Value.OperatorTypeAllowed == CommonEnums.OperatorType.Mobile).ToDictionary(i => i.Key, i => i.Value); 
        }

        public Dictionary<int, FilterDefinition> GetCriteriaDefinitions()
        {
            return s_criteriaDefinitions;
        }

        public List<FilterDefinitionInfo> GetCriteriaNames()
        {
            List<FilterDefinitionInfo> names = new List<FilterDefinitionInfo>();
            foreach (var i in s_criteriaDefinitions)
                names.Add(new FilterDefinitionInfo() { FilterId = i.Value.FilterId, OperatorTypeAllowed = i.Value.OperatorTypeAllowed, Description = i.Value.Description, Label = i.Value.Label, MaxValue = i.Value.MaxValue, MinValue = i.Value.MinValue, DecimalPrecision = i.Value.DecimalPrecision, ExcludeHourly = i.Value.ExcludeHourly, ToolTip = i.Value.ToolTip });

            return names;
        }
        
        // Funcs

        static decimal CalculateRatioCountIncominglowdurationCallsVsCountIncomingCalls(NumberProfile numberProfile)
        {
            Decimal countInCalls = numberProfile.AggregateValues["CountInCalls"];
            if (countInCalls != 0)
                return (numberProfile.AggregateValues["CountInLowDurationCalls"] / countInCalls);
            else
                return 0;
        }


        static decimal CalculateRatioIncomingCallsvsOutgoingCalls(NumberProfile numberProfile)
        {
            Decimal countOutCalls = numberProfile.AggregateValues["CountOutCalls"];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues["CountInCalls"] / countOutCalls);
            else
                return 0;
        }

        static decimal CalculateCountofDistinctDestinations(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["DiffOutputNumb"];
        }

        static decimal CalculateCountOutgoingCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["CountOutCalls"];
        }

        static decimal CalculateCountofTotalBTSPerMSISDN(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["TotalBTS"];
        }

        static decimal CalculateTotalOriginatedVolume(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["TotalOutVolume"];
        }

        static decimal CalculateCountofTotalIMEIPerMSISDN(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["TotalIMEI"];
        }

        static decimal CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration(NumberProfile numberProfile)
        {
            decimal callOutDurAvg = numberProfile.AggregateValues["CallOutDurAvg"];
            if (callOutDurAvg != 0)
                return (numberProfile.AggregateValues["CallInDurAvg"] / callOutDurAvg);
            else
                return 0;
        }

        static decimal CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls(NumberProfile numberProfile)
        {
            decimal countOutOnNets = numberProfile.AggregateValues["CountOutOnNets"];
            if (countOutOnNets != 0)
                return (numberProfile.AggregateValues["CountOutOffNets"] / countOutOnNets);
            else
                return 0;
        }

        static decimal CalculateCountofDailyActiveHours(NumberProfile numberProfile)
        {
          
                return numberProfile.AggregateValues["CountActiveHours"];
           
        }

        static decimal CalculateDistinctDestinationofNightCalls(NumberProfile numberProfile)
        {
            
                return numberProfile.AggregateValues["DiffOutputNumbNightCalls"]; 
            
        }

        static decimal CalculateVoiceOnlyServiceUsage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["CountOutSMSs"];
        }

        static decimal CalculateRatioofDistinctDestinationvsTotalNumberofCalls(NumberProfile numberProfile)
        {
            decimal countOutCalls = numberProfile.AggregateValues["CountOutCalls"];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues["DiffOutputNumb"] / countOutCalls);
            else
                return 0;
        }

        static decimal CalculateRatioInternationalOriginatedvsOutgoingCalls(NumberProfile numberProfile)
        {
            decimal countOutCalls = numberProfile.AggregateValues["CountOutCalls"];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues["CountOutInters"] / countOutCalls);
            else
                return 0;
        }

        static decimal CalculateCountofOutgoingDuringPeakHours(NumberProfile numberProfile)
        {
          
                return numberProfile.AggregateValues["CountOutCallsPeakHours"]; 
           
        }

        static decimal CalculateDataUsage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["TotalDataVolume"];
        }

        static decimal CalculateConsecutiveCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["CountConsecutiveCalls"];
        }

        static decimal CalculateFailConsecutiveCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["CountFailConsecutiveCalls"];
        }

        

    }
}
