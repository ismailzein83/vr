using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace PSTN.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    public class NormalizationRuleController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return GetWebResponse(input, manager.GetFilteredNormalizationRules(input));
        }

        [HttpGet]
        public NormalizationRule GetNormalizationRuleById(int normalizationRuleId)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.GetNormalizationRuleById(normalizationRuleId);
        }

        [HttpGet]
        public List<TemplateConfig> GetNormalizationRuleActionBehaviorTemplates()
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.GetNormalizationRuleActionBehaviorTemplates();
        }

        [HttpGet]
        public List<TemplateConfig> GetNormalizationRuleAdjustNumberActionSettingsTemplates()
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.GetNormalizationRuleAdjustNumberActionSettingsTemplates();
        }

        [HttpPost]
        public InsertOperationOutput<NormalizationRuleDetail> AddNormalizationRule(NormalizationRule normalizationRuleObj)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.AddNormalizationRule(normalizationRuleObj);
        }

        [HttpPost]
        public UpdateOperationOutput<NormalizationRuleDetail> UpdateNormalizationRule(NormalizationRule normalizationRuleObj)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.UpdateNormalizationRule(normalizationRuleObj);
        }

        [HttpGet]
        public DeleteOperationOutput<object> DeleteNormalizationRule(int normalizationRuleId)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.DeleteNormalizationRule(normalizationRuleId);
        }
    }
}