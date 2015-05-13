using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            dictionary.Add(1, new CriteriaDefinition() { CriteriaId = 1, Description = "Ratio_Incoming_Calls_vs_Outgoing_Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = Ratio_Incoming_Calls_vs_Outgoing_Calls });
            dictionary.Add(2, new CriteriaDefinition() { CriteriaId = 2, Description = "Count_of_Distinct_Destinations", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = Count_of_Distinct_Destinations });
            dictionary.Add(3, new CriteriaDefinition() { CriteriaId = 3, Description = "Count_outgoing_calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = Count_outgoing_calls });
            dictionary.Add(4, new CriteriaDefinition() { CriteriaId = 4, Description = "Count_of_Total_BTS_Per_MSISDN", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = Count_of_Total_BTS_Per_MSISDN });
            dictionary.Add(5, new CriteriaDefinition() { CriteriaId = 5, Description = "Total_Originated_Volume", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = Total_Originated_Volume });
            dictionary.Add(6, new CriteriaDefinition() { CriteriaId = 6, Description = "Count_of_Total_IMEI_Per_MSISDN", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = Count_of_Total_IMEI_Per_MSISDN });
            dictionary.Add(7, new CriteriaDefinition() { CriteriaId = 7, Description = "Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration });
            dictionary.Add(8, new CriteriaDefinition() { CriteriaId = 8, Description = "Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls });
            dictionary.Add(9, new CriteriaDefinition() { CriteriaId = 9, Description = "Count_of_daily_active_hours", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = Count_of_daily_active_hours });
            dictionary.Add(10, new CriteriaDefinition() { CriteriaId = 10, Description = "Distinct_Destination_of_Night_Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = Distinct_Destination_of_Night_Calls });
            dictionary.Add(11, new CriteriaDefinition() { CriteriaId = 11, Description = "Voice_Only_Service_Usage", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = Voice_Only_Service_Usage });
            dictionary.Add(12, new CriteriaDefinition() { CriteriaId = 12, Description = "Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual, Expression = Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls });
            dictionary.Add(13, new CriteriaDefinition() { CriteriaId = 13, Description = "Ratio_International_Originated_Vs_Outgoing_Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = Ratio_International_Originated_Vs_Outgoing_Calls });
            dictionary.Add(14, new CriteriaDefinition() { CriteriaId = 14, Description = "Count_of_outgoing_during_peak_hours", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = Count_of_outgoing_during_peak_hours });
            dictionary.Add(15, new CriteriaDefinition() { CriteriaId = 15, Description = "Data_Usage", CompareOperator = CriteriaCompareOperator.LessThanorEqual, Expression = Data_Usage });

            return dictionary; 
        }

        
        // Funcs
        static decimal Ratio_Incoming_Calls_vs_Outgoing_Calls(NumberProfile numberProfile)
        {
            if (numberProfile.AggregateValues["TotalInVolume"] != 0 && numberProfile.AggregateValues["TotalOutVolume"] != 0)
                return (numberProfile.AggregateValues["CountInCalls"] / numberProfile.AggregateValues["CountOutCalls"]);
            else
                return 0;
        }

        static decimal Count_of_Distinct_Destinations(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["DiffOutputNumb"];
        }

        static decimal Count_outgoing_calls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["CountOutCalls"];
        }

        static decimal Count_of_Total_BTS_Per_MSISDN(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["TotalBTS"];
        }

        static decimal Total_Originated_Volume(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["TotalOutVolume"];
        }

        static decimal Count_of_Total_IMEI_Per_MSISDN(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["TotalIMEI"];
        }

        static decimal Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration(NumberProfile numberProfile)
        {
            if (numberProfile.AggregateValues["CallInDurAvg"] != 0 && numberProfile.AggregateValues["CallOutDurAvg"] != 0)
                return (numberProfile.AggregateValues["CallInDurAvg"] / numberProfile.AggregateValues["CallOutDurAvg"]);
            else
                return 0;
        }

        static decimal Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls(NumberProfile numberProfile)
        {
            if (numberProfile.AggregateValues["CountInOffNets"] != 0 && numberProfile.AggregateValues["CountOutOffNets"] != 0)
                return (numberProfile.AggregateValues["CountInOffNets"] / numberProfile.AggregateValues["CountOutOffNets"]);
            else
                return 0;
        }

        static decimal Count_of_daily_active_hours(NumberProfile numberProfile)
        {
            // not developed yet
            return 0;
        }

        static decimal Distinct_Destination_of_Night_Calls(NumberProfile numberProfile)
        {
            if (numberProfile.PeriodId == (int)Enums.Period.Day)
            {
                // not developed yet
            }
            else if (numberProfile.PeriodId == (int)Enums.Period.Hour)
            {
                // not developed yet
            }
            return 0;

        }

        static decimal Voice_Only_Service_Usage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["CountOutSMSs"];
        }

        static decimal Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls(NumberProfile numberProfile)
        {
            if (numberProfile.AggregateValues["DiffOutputNumb"] != 0 && numberProfile.AggregateValues["CountOutCalls"] != 0)
                return (numberProfile.AggregateValues["DiffOutputNumb"] / numberProfile.AggregateValues["CountOutCalls"]);
            else
                return 0;
        }

        static decimal Ratio_International_Originated_Vs_Outgoing_Calls(NumberProfile numberProfile)
        {
            if (numberProfile.AggregateValues["CountInInters"] != 0 && numberProfile.AggregateValues["CountOutCalls"] != 0)
                return (numberProfile.AggregateValues["CountInInters"] / numberProfile.AggregateValues["CountOutCalls"]);
            else
                return 0;
        }

        static decimal Count_of_outgoing_during_peak_hours(NumberProfile numberProfile)
        {
            if (numberProfile.PeriodId == (int)Enums.Period.Day)
            {
                // not developed yet
            }
            return 0;
        }

        static decimal Data_Usage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["TotalDataVolume"];
        }

        

    }
}
