using System.Collections.Generic;
using System.Web.Http;
using Retail.RA.Business;
using Retail.RA.Business.TaxRules;
using Vanrise.Web.Base;

namespace Retail.RA.Web.Controller
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RaRules")]
    public class RaRulesController : BaseAPIController
    {
        [HttpGet]
        [Route("GetVoiceTaxRuleTemplates")]
        public IEnumerable<VoiceTaxRuleSettingsConfig> GetVoiceTaxRuleTemplates()
        {
            TaxRuleManager manager = new TaxRuleManager();
            return manager.GetVoiceTaxRuleTemplates();
        }

        [HttpGet]
        [Route("GetSMSTaxRuleTemplates")]
        public IEnumerable<SMSTaxRuleSettingsConfig> GetSMSTaxRuleTemplates()
        {
            TaxRuleManager manager = new TaxRuleManager();
            return manager.GetSMSTaxRuleTemplates();
        }
    }
}