using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Rules.Pricing;
using Vanrise.Web.Base;

namespace Vanrise.Rules.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "PricingRule")]
    public class VR_PricingRuleController : BaseAPIController
    {
        [HttpGet]
        [Route("GetPricingRuleRateTypeTemplates")]
        public IEnumerable<PricingRuleRateTypeItemSettingsConfig> GetPricingRuleRateTypeTemplates()
        {
            PricingRuleManager manager = new PricingRuleManager();
            return manager.GetPricingRuleRateTypeTemplates();
        }
        [HttpGet]
        [Route("GetPricingRuleTariffTemplates")]
        public IEnumerable<PricingRuleTariffSettingsConfig> GetPricingRuleTariffTemplates()
        {
            PricingRuleManager manager = new PricingRuleManager();
            return manager.GetPricingRuleTariffTemplates();
        }

        [HttpGet]
        [Route("GetPricingRuleExtraChargeTemplates")]
        public IEnumerable<PricingRuleExtraChargeActionSettingsConfig> GetPricingRuleExtraChargeTemplates()
        {
            PricingRuleManager manager = new PricingRuleManager();
            return manager.GetPricingRuleExtraChargeTemplates();
        }

        [HttpGet]
        [Route("GetPricingRuleRateValueTemplates")]
        public IEnumerable<PricingRuleRateValueSettingsConfig> GetPricingRuleRateValueTemplates()
        {
            PricingRuleManager manager = new PricingRuleManager();
            return manager.GetPricingRuleRateValueTemplates();
        }
    }
}