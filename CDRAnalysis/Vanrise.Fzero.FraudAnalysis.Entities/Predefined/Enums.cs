
namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class Enums
    {
             
        public enum Period
        {
            Hourly = 1,
            Daily = 2
        };



        public enum SuspicionLevel
        {
            Suspicious = 2,
            Highly_Suspicious = 3,
            Fraud = 4
        };

        public enum CallType
        {
            NotDefined = 0,
            OutgoingVoiceCall = 1,
            IncomingVoiceCall = 2,
            CallForward = 29,
            IncomingSms = 30,
            OutgoingSms = 31,
            RoamingCallForward = 26
        };

       
        public enum NetType
        {
            Others = 0, 
            Local = 1, 
            International = 2
        };

              


    }
}
