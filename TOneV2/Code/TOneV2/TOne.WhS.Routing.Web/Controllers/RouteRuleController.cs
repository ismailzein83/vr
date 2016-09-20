using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Rules.Web.Controllers;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Web.Controllers
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
        [Route("GetRouteRuleSettingsTemplates")]
        public List<TemplateConfig> GetRouteRuleSettingsTemplates()
        {
            RouteRuleManager manager = new RouteRuleManager();
            return manager.GetRouteRuleTypesTemplates();
        }

        [HttpGet]
        [Route("GetCodeCriteriaGroupTemplates")]
        public IEnumerable<CodeCriteriaGroupConfig> GetCodeCriteriaGroupTemplates()
        {
            CodeManager manager = new CodeManager();
            return manager.GetCodeCriteriaGroupTemplates();
        }

        [HttpGet]
        [Route("GetRule")]
        public new RouteRule GetRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpPost]
        [Route("AddRule")]
        public new InsertOperationOutput<RouteRuleDetail> AddRule(RouteRule input)
        {
            return base.AddRule(input);
        }

        [HttpPost]
        [Route("UpdateRule")]
        public new UpdateOperationOutput<RouteRuleDetail> UpdateRule(RouteRule input)
        {
            return base.UpdateRule(input);
        }

        [HttpGet]
        [Route("DeleteRule")]
        public new DeleteOperationOutput<RouteRuleDetail> DeleteRule(int ruleId)
        {
            return base.DeleteRule(ruleId);
        }
    }
}