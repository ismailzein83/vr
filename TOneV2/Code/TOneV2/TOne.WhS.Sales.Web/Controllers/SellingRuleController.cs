using System;
using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SellingRule")]
    public class SellingRuleController : BaseAPIController
    {
        [HttpGet]
        [Route("GetSellingRuleThresholdTemplates")]
        public IEnumerable<SellingRuleThresholdSettings> GetSellingRuleThresholdTemplates()
        {
            SellingRuleManager manager = new SellingRuleManager();
            return manager.GetSellingRuleThresholdTemplates();
        }

        [HttpGet]
        [Route("GetSellingRuleExecutionTemplates")]
        public IEnumerable<SellingRuleActionSettings> GetSellingRuleActionTemplates()
        {
            SellingRuleManager manager = new SellingRuleManager();
            return manager.GetSellingRuleActionTemplates();
        }
    }
}