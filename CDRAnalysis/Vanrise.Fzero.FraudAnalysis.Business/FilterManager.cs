using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.Business;
using Vanrise.Fzero.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;


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

            dictionary.Add(1, new FilterDefinition() { FilterId = 1, Abbreviation = "Filter_1", OperatorTypeAllowed = OperatorTypeEnum.Both, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 0.99F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 0.99, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio Incoming Calls on Outgoing Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioIncomingCallsvsOutgoingCalls });
            dictionary.Add(2, new FilterDefinition() { FilterId = 2, Abbreviation = "Filter_2", OperatorTypeAllowed = OperatorTypeEnum.Both, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Distinct Called Parties", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofDistinctDestinations });
            dictionary.Add(3, new FilterDefinition() { FilterId = 3, Abbreviation = "Filter_3", OperatorTypeAllowed = OperatorTypeEnum.Both, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Outgoing Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountOutgoingCalls });
            dictionary.Add(5, new FilterDefinition() { FilterId = 5, Abbreviation = "Filter_5", OperatorTypeAllowed = OperatorTypeEnum.Both, ExcludeHourly = false, MinValue = 0.01F, MaxValue = float.MaxValue, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2", Label = "Volume", Description = "Total Originated Volume in Minutes", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateTotalOriginatedVolume });
            dictionary.Add(7, new FilterDefinition() { FilterId = 7, Abbreviation = "Filter_7", OperatorTypeAllowed = OperatorTypeEnum.Both, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 0.99F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 0.99, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio Average Incoming Duration on Average Outgoing Duration", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration });
            dictionary.Add(9, new FilterDefinition() { FilterId = 9, Abbreviation = "Filter_9", OperatorTypeAllowed = OperatorTypeEnum.Both, ExcludeHourly = true, MinValue = 1, MaxValue = 24, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = 24, DecimalPrecision = 0", Label = "Count", Description = "Count of Daily Active Hours", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofDailyActiveHours });
            dictionary.Add(10, new FilterDefinition() { FilterId = 10, Abbreviation = "Filter_10", OperatorTypeAllowed = OperatorTypeEnum.Both, ExcludeHourly = true, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Distinct Called Parties during Night Period", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateDistinctDestinationofNightCalls });
            dictionary.Add(12, new FilterDefinition() { FilterId = 12, Abbreviation = "Filter_12", OperatorTypeAllowed = OperatorTypeEnum.Both, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 1.00F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio Distinct Called Parties on Total Outgoing Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioofDistinctDestinationvsTotalNumberofCalls });
            dictionary.Add(14, new FilterDefinition() { FilterId = 14, Abbreviation = "Filter_14", OperatorTypeAllowed = OperatorTypeEnum.Both, ExcludeHourly = true, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Outgoing Calls during Peak Hours", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofOutgoingDuringPeakHours });
            dictionary.Add(16, new FilterDefinition() { FilterId = 16, Abbreviation = "Filter_16", OperatorTypeAllowed = OperatorTypeEnum.Both, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Consecutive Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateConsecutiveCalls });
            dictionary.Add(17, new FilterDefinition() { FilterId = 17, Abbreviation = "Filter_17", OperatorTypeAllowed = OperatorTypeEnum.Both, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Consecutive Failed Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateFailConsecutiveCalls });
            dictionary.Add(18, new FilterDefinition() { FilterId = 18, Abbreviation = "Filter_18", OperatorTypeAllowed = OperatorTypeEnum.Both, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 1.00F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2", Label = "Ratio", Description = "Count Incoming “low duration” Calls on Count Incoming Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioCountIncominglowdurationCallsVsCountIncomingCalls });

            
            dictionary.Add(4, new FilterDefinition() { FilterId = 4, Abbreviation = "Filter_4", OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Total Connected BTS", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateCountofTotalBTSPerMSISDN });
            dictionary.Add(6, new FilterDefinition() { FilterId = 6, Abbreviation = "Filter_6", OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0, MaxValue = 100, DecimalPrecision = 0, ToolTip = "MinValue = 0, MaxValue = 100, DecimalPrecision = 0", Label = "Count", Description = "Count of Total IMEIs", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofTotalIMEIPerMSISDN });
            dictionary.Add(8, new FilterDefinition() { FilterId = 8, Abbreviation = "Filter_8", OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = float.MaxValue, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio OffNet Originated Calls on OnNet Originated Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls });
            dictionary.Add(11, new FilterDefinition() { FilterId = 11, Abbreviation = "Filter_11", OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 0, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Sent SMSs", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateVoiceOnlyServiceUsage });
            dictionary.Add(13, new FilterDefinition() { FilterId = 13, Abbreviation = "Filter_13", OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 1.00F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio International Outgoing Calls on Outgoing Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioInternationalOriginatedvsOutgoingCalls });
            dictionary.Add(15, new FilterDefinition() { FilterId = 15, Abbreviation = "Filter_15", OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = float.MaxValue, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2", Label = "Volume", Description = "Data Usage Volume in Mbytes", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateDataUsage });

            return dictionary.Where(x => x.Value.OperatorTypeAllowed == ConfigParameterManager.GetOperatorType() || x.Value.OperatorTypeAllowed == OperatorTypeEnum.Both).OrderBy(x => x.Value.FilterId).ToDictionary(i => i.Key, i => i.Value);
        }

        public Dictionary<int, FilterDefinition> GetCriteriaDefinitions()
        {
            return s_criteriaDefinitions;
        }

        public List<FilterDefinitionInfo> GetCriteriaNames()
        {
            List<FilterDefinitionInfo> names = new List<FilterDefinitionInfo>();
            foreach (var i in s_criteriaDefinitions)
            {
                string upSign = "";
                string downSign = "";

                if( i.Value.CompareOperator== CriteriaCompareOperator.GreaterThanorEqual)
                {
                    upSign = "critical"; 
                    downSign = "safe";
                }
                else
                {
                    upSign = "safe";  
                    downSign = "critical"; 
                }
                names.Add(new FilterDefinitionInfo() { FilterId = i.Value.FilterId, OperatorTypeAllowed = i.Value.OperatorTypeAllowed, Description = i.Value.Description,  Abbreviation=i.Value.Abbreviation, Label = i.Value.Label, MaxValue = i.Value.MaxValue, MinValue = i.Value.MinValue, DecimalPrecision = i.Value.DecimalPrecision, ExcludeHourly = i.Value.ExcludeHourly, ToolTip = i.Value.ToolTip, UpSign=upSign, DownSign=downSign });
            }
            return names;
        }

        // Funcs

        static decimal CalculateRatioCountIncominglowdurationCallsVsCountIncomingCalls(NumberProfile numberProfile)
        {
            Decimal countInCalls = numberProfile.AggregateValues["Count In Calls"];
            if (countInCalls != 0)
                return (numberProfile.AggregateValues["Count In Low Duration Calls"] / countInCalls);
            else
                return 0;
        }


        static decimal CalculateRatioIncomingCallsvsOutgoingCalls(NumberProfile numberProfile)
        {
            Decimal countOutCalls = numberProfile.AggregateValues["Count Out Calls"];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues["Count In Calls"] / countOutCalls);
            else
                return 0;
        }

        static decimal CalculateCountofDistinctDestinations(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["Diff Output Numbers"];
        }

        static decimal CalculateCountOutgoingCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["Count Out Calls"];
        }

        static decimal CalculateCountofTotalBTSPerMSISDN(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["Total BTS"];
        }

        static decimal CalculateTotalOriginatedVolume(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["Total Out Volume"];
        }

        static decimal CalculateCountofTotalIMEIPerMSISDN(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["TotalIMEI"];
        }

        static decimal CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration(NumberProfile numberProfile)
        {
            decimal callOutDurAvg = numberProfile.AggregateValues["Call Out Dur Avg"];
            if (callOutDurAvg != 0)
                return (numberProfile.AggregateValues["Call In Dur Avg"] / callOutDurAvg);
            else
                return 0;
        }

        static decimal CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls(NumberProfile numberProfile)
        {
            decimal countOutOnNets = numberProfile.AggregateValues["Count Out OnNets"];
            if (countOutOnNets != 0)
                return (numberProfile.AggregateValues["Count Out OffNets"] / countOutOnNets);
            else
                return 0;
        }

        static decimal CalculateCountofDailyActiveHours(NumberProfile numberProfile)
        {

            return numberProfile.AggregateValues["Count Active Hours"];

        }

        static decimal CalculateDistinctDestinationofNightCalls(NumberProfile numberProfile)
        {

            return numberProfile.AggregateValues["Diff Output Numbers NightCalls"];

        }

        static decimal CalculateVoiceOnlyServiceUsage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["Count Out SMSs"];
        }

        static decimal CalculateRatioofDistinctDestinationvsTotalNumberofCalls(NumberProfile numberProfile)
        {
            decimal countOutCalls = numberProfile.AggregateValues["Count Out Calls"];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues["Diff Output Numbers"] / countOutCalls);
            else
                return 0;
        }

        static decimal CalculateRatioInternationalOriginatedvsOutgoingCalls(NumberProfile numberProfile)
        {
            decimal countOutCalls = numberProfile.AggregateValues["Count Out Calls"];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues["Count Out Inters"] / countOutCalls);
            else
                return 0;
        }

        static decimal CalculateCountofOutgoingDuringPeakHours(NumberProfile numberProfile)
        {

            return numberProfile.AggregateValues["Count Out Calls Peak Hours"];

        }

        static decimal CalculateDataUsage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["Total Data Volume"];
        }

        static decimal CalculateConsecutiveCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["Count Consecutive Calls"];
        }

        static decimal CalculateFailConsecutiveCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["Count Fail Consecutive Calls"];
        }

    }
}
