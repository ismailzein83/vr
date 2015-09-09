
namespace Vanrise.Fzero.CDRImport.Entities
{
   

    

    public enum CallTypeEnum
    {
        NotDefined = 0,
        OutgoingVoiceCall = 1,
        IncomingVoiceCall = 2,
        CallForward = 29,
        IncomingSms = 30,
        OutgoingSms = 31,
        RoamingCallForward = 26
    };

    public enum NetTypeEnum
    {
        Others = 0,
        Local = 1,
        International = 2
    };

}
