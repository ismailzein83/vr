using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business.PricingRules
{
   public class PricingRuleManager
    {
       public List<TemplateConfig> GetPricingRuleTODTemplates()
       {
           TemplateConfigManager manager = new TemplateConfigManager();
           return manager.GetTemplateConfigurations(TOne.WhS.BusinessEntity.Business.Constants.PricingRuleTODSettingsConfigType);
       }
       public List<TemplateConfig> GetPricingRuleTariffTemplates()
       {
           TemplateConfigManager manager = new TemplateConfigManager();
           return manager.GetTemplateConfigurations(TOne.WhS.BusinessEntity.Business.Constants.PricingRuleTariffSettingsConfigType);
       }
       public List<TemplateConfig> GetPricingRuleExtraChargeTemplates()
       {
           TemplateConfigManager manager = new TemplateConfigManager();
           return manager.GetTemplateConfigurations(TOne.WhS.BusinessEntity.Business.Constants.PricingRuleExtraChargeSettingsConfigType);
       }
    }
}
