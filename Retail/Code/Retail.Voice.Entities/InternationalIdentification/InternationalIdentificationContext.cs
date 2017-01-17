
namespace Retail.Voice.Entities
{
    public class InternationalIdentificationContext : IInternationalIdentificationContext
    {
        public dynamic RawCDR { get; set; }
        public string OtherPartyNumber { get; set; }
        public bool? IsInternational { get; set; }
    }
}
