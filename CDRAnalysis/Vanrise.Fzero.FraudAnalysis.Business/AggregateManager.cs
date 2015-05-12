using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AggregateManager
    {
        public Dictionary<int, AggregateDefinition> GetAggregateDefinitions()
        {
            Dictionary<int, AggregateDefinition> dictionary = new Dictionary<int, AggregateDefinition>();
            dictionary.Add(1, new AggregateDefinition()
            {
                Name = "countOutCalls",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.callType == (int)Enums.CallType.outgoingVoiceCall);
                })
            });

            //dictionary.Add(1, new AggregateDefinition() { Aggregation= AggregateId = 1, Description = "Ratio_Incoming_Calls_vs_Outgoing_Calls", CompareOperator= AggregateCompareOperator.LessThanorEqual });
            //dictionary.Add(2, new AggregateDefinition() { AggregateId = 2, Description = "Count_of_Distinct_Destinations", CompareOperator = AggregateCompareOperator.GreaterThanorEqual });
            //dictionary.Add(3, new AggregateDefinition() { AggregateId = 3, Description = "Count_outgoing_calls", CompareOperator = AggregateCompareOperator.GreaterThanorEqual });
            //dictionary.Add(4, new AggregateDefinition() { AggregateId = 4, Description = "Count_of_Total_BTS_Per_MSISDN", CompareOperator = AggregateCompareOperator.LessThanorEqual });
            //dictionary.Add(5, new AggregateDefinition() { AggregateId = 5, Description = "Total_Originated_Volume", CompareOperator = AggregateCompareOperator.GreaterThanorEqual });
            //dictionary.Add(6, new AggregateDefinition() { AggregateId = 6, Description = "Count_of_Total_IMEI_Per_MSISDN", CompareOperator = AggregateCompareOperator.GreaterThanorEqual });
            //dictionary.Add(7, new AggregateDefinition() { AggregateId = 7, Description = "Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration", CompareOperator = AggregateCompareOperator.LessThanorEqual });
            //dictionary.Add(8, new AggregateDefinition() { AggregateId = 8, Description = "Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls", CompareOperator = AggregateCompareOperator.LessThanorEqual });
            //dictionary.Add(9, new AggregateDefinition() { AggregateId = 9, Description = "Count_of_daily_active_hours", CompareOperator = AggregateCompareOperator.LessThanorEqual });
            //dictionary.Add(10, new AggregateDefinition() { AggregateId = 10, Description = "Distinct_Destination_of_Night_Calls", CompareOperator = AggregateCompareOperator.GreaterThanorEqual });
            //dictionary.Add(11, new AggregateDefinition() { AggregateId = 11, Description = "Voice_Only_Service_Usage", CompareOperator = AggregateCompareOperator.LessThanorEqual });
            //dictionary.Add(12, new AggregateDefinition() { AggregateId = 12, Description = "Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls", CompareOperator = AggregateCompareOperator.GreaterThanorEqual });
            //dictionary.Add(13, new AggregateDefinition() { AggregateId = 13, Description = "Ratio_International_Originated_Vs_Outgoing_Calls", CompareOperator = AggregateCompareOperator.LessThanorEqual });
            //dictionary.Add(14, new AggregateDefinition() { AggregateId = 14, Description = "Count_of_outgoing_during_peak_hours", CompareOperator = AggregateCompareOperator.LessThanorEqual });
            //dictionary.Add(15, new AggregateDefinition() { AggregateId = 15, Description = "Data_Usage", CompareOperator = AggregateCompareOperator.LessThanorEqual });

            return dictionary; 
        }

    }
}
