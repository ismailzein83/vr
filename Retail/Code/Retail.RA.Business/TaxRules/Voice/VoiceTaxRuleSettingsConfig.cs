using Vanrise.Entities;

namespace Retail.RA.Business
{
    public class VoiceTaxRuleSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "RA_Rules_VoiceTaxRuleSettings";
        public string Editor { get; set; }
    }
}