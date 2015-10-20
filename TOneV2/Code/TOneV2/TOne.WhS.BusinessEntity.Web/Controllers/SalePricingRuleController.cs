using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Business.PricingRules;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Rules.Web.Controllers;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
   [JSONWithTypeAttribute]
   [RoutePrefix(Constants.ROUTE_PREFIX + "SalePricingRule")]
    public class WhSBE_SalePricingRuleController : BaseRuleController<SalePricingRule,SalePricingRuleDetail, SalePricingRuleManager>
    {
       [HttpPost]
       [Route("GetFilteredSalePricingRules")]
       public object GetFilteredSalePricingRules(Vanrise.Entities.DataRetrievalInput<object> input)
       {
           SalePricingRuleManager manager = new SalePricingRuleManager();
           return GetWebResponse(input, manager.GetFilteredSalePricingRules(input));
       }

       [HttpPost]
       [Route("AddRule")]
       public new InsertOperationOutput<SalePricingRuleDetail> AddRule(SalePricingRule input)
       {
           return base.AddRule(input);
       }
       [HttpGet]
       [Route("GetRule")]
       public new SalePricingRule GetRule(int ruleId)
       {
           return base.GetRule(ruleId);
       }

       [HttpGet]
       [Route("DeleteRule")]
       public new DeleteOperationOutput<SalePricingRuleDetail> DeleteRule(int ruleId)
       {
           return base.DeleteRule(ruleId);
       }

    }
}