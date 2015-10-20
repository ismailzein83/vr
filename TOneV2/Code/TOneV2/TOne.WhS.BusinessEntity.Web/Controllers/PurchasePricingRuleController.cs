using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Rules.Web.Controllers;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "PurchasePricingRule")]
    public class PurchasePricingRuleController : BaseRuleController<PurchasePricingRule, PurchasePricingRuleDetail, PurchasePricingRuleManager>
    {
        [HttpPost]
        [Route("GetFilteredPurchasePricingRules")]
        public object GetFilteredPurchasePricingRules(Vanrise.Entities.DataRetrievalInput<PurchasePricingRuleQuery> input)
        {
            PurchasePricingRuleManager manager = new PurchasePricingRuleManager();
            return GetWebResponse(input, manager.GetFilteredPurchasePricingRules(input));
        }

        [HttpPost]
        [Route("AddRule")]
        public new InsertOperationOutput<PurchasePricingRuleDetail> AddRule(PurchasePricingRule input)
        {
            return base.AddRule(input);
        }
        [HttpGet]
        [Route("GetRule")]
        public new PurchasePricingRule GetRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpGet]
        [Route("DeleteRule")]
        public new DeleteOperationOutput<PurchasePricingRuleDetail> DeleteRule(int ruleId)
        {
            return base.DeleteRule(ruleId);
        }
        [HttpPost]
        [Route("UpdateRule")]
        public new UpdateOperationOutput<PurchasePricingRuleDetail> UpdateRule(PurchasePricingRule input)
        {
            return base.UpdateRule(input);
        }
    }
}