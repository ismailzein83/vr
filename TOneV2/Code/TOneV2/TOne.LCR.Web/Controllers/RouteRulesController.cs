using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.LCR.Entities;
using TOne.LCR.Business;
using TOne.LCR.Web.Models;
using TOne.LCR.Web.ModelMappers;
using System.Web.Http.Controllers;

namespace TOne.LCR.Web.Controllers
{
    [CustomJSONTypeHandlingAttribure]
    public class RouteRulesController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public RouteRuleSummaryModel SaveRouteRule(RouteRule rule)
        {
            RouteRuleManager manager = new RouteRuleManager();
            return Mappers.MapRouteRule(manager.SaveRouteRule(rule));
        }

        [HttpPost]
        public RouteRuleSummaryModel UpdateRouteRule(RouteRule rule)
        {
            RouteRuleManager manager = new RouteRuleManager();
            return Mappers.MapRouteRule(manager.UpdateRouteRule(rule));
        }

        [HttpPost]
        public IEnumerable<RouteRuleSummaryModel> GetFilteredRouteRules(GetFilteredRoutingRulesInput input)
        {
            RouteRuleManager manager = new RouteRuleManager();
            var rules = manager.GetFilteredRouteRules(input.Filter.RuleTypes, input.Filter.ZoneIds, input.Filter.Code, input.Filter.CustomerIds, input.FromRow, input.ToRow);
            if (rules != null)
                return Mappers.MapRouteRules(rules);
            else
                return null;
        }

        [HttpGet]
        public RouteRule GetRouteRuleDetails(int RouteRuleId)
        {
            RouteRuleManager manager = new RouteRuleManager();
            return manager.GetRouteRuleDetails(RouteRuleId);
        }

    }

    #region Argument Classes
    public class GetFilteredRoutingRulesInput
    {
        public RoutingRulesFilter Filter { get; set; }
        public int FromRow { get; set; }
        public int ToRow { get; set; }
        public bool IsDescending { get; set; }
    }

    #endregion

}