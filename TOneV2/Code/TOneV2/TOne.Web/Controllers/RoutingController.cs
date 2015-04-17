using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.LCR.Entities;
using TOne.LCR.Business;
using TOne.Web.Models;
using TOne.Web.ModelMappers;
using System.Web.Http.Controllers;

namespace TOne.Web.Controllers
{
    [CustomJSONTypeHandlingAttribure]
    public class RoutingController : ApiController
    {
        [HttpGet]
        public string GetTest(string prm)
        {
            return prm + " returned";
        }

        [HttpPost]
        public RouteRuleSummaryModel SaveRouteRule(RouteRule rule)
        {
            System.Threading.Thread.Sleep(1000);
            RouteRuleManager manager = new RouteRuleManager();
            return Mappers.MapRouteRule(manager.SaveRouteRule(rule));
          

        }
        [HttpGet]
        public IEnumerable<RouteRuleSummaryModel> GetAllRouteRule( int pageNumber, int pageSize)
        {
            System.Threading.Thread.Sleep(1000);

            RouteRuleManager manager = new RouteRuleManager();
            IEnumerable<RouteRuleSummaryModel> rows = Mappers.MapRouteRules(manager.GetAllRouteRule());
            rows = rows.Skip((pageNumber) * pageSize).Take(pageSize);
            return rows;
           
           // return Mappers.MapRouteRules(manager.GetAllRouteRule());

        }

        [HttpPost]
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
        public RouteRule GetRouteRuleDetails(int RouteRuleId)
        {
           
            RouteRuleManager manager = new RouteRuleManager();
            return manager.GetRouteRuleDetails(RouteRuleId);

        }
        
    }

    #region Argument Classes
    public class GetFilteredRouteRulesInput
    {
        public List<string> RuleTypes { get; set; }
        public List<int> ZoneIds { get; set; }
        public string Code { get; set; }
        public List<string> CustomerIds { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    #endregion

    public class CustomJSONTypeHandlingAttribure : Attribute, IControllerConfiguration
    {
      public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Formatters.Clear();
            var jsonFormatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
            jsonFormatter.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
            controllerSettings.Formatters.Add(jsonFormatter);
        }
    }

}
