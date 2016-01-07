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

    public enum NetType
    {
        [Description("Others")]
        Others = 0,

        [Description("Local")]
        Local = 1,

        [Description("International")]
        International = 2
    };


    public enum SubscriberType
    {
        [Description("INROAMER")]
        INROAMER = 0,

        [Description("OUTROAMER")]
        OUTROAMER = 1,

        [Description("POSTPAID")]
        POSTPAID = 2,

        [Description("PREPAID")]
        PREPAID = 3,

        [Description("PREROAMER")]
        PREROAMER = 4
    };


}
