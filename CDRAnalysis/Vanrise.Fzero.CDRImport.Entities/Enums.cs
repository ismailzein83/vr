
namespace Vanrise.Fzero.CDRImport.Entities
{
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

    public enum OperatorType
    {
        Both = 0,
        Mobile = 1,
        PSTN = 2
    };

}
