using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Rules.Web.Controllers;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RouteRule")]
    [JSONWithTypeAttribute]
    public class RouteRuleController : BaseRuleController<RouteRule, RouteRuleDetail, RouteRuleManager>
    {
        [HttpPost]
        [Route("GetFilteredRouteRules")]
        public object GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<RouteRuleQuery> input)
        {
            RouteRuleManager manager = new RouteRuleManager();
            return GetWebResponse(input, manager.GetFilteredRouteRules(input));
        }
                
        [HttpGet]
        [Route("GetCodeCriteriaGroupTemplates")]
        public List<TemplateConfig> GetCodeCriteriaGroupTemplates()
        {
            CodeManager manager = new CodeManager();
            return manager.GetCodeCriteriaGroupTemplates();
        }

        [HttpGet]
        [Route("GetRouteRule")]
        public RouteRule GetRouteRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpPost]
        [Route("AddRouteRule")]
        public InsertOperationOutput<RouteRuleDetail> AddRouteRule(RouteRule input)
        {
            return base.AddRule(input);
        }

        [HttpPost]
        [Route("UpdateRouteRule")]
        public UpdateOperationOutput<RouteRuleDetail> UpdateRouteRule(RouteRule input)
        {
            return base.UpdateRule(input);
        }

        [HttpGet]
        [Route("DeleteRouteRule")]
        public DeleteOperationOutput<RouteRuleDetail> DeleteRouteRule(int ruleId)
        {
            return base.DeleteRule(ruleId);
        }
    }
}