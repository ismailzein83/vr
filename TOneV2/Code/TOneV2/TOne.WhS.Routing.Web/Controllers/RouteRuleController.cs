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
        public IEnumerable<RouteRuleSettingsConfig> GetRouteRuleSettingsTemplates()
        {
            RouteRuleManager manager = new RouteRuleManager();
            return manager.GetRouteRuleTypesTemplates();
        }

        [HttpGet]
        [Route("GetRouteRuleCriteriaTemplates")]
        public IEnumerable<RouteRuleCriteriaConfig> GetRouteRuleCriteriaTemplates()
        {
            RouteRuleManager manager = new RouteRuleManager();
            return manager.GetRouteRuleCriteriaTemplates();
        }

        [HttpGet]
        [Route("GetCodeCriteriaGroupTemplates")]
        public IEnumerable<CodeCriteriaGroupConfig> GetCodeCriteriaGroupTemplates()
        {
            CodeManager manager = new CodeManager();
            return manager.GetCodeCriteriaGroupTemplates();
        }

        [Route("GetRouteRuleHistoryDetailbyHistoryId")]
        public RouteRule GetRouteRuleHistoryDetailbyHistoryId(int routeRuleHistoryId)
        {
            RouteRuleManager manager = new RouteRuleManager();
            return manager.GetRouteRuleHistoryDetailbyHistoryId(routeRuleHistoryId);
        }
        [HttpGet]
        [Route("GetRule")]
        public new RouteRule GetRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpPost]
        [Route("BuildLinkedRouteRule")]
        public RouteRule BuildLinkedRouteRule(LinkedRouteRuleInput input)
        {
            RouteRuleManager manager = new RouteRuleManager();
            return manager.BuildLinkedRouteRule(input.RuleId, input.CustomerId, input.Code, input.RouteOptions);
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

        [HttpPost]
        [Route("SetRouteRulesDeleted")]
        public new DeleteOperationOutput<RouteRuleDetail> SetRouteRulesDeleted(RouteRuleDeleteInpute input)
        {
            return base.SetRuleDeleted(input.RuleIds);
        }
    }

    public class LinkedRouteRuleInput
    {
        public int RuleId { get; set; }

        public string Code { get; set; }

        public int? CustomerId { get; set; }

        public List<RouteOption> RouteOptions { get; set; }
    }

    public class RouteRuleDeleteInpute
    {
        public List<int> RuleIds { get; set; }
    }
}