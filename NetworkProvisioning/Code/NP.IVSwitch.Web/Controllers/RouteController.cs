using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Route")]
    [JSONWithTypeAttribute]
    public class RouteController : BaseAPIController
    {
        RouteManager _manager = new RouteManager();

        [HttpPost]
        [Route("GetFilteredRoutes")]
        public object GetFilteredRoutes(Vanrise.Entities.DataRetrievalInput<RouteQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredRoutes(input));
        }
         

        [HttpGet]
        [Route("GetRoute")]
        public Route GetRoute(int routeId)
        {
            return _manager.GetRoute(routeId);
        }

        [HttpPost]
        [Route("AddRoute")]
        public Vanrise.Entities.InsertOperationOutput<RouteDetail> AddRoute(Route routeItem)
        {
            return _manager.AddRoute(routeItem);
        }

        [HttpPost]
        [Route("UpdateRoute")]
        public Vanrise.Entities.UpdateOperationOutput<RouteDetail> UpdateRoute(Route routeItem)
        {
            return _manager.UpdateRoute(routeItem);
        }
    }
}