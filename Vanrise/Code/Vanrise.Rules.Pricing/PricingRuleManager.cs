using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Rules.Pricing;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleManager
    {
        public IEnumerable<PricingRuleRateTypeItemSettingsConfig> GetPricingRuleRateTypeTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<PricingRuleRateTypeItemSettingsConfig>(PricingRuleRateTypeItemSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<PricingRuleTariffSettingsConfig> GetPricingRuleTariffTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<PricingRuleTariffSettingsConfig>(PricingRuleTariffSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<PricingRuleExtraChargeActionSettingsConfig> GetPricingRuleExtraChargeTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<PricingRuleExtraChargeActionSettingsConfig>(PricingRuleExtraChargeActionSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<PricingRuleRateValueSettingsConfig> GetPricingRuleRateValueTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<PricingRuleRateValueSettingsConfig>(PricingRuleRateValueSettingsConfig.EXTENSION_TYPE);
        }
    }
}
