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
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SetCustomerRule")]
    public class SetCustomerRuleController : BaseRuleController<SetCustomerRule, SetCustomerRuleDetail, SetCustomerRuleManager>
    {
        [HttpPost]
        [Route("GetFilteredSetCustomerRules")]
        public object GetFilteredSetCustomerRules(Vanrise.Entities.DataRetrievalInput<SetCustomerRuleQuery> input)
        {
            SetCustomerRuleManager manager = new SetCustomerRuleManager();
            return GetWebResponse(input, manager.GetFilteredSetCustomerRules(input));
        }

        [HttpPost]
        [Route("AddRule")]
        public new InsertOperationOutput<SetCustomerRuleDetail> AddRule(SetCustomerRule input)
        {
            return base.AddRule(input);
        }
        [HttpGet]
        [Route("GetRule")]
        public new SetCustomerRule GetRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpGet]
        [Route("DeleteRule")]
        public new DeleteOperationOutput<SetCustomerRuleDetail> DeleteRule(int ruleId)
        {
            return base.DeleteRule(ruleId);
        }

        [HttpPost]
        [Route("UpdateRule")]
        public new UpdateOperationOutput<SetCustomerRuleDetail> UpdateRule(SetCustomerRule input)
        {
            return base.UpdateRule(input);
        }
    }
}