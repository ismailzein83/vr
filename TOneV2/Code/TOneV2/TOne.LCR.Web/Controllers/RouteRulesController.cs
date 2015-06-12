using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TOne.LCR.Business;
using TOne.LCR.Entities;
using TOne.LCR.Web.ModelMappers;
using TOne.LCR.Web.Models;

namespace TOne.LCR.Web.Controllers
{
    [TOne.Entities.SecureController("/Routing/RouteRuleManagement")]
    [CustomJSONTypeHandlingAttribure]
    public class RouteRulesController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public RouteRuleSummaryModel SaveRouteRule(RouteRule rule)
        {
            System.Threading.Thread.Sleep(1000);
            RouteRuleManager manager = new RouteRuleManager();
            return Mappers.MapRouteRule(manager.SaveRouteRule(rule));
        }

        [HttpPost]
        [TOne.Entities.SecureAction("View")]
        public IEnumerable<RouteRuleSummaryModel> GetFilteredRouteRules(GetFilteredRouteRulesInput filter)
        {
            RouteRuleManager manager = new RouteRuleManager();
            var rules = manager.GetFilteredRouteRules(filter.RuleTypes, filter.ZoneIds, filter.Code, filter.CustomerIds, filter.PageNumber, filter.PageSize);
            if (rules != null)
                return Mappers.MapRouteRules(rules);
            else
                return null;
        }

        [HttpGet]
        [TOne.Entities.SecureAction("View")]
        public RouteRule GetRouteRuleDetails(int RouteRuleId)
        {
            RouteRuleManager manager = new RouteRuleManager();
            return manager.GetRouteRuleDetails(RouteRuleId);
        }

    }
}