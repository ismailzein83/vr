using Vanrise.Entities.EntitySynchronization;
namespace TOne.WhS.DBSync.Entities
{

    public enum SourceActivationStatus : byte { Inactive = 0, Testing = 1, Active = 2 }

    public enum SourceAccountType : byte { Client = 0, Exchange = 1, Termination = 2 }

    public class SourceCarrierAccount : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string NameSuffix { get; set; }

        public short ProfileId { get; set; }

        public short ServicesFlag { get; set; }

        public string CurrencyId { get; set; }

        public short? CustomerGMTTime { get; set; }

        public short? GMTTime { get; set; }

        public SourceActivationStatus ActivationStatus { get; set; }

        public SourceAccountType AccountType { get; set; }

        public string CarrierMask { get; set; }

    }
}
