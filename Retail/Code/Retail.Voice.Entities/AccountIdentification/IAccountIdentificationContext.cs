
namespace Retail.Voice.Entities
{
    public interface IAccountIdentificationContext
    {
        dynamic RawCDR { get; }
        string CallingNumber { get; }
        string CalledNumber { get; }
        long? CallingAccountId { set; }
        long? CalledAccountId { set; }
    }
}
