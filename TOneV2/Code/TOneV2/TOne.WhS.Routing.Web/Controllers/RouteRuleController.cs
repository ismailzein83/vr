using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;
using Vanrise.Rules.Web.Controllers;
using Vanrise.Web.Base;

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
            return GetWebResponse(input, manager.GetFilteredRouteRules(input), "Route Rules");
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
            return manager.BuildLinkedRouteRule(input.RuleId, input.CustomerId, input.Code, input.SaleZoneId, input.RuleByCountry, input.RouteOptions);
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
        [Route("SetRouteRuleDeleted")]
        public DeleteOperationOutput<RouteRuleDetail> SetRouteRuleDeleted(int ruleId)
        {
            return base.SetRuleDeleted(ruleId);
        }

        [HttpPost]
        [Route("SetRouteRulesDeleted")]
        public DeleteOperationOutput<RouteRuleDetail> SetRouteRulesDeleted(RouteRuleDeleteInpute input)
        {
            return base.SetRulesDeleted(input.RuleIds);
        }

        [HttpGet]
        [Route("GetRouteRuleConfiguration")]
        public RouteRuleConfiguration GetRouteRuleConfiguration()
        {
            return new TOne.WhS.Routing.Business.ConfigManager().GetRouteRuleConfiguration();
        }
    }

    public class LinkedRouteRuleInput
    {
        public int RuleId { get; set; }

        public string Code { get; set; }

        public long? SaleZoneId { get; set; }

        public bool RuleByCountry { get; set; }

        public int? CustomerId { get; set; }

        public List<RouteOption> RouteOptions { get; set; }
    }

    public class RouteRuleDeleteInpute
    {
        public List<int> RuleIds { get; set; }
    }
}