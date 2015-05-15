
namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class Enums
    {

        public enum Criteria 
        { 
            RatioIncomingCallsvsOutgoingCalls = 1,
            CountofDistinctDestinations = 2,
            CountOutgoingCalls = 3,
            CountofTotalBTSPerMSISDN = 4,
            TotalOriginatedVolume = 5,
            CountofTotalIMEIPerMSISDN = 6,
            RatioAverageIncomingDurationvsAverageOutgoingDuration = 7,
            RatioOffNetOriginatedCallsvsOnNetOriginatedCalls = 8,
            CountofDailyActiveHours = 9,
            DistinctDestinationofNightCalls = 10,
            VoiceOnlyServiceUsage = 11,
            RatioofDistinctDestinationvsTotalNumberofCalls = 12,
            RatioInternationalOriginatedvsOutgoingCalls = 13,
            CountofOutgoingDuringPeakHours = 14,
            DataUsage = 15
        };


        public enum Period
        {
            Day = 6,
            Hour = 1
        };


        public enum CallType
        {
            OutgoingVoiceCall = 1,
            IncomingVoiceCall = 2,
            CallForward = 29,
            IncomingSms = 30,
            OutgoingSms = 31,
            RoamingCallForward = 26
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
