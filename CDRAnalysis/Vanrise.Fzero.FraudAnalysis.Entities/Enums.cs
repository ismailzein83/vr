using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class Enums
    {

        public enum Criteria 
        { 
            Ratio_Incoming_Calls_vs_Outgoing_Calls = 1,
            Count_of_Distinct_Destinations = 2,
            Count_outgoing_calls = 3,
            Count_of_Total_BTS_Per_MSISDN = 4,
            Total_Originated_Volume = 5,
            Count_of_Total_IMEI_Per_MSISDN = 6,
            Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration = 7,
            Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls = 8,
            Count_of_daily_active_hours = 9,
            Distinct_Destination_of_Night_Calls = 10,
            Voice_Only_Service_Usage = 11,
            Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls = 12,
            Ratio_International_Originated_Vs_Outgoing_Calls = 13,
            Count_of_outgoing_during_peak_hours = 14,
            Data_Usage = 15
        };


        public enum Period
        {
            Day = 6,
            Hour = 1
        };


        public enum CallType
        {
            outgoingVoiceCall = 1,
            incomingVoiceCall = 2,
            callForward = 29,
            incomingSms = 30,
            outgoingSms = 31,
            roamingCallForward = 26
        };

        public enum CallClass
        {
            ZAINIQ = 1, //1
            VAS = 2, //1
            INV = 29, //1
            INTL = 30, //2
            KOREKTEL = 31, //0
            ASIACELL = 26 //0
        };

        public enum EntityType
        {
            SubscriberNumber = 1,
            Destination = 2
        };


    }
}
