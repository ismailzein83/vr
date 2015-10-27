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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierIdentificationRule")]
    public class SupplierIdentificationRuleController : BaseRuleController<SupplierIdentificationRule, SupplierIdentificationRuleDetail, SupplierIdentificationRuleManager>
    {
        [HttpPost]
        [Route("GetFilteredSupplierIdentificationRules")]
        public object GetFilteredSupplierIdentificationRules(Vanrise.Entities.DataRetrievalInput<SupplierIdentificationRuleQuery> input)
        {
            SupplierIdentificationRuleManager manager = new SupplierIdentificationRuleManager();
            return GetWebResponse(input, manager.GetFilteredSupplierIdentificationRules(input));
        }

        [HttpPost]
        [Route("AddRule")]
        public new InsertOperationOutput<SupplierIdentificationRuleDetail> AddRule(SupplierIdentificationRule input)
        {
            return base.AddRule(input);
        }
        [HttpGet]
        [Route("GetRule")]
        public new SupplierIdentificationRule GetRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpGet]
        [Route("DeleteRule")]
        public new DeleteOperationOutput<SupplierIdentificationRuleDetail> DeleteRule(int ruleId)
        {
            return base.DeleteRule(ruleId);
        }

        [HttpPost]
        [Route("UpdateRule")]
        public new UpdateOperationOutput<SupplierIdentificationRuleDetail> UpdateRule(SupplierIdentificationRule input)
        {
            return base.UpdateRule(input);
        }
    }
}