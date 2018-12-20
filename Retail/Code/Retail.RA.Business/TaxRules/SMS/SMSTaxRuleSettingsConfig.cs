using Vanrise.Entities;

namespace Retail.RA.Business
{
    public class SMSTaxRuleSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "RA_Rules_SMSTaxRuleSettings";
        public string Editor { get; set; }
    }
}