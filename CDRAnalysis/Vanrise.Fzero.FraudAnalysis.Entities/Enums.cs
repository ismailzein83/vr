
namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class Enums
    {
             
        public enum Period
        {
            Day = 1,
            Hour = 6
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

        //public enum CallClass
        //{
        //    ZAINIQ = 1, //1
        //    VAS = 2, //1
        //    INV = 29, //1
        //    INTL = 30, //2
        //    KOREKTEL = 31, //0
        //    ASIACELL = 26, //0
        //    UNKNOWN = 0
        //};

        public enum NetType
        {
            Others = 0, 
            Local = 1, 
            International = 2
        };

              


    }
}
