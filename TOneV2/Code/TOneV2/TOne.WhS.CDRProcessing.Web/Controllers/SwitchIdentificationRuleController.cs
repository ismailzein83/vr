using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.CDRProcessing.Business;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Entities;
using Vanrise.Rules.Web.Controllers;
using Vanrise.Web.Base;

namespace TOne.WhS.CDRProcessing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SwitchIdentificationRule")]
    public class SwitchIdentificationRuleController : BaseRuleController<SwitchIdentificationRule, SwitchIdentificationRuleDetail, SwitchIdentificationRuleManager>
    {
        [HttpPost]
        [Route("GetFilteredSwitchIdentificationRules")]
        public object GetFilteredSwitchIdentificationRules(Vanrise.Entities.DataRetrievalInput<SwitchIdentificationRuleQuery> input)
        {
            SwitchIdentificationRuleManager manager = new SwitchIdentificationRuleManager();
            return GetWebResponse(input, manager.GetFilteredSwitchIdentificationRules(input));
        }

        [HttpPost]
        [Route("AddRule")]
        public new InsertOperationOutput<SwitchIdentificationRuleDetail> AddRule(SwitchIdentificationRule input)
        {
            return base.AddRule(input);
        }
        [HttpGet]
        [Route("GetRule")]
        public new SwitchIdentificationRule GetRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpGet]
        [Route("DeleteRule")]
        public new DeleteOperationOutput<SwitchIdentificationRuleDetail> DeleteRule(int ruleId)
        {
            return base.DeleteRule(ruleId);
        }

        [HttpPost]
        [Route("UpdateRule")]
        public new UpdateOperationOutput<SwitchIdentificationRuleDetail> UpdateRule(SwitchIdentificationRule input)
        {
            return base.UpdateRule(input);
        }
    }
}