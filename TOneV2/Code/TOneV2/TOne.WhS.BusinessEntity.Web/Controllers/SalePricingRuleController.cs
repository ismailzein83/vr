using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business.PricingRules;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
   [RoutePrefix(Constants.ROUTE_PREFIX + "SalePricingRule")]
    public class WhSBE_SalePricingRuleController : Vanrise.Rules.Web.Controllers.BaseRuleController<SalePricingRule, SalePricingRuleManager>
    {
       [HttpPost]
       [Route("GetFilteredSalePricingRules")]
       public object GetFilteredSalePricingRules(Vanrise.Entities.DataRetrievalInput<object> input)
       {
           SalePricingRuleManager manager = new SalePricingRuleManager();
           return GetWebResponse(input, manager.GetFilteredSalePricingRules(input));
       }
    }
}