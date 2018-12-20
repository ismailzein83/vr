using System.Collections.Generic;
using Vanrise.Common.Business;

namespace Retail.RA.Business.TaxRules
{
    public class TaxRuleManager
    {
        public IEnumerable<VoiceTaxRuleSettingsConfig> GetVoiceTaxRuleTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<VoiceTaxRuleSettingsConfig>(VoiceTaxRuleSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<SMSTaxRuleSettingsConfig> GetSMSTaxRuleTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SMSTaxRuleSettingsConfig>(SMSTaxRuleSettingsConfig.EXTENSION_TYPE);
        }
    }
}
