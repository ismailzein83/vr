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

            dictionary.Add(1, new FilterDefinition() { FilterId = 1, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 0.99F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 0.99, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio Incoming Calls on Outgoing Calls ", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioIncomingCallsvsOutgoingCalls });
            dictionary.Add(2, new FilterDefinition() { FilterId = 2, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Distinct Called Destinations", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofDistinctDestinations });
            dictionary.Add(3, new FilterDefinition() { FilterId = 3, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count Outgoing calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountOutgoingCalls });
            dictionary.Add(4, new FilterDefinition() { FilterId = 4, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Total BTS Per MSISDN", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateCountofTotalBTSPerMSISDN });
            dictionary.Add(5, new FilterDefinition() { FilterId = 5, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = float.MaxValue, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2", Label = "Volume", Description = "Total Originated Volume", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateTotalOriginatedVolume });
            dictionary.Add(6, new FilterDefinition() { FilterId = 6, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0, MaxValue = 100, DecimalPrecision = 0, ToolTip = "MinValue = 0, MaxValue = 100, DecimalPrecision = 0", Label = "Count", Description = "Count of Total IMEI Per MSISDN", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofTotalIMEIPerMSISDN });
            dictionary.Add(7, new FilterDefinition() { FilterId = 7, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 0.99F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 0.99, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio Average Incoming Duration on Average Outgoing Duration", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration });
            dictionary.Add(8, new FilterDefinition() { FilterId = 8, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = float.MaxValue, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio OffNet Originated Calls on OnNet Originated Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls });
            dictionary.Add(9, new FilterDefinition() { FilterId = 9, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = true, MinValue = 1, MaxValue = 24, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = 24, DecimalPrecision = 0", Label = "Count", Description = "Count of  Daily Active Hours", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofDailyActiveHours });
            dictionary.Add(10, new FilterDefinition() { FilterId = 10, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = true, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Distinct Destination of Night Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateDistinctDestinationofNightCalls });
            dictionary.Add(11, new FilterDefinition() { FilterId = 11, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 0, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Voice-Only Service Usage", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateVoiceOnlyServiceUsage });
            dictionary.Add(12, new FilterDefinition() { FilterId = 12, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 1.00F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio of Distinct Destination on Total Number of Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioofDistinctDestinationvsTotalNumberofCalls });
            dictionary.Add(13, new FilterDefinition() { FilterId = 13, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 1.00F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio International Originated on Outgoing Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioInternationalOriginatedvsOutgoingCalls });
            dictionary.Add(14, new FilterDefinition() { FilterId = 14, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = true, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Outgoing Calls during Peak Hours ", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateCountofOutgoingDuringPeakHours });
            dictionary.Add(15, new FilterDefinition() { FilterId = 15, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = float.MaxValue, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2", Label = "Volume", Description = "Data Usage", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateDataUsage });
            dictionary.Add(16, new FilterDefinition() { FilterId = 16, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Consecutive Calls in seconds", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateConsecutiveCalls });
            dictionary.Add(17, new FilterDefinition() { FilterId = 17, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Fail Consecutive Calls in seconds", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateFailConsecutiveCalls });
            dictionary.Add(18, new FilterDefinition() { FilterId = 18, OperatorTypeAllowed = OperatorTypeEnum.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 1.00F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio (Count Incoming “low duration” Calls)  on (Count Incoming Calls)", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioCountIncominglowdurationCallsVsCountIncomingCalls });
            return dictionary.Where(x => x.Value.OperatorTypeAllowed == ConfigParameterManager.GetOperatorType() || x.Value.OperatorTypeAllowed == OperatorTypeEnum.Mobile).ToDictionary(i => i.Key, i => i.Value);
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
                names.Add(new FilterDefinitionInfo() { FilterId = i.Value.FilterId, OperatorTypeAllowed = i.Value.OperatorTypeAllowed, Description = i.Value.Description, Label = i.Value.Label, MaxValue = i.Value.MaxValue, MinValue = i.Value.MinValue, DecimalPrecision = i.Value.DecimalPrecision, ExcludeHourly = i.Value.ExcludeHourly, ToolTip = i.Value.ToolTip, UpSign=upSign, DownSign=downSign });
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
