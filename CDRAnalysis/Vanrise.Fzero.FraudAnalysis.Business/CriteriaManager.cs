using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Linq;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class CriteriaManager
    {

        public Decimal GetCriteriaValue(CriteriaDefinition criteria, NumberProfile numberProfile)
        {            
            return s_criteriaDefinitions[criteria.FilterId].Expression(numberProfile);
        }

        static Dictionary<int, CriteriaDefinition> s_criteriaDefinitions = BuildAndGetCriteriaDefinitions();
        
        static Dictionary<int, CriteriaDefinition> BuildAndGetCriteriaDefinitions()
        {
            Dictionary<int, CriteriaDefinition> dictionary = new Dictionary<int, CriteriaDefinition>();

            dictionary.Add(1, new CriteriaDefinition() { FilterId = 1, Description = "Ratio Incoming Calls vs Outgoing Calls ", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioIncomingCallsvsOutgoingCalls });
            dictionary.Add(2, new CriteriaDefinition() { FilterId = 2, Description = "Count of Distinct Destinations", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofDistinctDestinations });
            dictionary.Add(3, new CriteriaDefinition() { FilterId = 3, Description = "Count outgoing calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountOutgoingCalls });
            dictionary.Add(4, new CriteriaDefinition() { FilterId = 4, Description = "Count of Total BTS Per MSISDN", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateCountofTotalBTSPerMSISDN });
            dictionary.Add(5, new CriteriaDefinition() { FilterId = 5, Description = "Total Originated Volume", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateTotalOriginatedVolume });
            dictionary.Add(6, new CriteriaDefinition() { FilterId = 6, Description = "Count of Total IMEI Per MSISDN", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofTotalIMEIPerMSISDN });
            dictionary.Add(7, new CriteriaDefinition() { FilterId = 7, Description = "Ratio Average Incoming Duration vs Average Outgoing Duration", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration });
            dictionary.Add(8, new CriteriaDefinition() { FilterId = 8, Description = "Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls });
            dictionary.Add(9, new CriteriaDefinition() { FilterId = 9, Description = "Count of  daily active hours", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofDailyActiveHours });
            dictionary.Add(10, new CriteriaDefinition() { FilterId = 10, Description = "Distinct Destination of Night Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateDistinctDestinationofNightCalls });
            dictionary.Add(11, new CriteriaDefinition() { FilterId = 11, Description = "Voice-Only Service Usage", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateVoiceOnlyServiceUsage });
            dictionary.Add(12, new CriteriaDefinition() { FilterId = 12, Description = "Ratio of Distinct Destination vs Total Number of Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioofDistinctDestinationvsTotalNumberofCalls });
            dictionary.Add(13, new CriteriaDefinition() { FilterId = 13, Description = "Ratio International Originated Vs Outgoing Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioInternationalOriginatedvsOutgoingCalls });
            dictionary.Add(14, new CriteriaDefinition() { FilterId = 14, Description = "Count of outgoing calls during peak hours ", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateCountofOutgoingDuringPeakHours });
            dictionary.Add(15, new CriteriaDefinition() { FilterId = 15, Description = "Data Usage", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateDataUsage });
            dictionary.Add(16, new CriteriaDefinition() { FilterId = 16, Description = "Consecutive Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateConsecutiveCalls });
            dictionary.Add(17, new CriteriaDefinition() { FilterId = 17, Description = "Fail Consecutive Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateFailConsecutiveCalls });
            dictionary.Add(18, new CriteriaDefinition() { FilterId = 18, Description = "Ratio (Count Incoming “low duration” Calls)  Vs (Count Incoming Calls)", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioCountIncominglowdurationCallsVsCountIncomingCalls });
            return dictionary; 
        }

        public Dictionary<int, CriteriaDefinition> GetCriteriaDefinitions()
        {
            return s_criteriaDefinitions;
        }

        public Dictionary<int, string> GetCriteriaNames()
        {
            Dictionary<int, String> names= new Dictionary<int,string>();
            foreach(var i in s_criteriaDefinitions)
                names.Add(i.Value.FilterId,i.Value.Description);

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
