
namespace Retail.Voice.Entities
{
    public interface IInternationalIdentificationContext
    {
        dynamic RawCDR { get; }
        string OtherPartyNumber { get; }
        bool? IsInternational { set; }
    }
}
