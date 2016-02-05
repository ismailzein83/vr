using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleManager
    {
        public List<TemplateConfig> GetPricingRuleRateTypeTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.PricingRuleRateTypeSettingsConfigType);
        }
        public List<TemplateConfig> GetPricingRuleTariffTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.PricingRuleTariffSettingsConfigType);
        }
        public List<TemplateConfig> GetPricingRuleExtraChargeTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.PricingRuleExtraChargeSettingsConfigType);
        }
    }
}
