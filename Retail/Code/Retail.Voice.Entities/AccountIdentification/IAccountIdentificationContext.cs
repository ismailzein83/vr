
namespace Retail.Voice.Entities
{
    public interface IAccountIdentificationContext
    {
        dynamic RawCDR { get; }
        string CallingNumber { get; }
        string CalledNumber { get; }
        bool IsCallingAccountOnNet { set; }
        bool IsCalledAccountOnNet { set; }
        long? CallingAccountId { set; }
        long? CalledAccountId { set; }
    }
}
