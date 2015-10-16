using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace PSTN.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "NormalizationRule")]
    [JSONWithTypeAttribute]
    public class NormalizationRuleController : Vanrise.Rules.Web.Controllers.BaseRuleController<NormalizationRule, NormalizationRuleDetail, NormalizationRuleManager>
    {
        [HttpPost]
        [Route("GetFilteredNormalizationRules")]
        public object GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return GetWebResponse(input, manager.GetFilteredNormalizationRules(input));
        }

        [HttpGet]
        [Route("GetNormalizationRuleById")]
        public NormalizationRuleDetail GetNormalizationRuleById(int normalizationRuleId)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.GetNormalizationRuleById(normalizationRuleId);
        }

        [HttpGet]
        [Route("GetNormalizationRuleAdjustNumberActionSettingsTemplates")]
        public List<TemplateConfig> GetNormalizationRuleAdjustNumberActionSettingsTemplates()
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.GetNormalizationRuleAdjustNumberActionSettingsTemplates();
        }

        [HttpGet]
        [Route("GetNormalizationRuleSetAreaSettingsTemplates")]
        public List<TemplateConfig> GetNormalizationRuleSetAreaSettingsTemplates()
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.GetNormalizationRuleSetAreaSettingsTemplates();
        }

        [HttpPost]
        [Route("AddNormalizationRule")]
        public InsertOperationOutput<NormalizationRuleDetail> AddNormalizationRule(NormalizationRule normalizationRuleObj)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.AddNormalizationRule(normalizationRuleObj);
        }

        [HttpPost]
        [Route("UpdateNormalizationRule")]
        public UpdateOperationOutput<NormalizationRuleDetail> UpdateNormalizationRule(NormalizationRule normalizationRuleObj)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.UpdateNormalizationRule(normalizationRuleObj);
        }

        [HttpGet]
        [Route("DeleteNormalizationRule")]
        public DeleteOperationOutput<object> DeleteNormalizationRule(int normalizationRuleId)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.DeleteNormalizationRule(normalizationRuleId);
        }
    }
}