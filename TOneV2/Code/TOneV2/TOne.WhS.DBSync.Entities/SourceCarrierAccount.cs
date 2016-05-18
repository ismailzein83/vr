using System;
namespace TOne.WhS.DBSync.Entities
{

    public enum SourceActivationStatus : byte { Inactive = 0, Testing = 1, Active = 2 }

    public enum SourceAccountType : byte { Client = 0, Exchange = 1, Termination = 2 }

    public class SourceCarrierAccount : Vanrise.Entities.EntitySynchronization.ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string NameSuffix { get; set; }

        public short ProfileId { get; set; }

        public string CarrierAccountID { get; set; }

        public string CurrencyID { get; set; }

        public SourceActivationStatus ActivationStatus { get; set; }

        public SourceAccountType AccountType { get; set; }

        public string CarrierMask { get; set; }

    }
}
