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

namespace TOne.Web.Controllers
{
    public class RoutingController : ApiController
    {
        [HttpGet]
        public string GetTest(string prm)
        {
            return prm + " returned";
        }

        [HttpPost]
        public void SaveRouteRule(RouteRule rule)
        {
            RouteRuleManager manager = new RouteRuleManager();
            manager.SaveRouteRule(rule);

        }
        [HttpGet]
        public List<RouteRuleSummaryModel> GetAllRouteRule()
        {
            //System.Threading.Thread.Sleep(2000);
            RouteRuleManager manager = new RouteRuleManager();
            return Mappers.MapRouteRules(manager.GetAllRouteRule());

        }

        [HttpGet]
        public RouteRule GetRouteRuleDetails(int RouteRuleId)
        {
           
            RouteRuleManager manager = new RouteRuleManager();
            return manager.GetRouteRuleDetails(RouteRuleId);

        }
        
    }
}
