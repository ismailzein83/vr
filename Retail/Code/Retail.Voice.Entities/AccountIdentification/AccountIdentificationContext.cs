
namespace Retail.Voice.Entities
{
    public class AccountIdentificationContext : IAccountIdentificationContext
    {
        public dynamic RawCDR { get; set; }
        public string CallingNumber { get; set; }
        public string CalledNumber { get; set; }
        public long? CallingAccountId { get; set; }
        public long? CalledAccountId { get; set; }
    }
}
