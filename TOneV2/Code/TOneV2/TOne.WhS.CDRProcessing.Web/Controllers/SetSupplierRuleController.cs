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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SetSupplierRule")]
    public class SetSupplierRuleController : BaseRuleController<SetSupplierRule, SetSupplierRuleDetail, SetSupplierRuleManager>
    {
        [HttpPost]
        [Route("GetFilteredSetSupplierRules")]
        public object GetFilteredSetSupplierRules(Vanrise.Entities.DataRetrievalInput<SetSupplierRuleQuery> input)
        {
            SetSupplierRuleManager manager = new SetSupplierRuleManager();
            return GetWebResponse(input, manager.GetFilteredSetSupplierRules(input));
        }

        [HttpPost]
        [Route("AddRule")]
        public new InsertOperationOutput<SetSupplierRuleDetail> AddRule(SetSupplierRule input)
        {
            return base.AddRule(input);
        }
        [HttpGet]
        [Route("GetRule")]
        public new SetSupplierRule GetRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpGet]
        [Route("DeleteRule")]
        public new DeleteOperationOutput<SetSupplierRuleDetail> DeleteRule(int ruleId)
        {
            return base.DeleteRule(ruleId);
        }

        [HttpPost]
        [Route("UpdateRule")]
        public new UpdateOperationOutput<SetSupplierRuleDetail> UpdateRule(SetSupplierRule input)
        {
            return base.UpdateRule(input);
        }
    }
}