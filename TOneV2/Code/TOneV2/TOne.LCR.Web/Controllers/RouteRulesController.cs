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
        public TOne.Entities.InsertOperationOutput<RouteRuleSummaryModel> InsertRouteRule(RouteRule rule)
        {
            RouteRuleManager manager = new RouteRuleManager();
            TOne.Entities.InsertOperationOutput<RouteRule> insertedRule = manager.InsertRouteRule(rule);
            TOne.Entities.InsertOperationOutput<RouteRuleSummaryModel> routeRuleSummaryModel = new TOne.Entities.InsertOperationOutput<RouteRuleSummaryModel>();
            routeRuleSummaryModel.InsertedObject = Mappers.MapRouteRule(insertedRule.InsertedObject);
            routeRuleSummaryModel.Result = insertedRule.Result;
            return routeRuleSummaryModel;
        }

        [HttpPost]
        public TOne.Entities.UpdateOperationOutput<RouteRuleSummaryModel> UpdateRouteRule(RouteRule rule)
        {
            RouteRuleManager manager = new RouteRuleManager();
            TOne.Entities.UpdateOperationOutput<RouteRule> updatedRule = manager.UpdateRouteRule(rule);
            TOne.Entities.UpdateOperationOutput<RouteRuleSummaryModel> routeRuleSummaryModel = new TOne.Entities.UpdateOperationOutput<RouteRuleSummaryModel>();
            routeRuleSummaryModel.UpdatedObject = Mappers.MapRouteRule(updatedRule.UpdatedObject);
            routeRuleSummaryModel.Result = updatedRule.Result;
            return routeRuleSummaryModel;
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