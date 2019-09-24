using Vanrise.Entities;

namespace Retail.Billing.Entities
{
    public class RetailBillingChargeTypeExtendedSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Retail_Billing_ChargeTypeExtendedSettings";

        public string DefinitionEditor { get; set; }
    }
}