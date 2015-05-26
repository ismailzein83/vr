using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class CriteriaManager
    {

        public Decimal GetCriteriaValue(CriteriaDefinition criteria, NumberProfile numberProfile)
        {
            Dictionary<int, CriteriaDefinition> dictionary = new Dictionary<int, CriteriaDefinition>();
            dictionary = GetCriteriaDefinitions();
            return  dictionary[criteria.CriteriaId].Expression(numberProfile);
        }

        
        public Dictionary<int, CriteriaDefinition> GetCriteriaDefinitions()
        {
            Dictionary<int, CriteriaDefinition> dictionary = new Dictionary<int, CriteriaDefinition>();

            dictionary.Add(1, new CriteriaDefinition() { CriteriaId = 1, Description = "Ratio_Incoming_Calls_vs_Outgoing_Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioIncomingCallsvsOutgoingCalls });
            dictionary.Add(2, new CriteriaDefinition() { CriteriaId = 2, Description = "Count_of_Distinct_Destinations", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofDistinctDestinations });
            dictionary.Add(3, new CriteriaDefinition() { CriteriaId = 3, Description = "Count_outgoing_calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountOutgoingCalls });
            dictionary.Add(4, new CriteriaDefinition() { CriteriaId = 4, Description = "Count_of_Total_BTS_Per_MSISDN", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateCountofTotalBTSPerMSISDN });
            dictionary.Add(5, new CriteriaDefinition() { CriteriaId = 5, Description = "Total_Originated_Volume", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateTotalOriginatedVolume });
            dictionary.Add(6, new CriteriaDefinition() { CriteriaId = 6, Description = "Count_of_Total_IMEI_Per_MSISDN", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateCountofTotalIMEIPerMSISDN });
            dictionary.Add(7, new CriteriaDefinition() { CriteriaId = 7, Description = "Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration });
            dictionary.Add(8, new CriteriaDefinition() { CriteriaId = 8, Description = "Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls });
            dictionary.Add(9, new CriteriaDefinition() { CriteriaId = 9, Description = "Count_of_daily_active_hours", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateCountofDailyActiveHours });
            dictionary.Add(10, new CriteriaDefinition() { CriteriaId = 10, Description = "Distinct_Destination_of_Night_Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateDistinctDestinationofNightCalls });
            dictionary.Add(11, new CriteriaDefinition() { CriteriaId = 11, Description = "Voice_Only_Service_Usage", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateVoiceOnlyServiceUsage });
            dictionary.Add(12, new CriteriaDefinition() { CriteriaId = 12, Description = "Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateRatioofDistinctDestinationvsTotalNumberofCalls });
            dictionary.Add(13, new CriteriaDefinition() { CriteriaId = 13, Description = "Ratio_International_Originated_Vs_Outgoing_Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateRatioInternationalOriginatedvsOutgoingCalls });
            dictionary.Add(14, new CriteriaDefinition() { CriteriaId = 14, Description = "Count_of_outgoing_during_peak_hours", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateCountofOutgoingDuringPeakHours });
            dictionary.Add(15, new CriteriaDefinition() { CriteriaId = 15, Description = "Data_Usage", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = CalculateDataUsage });
            dictionary.Add(16, new CriteriaDefinition() { CriteriaId = 16, Description = "Consecutive_Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = CalculateConsecutiveCalls });

            return dictionary; 
        }

        
        // Funcs
        static decimal CalculateRatioIncomingCallsvsOutgoingCalls(NumberProfile numberProfile)
        {
            if (numberProfile.AggregateValues["TotalInVolume"] != 0 && numberProfile.AggregateValues["TotalOutVolume"] != 0)
                return (numberProfile.AggregateValues["CountInCalls"] / numberProfile.AggregateValues["CountOutCalls"]);
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
            if (numberProfile.AggregateValues["CallInDurAvg"] != 0 && numberProfile.AggregateValues["CallOutDurAvg"] != 0)
                return (numberProfile.AggregateValues["CallInDurAvg"] / numberProfile.AggregateValues["CallOutDurAvg"]);
            else
                return 0;
        }

        static decimal CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls(NumberProfile numberProfile)
        {
            if (numberProfile.AggregateValues["CountInOffNets"] != 0 && numberProfile.AggregateValues["CountOutOffNets"] != 0)
                return (numberProfile.AggregateValues["CountInOffNets"] / numberProfile.AggregateValues["CountOutOffNets"]);
            else
                return 0;
        }

        static decimal CalculateCountofDailyActiveHours(NumberProfile numberProfile)
        {
            // not developed yet
            return 0;
        }

        static decimal CalculateDistinctDestinationofNightCalls(NumberProfile numberProfile)
        {
            if (numberProfile.Period == Enums.Period.Day)
            {
                return numberProfile.AggregateValues["DiffOutputNumbNightCalls"]; 
            }
            return 0;
        }

        static decimal CalculateVoiceOnlyServiceUsage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["CountOutSMSs"];
        }

        static decimal CalculateRatioofDistinctDestinationvsTotalNumberofCalls(NumberProfile numberProfile)
        {
            if (numberProfile.AggregateValues["DiffOutputNumb"] != 0 && numberProfile.AggregateValues["CountOutCalls"] != 0)
                return (numberProfile.AggregateValues["DiffOutputNumb"] / numberProfile.AggregateValues["CountOutCalls"]);
            else
                return 0;
        }

        static decimal CalculateRatioInternationalOriginatedvsOutgoingCalls(NumberProfile numberProfile)
        {
            if (numberProfile.AggregateValues["CountInInters"] != 0 && numberProfile.AggregateValues["CountOutCalls"] != 0)
                return (numberProfile.AggregateValues["CountInInters"] / numberProfile.AggregateValues["CountOutCalls"]);
            else
                return 0;
        }

        static decimal CalculateCountofOutgoingDuringPeakHours(NumberProfile numberProfile)
        {
            if (numberProfile.Period == Enums.Period.Day)
            {
                return numberProfile.AggregateValues["CountOutCallsPeakHours"]; 
            }
            return 0;
        }

        static decimal CalculateDataUsage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["TotalDataVolume"];
        }

        static decimal CalculateConsecutiveCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["CountConsecutiveCalls"];
        }

        

    }
}
