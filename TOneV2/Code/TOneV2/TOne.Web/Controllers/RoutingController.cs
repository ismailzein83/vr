using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.LCR.Entities;
using TOne.LCR.Business;

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
    }
}
