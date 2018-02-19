using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Rules.Web.Controllers;
using Vanrise.Web.Base;

namespace PSTN.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "NormalizationRule")]
    [JSONWithTypeAttribute]
    public class NormalizationRuleController : BaseRuleController<NormalizationRule, NormalizationRuleDetail, NormalizationRuleManager>
    {
        [HttpPost]
        [Route("GetFilteredNormalizationRules")]
        public object GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input)
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return GetWebResponse(input, manager.GetFilteredNormalizationRules(input));
        }

        [HttpGet]
        [Route("GetRule")]
        public new NormalizationRule GetRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpPost]
        [Route("AddRule")]
        public new InsertOperationOutput<NormalizationRuleDetail> AddRule(NormalizationRule input)
        {
            return base.AddRule(input);
        }

        [HttpPost]
        [Route("UpdateRule")]
        public new UpdateOperationOutput<NormalizationRuleDetail> UpdateRule(NormalizationRule input)
        {
            return base.UpdateRule(input);
        }

        //[HttpGet]
        //[Route("DeleteRule")]
        //public new DeleteOperationOutput<NormalizationRuleDetail> DeleteRule(int ruleId)
        //{
        //    return base.DeleteRule(ruleId);
        //}

        [HttpGet]
        [Route("GetNormalizationRuleAdjustNumberActionSettingsTemplates")]
        public IEnumerable<AdjustNumberActionConfig> GetNormalizationRuleAdjustNumberActionSettingsTemplates()
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.GetNormalizationRuleAdjustNumberActionSettingsTemplates();
        }

        [HttpGet]
        [Route("GetNormalizationRuleSetAreaSettingsTemplates")]
        public IEnumerable<SetAreaConfig> GetNormalizationRuleSetAreaSettingsTemplates()
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.GetNormalizationRuleSetAreaSettingsTemplates();
        }
    }
}