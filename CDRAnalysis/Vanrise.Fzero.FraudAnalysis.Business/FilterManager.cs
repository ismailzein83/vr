using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
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

            dictionary.Add(1, new FilterDefinition() { FilterId = 1, Abbreviation = Constants._Filter_1, OperatorTypeAllowed = OperatorType.Both, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 0.99F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 0.99, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio Incoming Calls on Outgoing Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioIncomingCallsvsOutgoingCalls });
            dictionary.Add(2, new FilterDefinition() { FilterId = 2, Abbreviation = Constants._Filter_2, OperatorTypeAllowed = OperatorType.Both, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Distinct Called Parties", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofDistinctDestinations });
            dictionary.Add(3, new FilterDefinition() { FilterId = 3, Abbreviation = Constants._Filter_3, OperatorTypeAllowed = OperatorType.Both, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Outgoing Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountOutgoingCalls });
            dictionary.Add(5, new FilterDefinition() { FilterId = 5, Abbreviation = Constants._Filter_5, OperatorTypeAllowed = OperatorType.Both, ExcludeHourly = false, MinValue = 0.01F, MaxValue = float.MaxValue, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2", Label = "Volume", Description = "Total Originated Volume in Minutes", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateTotalOriginatedVolume });
            dictionary.Add(7, new FilterDefinition() { FilterId = 7, Abbreviation = Constants._Filter_7, OperatorTypeAllowed = OperatorType.Both, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 0.99F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 0.99, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio Average Incoming Duration on Average Outgoing Duration", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration });
            dictionary.Add(9, new FilterDefinition() { FilterId = 9, Abbreviation = Constants._Filter_9, OperatorTypeAllowed = OperatorType.Both, ExcludeHourly = true, MinValue = 1, MaxValue = 24, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = 24, DecimalPrecision = 0", Label = "Count", Description = "Count of Daily Active Hours", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofDailyActiveHours });
            dictionary.Add(10, new FilterDefinition() { FilterId = 10, Abbreviation = Constants._Filter_10, OperatorTypeAllowed = OperatorType.Both, ExcludeHourly = true, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Distinct Called Parties during Night Period", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateDistinctDestinationofNightCalls });
            dictionary.Add(12, new FilterDefinition() { FilterId = 12, Abbreviation = Constants._Filter_12, OperatorTypeAllowed = OperatorType.Both, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 1.00F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio Distinct Called Parties on Total Outgoing Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioofDistinctDestinationvsTotalNumberofCalls });
            dictionary.Add(14, new FilterDefinition() { FilterId = 14, Abbreviation = Constants._Filter_14, OperatorTypeAllowed = OperatorType.Both, ExcludeHourly = true, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Outgoing Calls during Peak Hours", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofOutgoingDuringPeakHours });
            dictionary.Add(16, new FilterDefinition() { FilterId = 16, Abbreviation = Constants._Filter_16, OperatorTypeAllowed = OperatorType.Both, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Consecutive Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateConsecutiveCalls });
            dictionary.Add(17, new FilterDefinition() { FilterId = 17, Abbreviation = Constants._Filter_17, OperatorTypeAllowed = OperatorType.Both, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Consecutive Failed Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateFailConsecutiveCalls });
            dictionary.Add(18, new FilterDefinition() { FilterId = 18, Abbreviation = Constants._Filter_18, OperatorTypeAllowed = OperatorType.Both, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 1.00F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2", Label = "Ratio", Description = "Count Incoming “low duration” Calls on Count Incoming Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioCountIncominglowdurationCallsVsCountIncomingCalls });
            dictionary.Add(4, new FilterDefinition() { FilterId = 4, Abbreviation = Constants._Filter_4, OperatorTypeAllowed = OperatorType.Mobile, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Total Connected BTS", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateCountofTotalBTSPerMSISDN });
            dictionary.Add(6, new FilterDefinition() { FilterId = 6, Abbreviation = Constants._Filter_6, OperatorTypeAllowed = OperatorType.Mobile, ExcludeHourly = false, MinValue = 0, MaxValue = 100, DecimalPrecision = 0, ToolTip = "MinValue = 0, MaxValue = 100, DecimalPrecision = 0", Label = "Count", Description = "Count of Total IMEIs", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofTotalIMEIPerMSISDN });
            dictionary.Add(8, new FilterDefinition() { FilterId = 8, Abbreviation = Constants._Filter_8, OperatorTypeAllowed = OperatorType.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = float.MaxValue, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio OffNet Originated Calls on OnNet Originated Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls });
            dictionary.Add(11, new FilterDefinition() { FilterId = 11, Abbreviation = Constants._Filter_11, OperatorTypeAllowed = OperatorType.Mobile, ExcludeHourly = false, MinValue = 0, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 0, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Count of Sent SMSs", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateVoiceOnlyServiceUsage });
            dictionary.Add(13, new FilterDefinition() { FilterId = 13, Abbreviation = Constants._Filter_13, OperatorTypeAllowed = OperatorType.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = 1.00F, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = 1.00, DecimalPrecision = 2", Label = "Ratio", Description = "Ratio International Outgoing Calls on Outgoing Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioInternationalOriginatedvsOutgoingCalls });
            dictionary.Add(15, new FilterDefinition() { FilterId = 15, Abbreviation = Constants._Filter_15, OperatorTypeAllowed = OperatorType.Mobile, ExcludeHourly = false, MinValue = 0.01F, MaxValue = float.MaxValue, DecimalPrecision = 2, ToolTip = "MinValue = 0.01, MaxValue = float.MaxValue, DecimalPrecision = 2", Label = "Volume", Description = "Data Usage Volume in Mbytes", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateDataUsage });


            dictionary.Add(19, new FilterDefinition() { FilterId = 19, Abbreviation = Constants._Filter_19, OperatorTypeAllowed = OperatorType.PSTN, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Different Destination Zones", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateDifferentDestinationZones });
            dictionary.Add(20, new FilterDefinition() { FilterId = 20, Abbreviation = Constants._Filter_20, OperatorTypeAllowed = OperatorType.PSTN, ExcludeHourly = false, MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0, ToolTip = "MinValue = 1, MaxValue = int.MaxValue, DecimalPrecision = 0", Label = "Count", Description = "Different Source Zones", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateDifferentSourceZones });


            return dictionary.Where(x => x.Value.OperatorTypeAllowed == GlobalConstants._DefaultOperatorType || x.Value.OperatorTypeAllowed == OperatorType.Both).OrderBy(x => x.Value.FilterId).ToDictionary(i => i.Key, i => i.Value);
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
                    upSign = Constants._Critical;
                    downSign = Constants._Safe; 
                }
                else
                {
                    upSign = Constants._Safe; 
                    downSign = Constants._Critical; 
                }
                names.Add(new FilterDefinitionInfo() { FilterId = i.Value.FilterId, OperatorTypeAllowed = i.Value.OperatorTypeAllowed, Description = i.Value.Description,  Abbreviation=i.Value.Abbreviation, Label = i.Value.Label, MaxValue = i.Value.MaxValue, MinValue = i.Value.MinValue, DecimalPrecision = i.Value.DecimalPrecision, ExcludeHourly = i.Value.ExcludeHourly, ToolTip = i.Value.ToolTip, UpSign=upSign, DownSign=downSign });
            }
            return names;
        }

        // Funcs

        static decimal CalculateRatioCountIncominglowdurationCallsVsCountIncomingCalls(NumberProfile numberProfile)
        {
            Decimal countInCalls = numberProfile.AggregateValues[Constants._CountInCalls];
            if (countInCalls != 0)
                return (numberProfile.AggregateValues[Constants._CountInLowDurationCalls] / countInCalls);
            else
                return 0;
        }


        static decimal CalculateRatioIncomingCallsvsOutgoingCalls(NumberProfile numberProfile)
        {
            Decimal countOutCalls = numberProfile.AggregateValues[Constants._CountOutCalls];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues[Constants._CountInCalls] / countOutCalls);
            else
                return 0;
        }

        static decimal CalculateCountofDistinctDestinations(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._DiffOutputNumbers];
        }

        static decimal CalculateCountOutgoingCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountOutCalls];
        }

        static decimal CalculateCountofTotalBTSPerMSISDN(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._TotalBTS];
        }

        static decimal CalculateTotalOriginatedVolume(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._TotalOutVolume];
        }

        static decimal CalculateCountofTotalIMEIPerMSISDN(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["TotalIMEI"];
        }

        static decimal CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration(NumberProfile numberProfile)
        {
            decimal callOutDurAvg = numberProfile.AggregateValues[Constants._CallOutDurAvg];
            if (callOutDurAvg != 0)
                return (numberProfile.AggregateValues[Constants._CallInDurAvg] / callOutDurAvg);
            else
                return 0;
        }

        static decimal CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls(NumberProfile numberProfile)
        {
            decimal countOutOnNets = numberProfile.AggregateValues[Constants._CountOutOnNets];
            if (countOutOnNets != 0)
                return (numberProfile.AggregateValues[Constants._CountOutOffNets] / countOutOnNets);
            else
                return 0;
        }

        static decimal CalculateCountofDailyActiveHours(NumberProfile numberProfile)
        {

            return numberProfile.AggregateValues[Constants._CountActiveHours];

        }

        static decimal CalculateDistinctDestinationofNightCalls(NumberProfile numberProfile)
        {

            return numberProfile.AggregateValues[Constants._DiffOutputNumbersNightCalls];

        }

        static decimal CalculateVoiceOnlyServiceUsage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountOutSMSs];
        }

        static decimal CalculateRatioofDistinctDestinationvsTotalNumberofCalls(NumberProfile numberProfile)
        {
            decimal countOutCalls = numberProfile.AggregateValues[Constants._CountOutCalls];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues[Constants._DiffOutputNumbers] / countOutCalls);
            else
                return 0;
        }

        static decimal CalculateRatioInternationalOriginatedvsOutgoingCalls(NumberProfile numberProfile)
        {
            decimal countOutCalls = numberProfile.AggregateValues[Constants._CountOutCalls];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues[Constants._CountOutInters] / countOutCalls);
            else
                return 0;
        }

        static decimal CalculateCountofOutgoingDuringPeakHours(NumberProfile numberProfile)
        {

            return numberProfile.AggregateValues[Constants._CountOutCallsPeakHours];

        }

        static decimal CalculateDataUsage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._TotalDataVolume];
        }

        static decimal CalculateDifferentDestinationZones(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._DiffDestZones];
        }

        static decimal CalculateDifferentSourceZones(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._DiffSourcesZones];
        }

        static decimal CalculateConsecutiveCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountConsecutiveCalls];
        }

        static decimal CalculateFailConsecutiveCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountFailConsecutiveCalls];
        }

    }
}
