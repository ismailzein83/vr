using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business.PricingRules;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Rules.Web.Controllers;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
   [RoutePrefix(Constants.ROUTE_PREFIX + "SalePricingRule")]
   [JSONWithTypeAttribute]
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
       [Route("AddSalePricingRule")]
       public InsertOperationOutput<SalePricingRuleDetail> AddSalePricingRule(SalePricingRule input)
       {
           return AddRule(input);
       }
    }
}