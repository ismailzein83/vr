using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business.PricingRules
{
   public class PricingRuleManager
    {
       public List<TemplateConfig> GetPricingRuleRateTypeTemplates()
       {
           TemplateConfigManager manager = new TemplateConfigManager();
           return manager.GetTemplateConfigurations(TOne.WhS.BusinessEntity.Business.Constants.PricingRuleRateTypeSettingsConfigType);
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
