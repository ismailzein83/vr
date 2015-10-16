using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business.PricingRules;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
     [RoutePrefix(Constants.ROUTE_PREFIX + "PricingRule")]
    public class WhSBE_PricingRuleController : BaseAPIController
    {
         [HttpGet]
         [Route("GetPricingRuleTODTemplates")]
         public List<TemplateConfig> GetPricingRuleTODTemplates()
         {
             PricingRuleManager manager = new PricingRuleManager();
             return manager.GetPricingRuleTODTemplates(); 
         }
         [HttpGet]
         [Route("GetPricingRuleTariffTemplates")]
         public List<TemplateConfig> GetPricingRuleTariffTemplates()
         {
             PricingRuleManager manager = new PricingRuleManager();
             return manager.GetPricingRuleTariffTemplates(); 
         }

         [HttpGet]
         [Route("GetPricingRuleExtraChargeTemplates")]
         public List<TemplateConfig> GetPricingRuleExtraChargeTemplates()
         {
             PricingRuleManager manager = new PricingRuleManager();
             return manager.GetPricingRuleExtraChargeTemplates(); 
         }

         
         
    }
}