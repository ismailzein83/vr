using Vanrise.Entities;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public enum ReceiverIdentification { Receiver = 0, ReceiverIn = 1, ReceiverOut = 2, NormalizedReceiver = 3, NormalizedReceiverIn = 4, NormalizedReceiverOut = 5 }

    public class SMSImportSettings : SettingData
    {
        public SwitchSMSMappingConfiguration SwitchSMSMappingConfiguration { get; set; }
    }

    public class SwitchSMSMappingConfiguration
    {
        public ReceiverIdentification? GeneralIdentification { get; set; }

        public ReceiverIdentification? CustomerIdentification { get; set; }

        public ReceiverIdentification? SupplierIdentification { get; set; }

        public ReceiverIdentification? MobileNetworkIdentification { get; set; }
    }
}
