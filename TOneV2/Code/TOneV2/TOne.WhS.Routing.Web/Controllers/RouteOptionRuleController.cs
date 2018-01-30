using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;
using Vanrise.Rules.Web.Controllers;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RouteOptionRule")]
    [JSONWithTypeAttribute]

    public class RouteOptionRuleController : BaseRuleController<RouteOptionRule, RouteOptionRuleDetail, RouteOptionRuleManager>
    {
        [HttpPost]
        [Route("GetFilteredRouteOptionRules")]
        public object GetFilteredRouteOptionRules(Vanrise.Entities.DataRetrievalInput<RouteOptionRuleQuery> input)
        {
            RouteOptionRuleManager manager = new RouteOptionRuleManager();
            return GetWebResponse(input, manager.GetFilteredRouteOptionRules(input));
        }

        [HttpGet]
        [Route("GetRouteOptionRuleSettingsTemplates")]
        public IEnumerable<RouteOptionRuleConfig> GetRouteOptionRuleSettingsTemplates()
        {
            RouteOptionRuleManager manager = new RouteOptionRuleManager();
            return manager.GetRouteOptionRuleTypesTemplates();
        }

        [HttpGet]
        [Route("GetRouteOptionRuleSettingsTemplatesByProcessType")]
        public IEnumerable<ProcessRouteOptionRuleConfig> GetRouteOptionRuleSettingsTemplatesByProcessType(RoutingProcessType routingProcessType)
        {
            RouteOptionRuleManager manager = new RouteOptionRuleManager();
            return manager.GetRouteOptionRuleSettingsTemplatesByProcessType(routingProcessType);
        }
       
        [Route("GetRouteOptionRuleHistoryDetailbyHistoryId")]
        public RouteOptionRule GetRouteOptionRuleHistoryDetailbyHistoryId(int routeOPtionRuleHistoryId)
        {
            RouteOptionRuleManager manager = new RouteOptionRuleManager();
            return manager.GetRouteOptionRuleHistoryDetailbyHistoryId(routeOPtionRuleHistoryId);
        }

        [HttpGet]
        [Route("GetRuleEditorRuntime")]
        public RouteOptionRuleEditorRuntime GetRuleEditorRuntime(int ruleId)
        {
            RouteOptionRuleManager manager = new RouteOptionRuleManager();
            return manager.GetRuleEditorRuntime(ruleId);
        }

        [HttpGet]
        [Route("GetRule")]
        public new RouteOptionRule GetRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpPost]
        [Route("BuildLinkedRouteOptionRule")]
        public RouteOptionRule BuildLinkedRouteOptionRule(LinkedRouteOptionRuleInput input)
        {
            RouteOptionRuleManager manager = new RouteOptionRuleManager();
            return manager.BuildLinkedRouteOptionRule(input.RuleId, input.CustomerId, input.Code, input.SupplierId, input.SupplierZoneId);
        }

        [HttpPost]
        [Route("AddRule")]
        public new InsertOperationOutput<RouteOptionRuleDetail> AddRule(RouteOptionRule input)
        {
            return base.AddRule(input);
        }

        [HttpPost]
        [Route("UpdateRule")]
        public new UpdateOperationOutput<RouteOptionRuleDetail> UpdateRule(RouteOptionRule input)
        {
            return base.UpdateRule(input);
        }

        [HttpGet]
        [Route("SetRouteOptionRuleDeleted")]
        public DeleteOperationOutput<RouteOptionRuleDetail> SetRouteOptionRuleDeleted(int ruleId)
        {
            return base.SetRuleDeleted(ruleId);
        }

        [HttpPost]
        [Route("SetRouteOptionRulesDeleted")]
        public DeleteOperationOutput<RouteOptionRuleDetail> SetRouteOptionRulesDeleted(RouteOptionsRulesDeleteInpute input)
        {
            return base.SetRulesDeleted(input.RouteOptionsIds);
        }
    }

    public class LinkedRouteOptionRuleInput
    {
        public int? RuleId { get; set; }

        public string Code { get; set; }

        public int? CustomerId { get; set; }

        public int? SupplierId { get; set; }

        public long? SupplierZoneId { get; set; }
    }

    public class RouteOptionsRulesDeleteInpute
    {
        public List<int> RouteOptionsIds { get; set; }
    }
}