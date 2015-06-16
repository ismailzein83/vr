﻿
namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class Enums
    {
             
        public enum Period
        {
            Hour = 1,
            Day = 2
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

       
        public enum NetType
        {
            Others = 0, 
            Local = 1, 
            International = 2
        };

              


    }
}
