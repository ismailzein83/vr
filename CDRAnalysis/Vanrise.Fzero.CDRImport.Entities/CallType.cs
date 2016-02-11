using System.ComponentModel;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public enum CallType
    {
        [Description("Not Defined")]
        NotDefined = 0,

        [Description("Outgoing Voice Call")]
        OutgoingVoiceCall = 1,

        [Description("Incoming Voice Call")]
        IncomingVoiceCall = 2,

        [Description("Call Forward")]
        CallForward = 29,

        [Description("Incoming Sms")]
        IncomingSms = 30,

        [Description("Outgoing Sms")]
        OutgoingSms = 31,

        [Description("Roaming Call Forward")]
        RoamingCallForward = 26
    };


}
