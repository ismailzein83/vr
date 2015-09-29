using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    public class RouteRuleController : BaseAPIController
    {
        [HttpPost]
        public object GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<RouteRuleQuery> input)
        {
            RouteRuleManager manager = new RouteRuleManager();
            return GetWebResponse(input, manager.GetFilteredRouteRules(input));
        }

        [HttpGet]
        public TOne.Entities.DeleteOperationOutput<object> DeleteRouteRule(int routeRuleId)
        {
            RouteRuleManager manager = new RouteRuleManager();
            return manager.DeleteRouteRule(routeRuleId);
        } 
    }
}