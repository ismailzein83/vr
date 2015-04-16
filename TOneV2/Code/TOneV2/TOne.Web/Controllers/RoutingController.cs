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

        [HttpGet]
        public RouteRule GetRouteRuleDetails(int RouteRuleId)
        {
           
            RouteRuleManager manager = new RouteRuleManager();
            return manager.GetRouteRuleDetails(RouteRuleId);

        }
        
    }

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
