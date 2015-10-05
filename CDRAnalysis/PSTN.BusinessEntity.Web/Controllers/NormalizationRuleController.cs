using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class NormalizationRuleController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return GetWebResponse(input, manager.GetFilteredNormalizationRules(input));
        }

        [HttpGet]
        public List<TemplateConfig> GetNormalizationRuleActionBehaviorTemplates()
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.GetNormalizationRuleActionBehaviorTemplates();
        }
    }
}