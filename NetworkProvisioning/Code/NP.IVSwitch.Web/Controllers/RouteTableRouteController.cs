using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using NP.IVSwitch.Entities.RouteTableRoute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;
namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RouteTableRoute")]
    [JSONWithTypeAttribute]
    public class RouteTableRouteController : BaseAPIController
    {
        RouteTableRouteManager _manager = new RouteTableRouteManager();
        [HttpPost]
        [Route("GetFilteredRouteTableRoutes")]
        public object GetFilteredRouteTableRoutes(Vanrise.Entities.DataRetrievalInput<RouteTableRouteQuery> input)
        {
            return _manager.GetFilteredRouteTableRoutes(input);
        }

        [HttpPost]
        [Route("AddRouteTableRoutes")]
        public InsertOperationOutput<RouteTableRouteDetails> AddRouteTableRoutes(RouteTableRoutesToAdd routeTableRouteItems)
        {
            return _manager.AddRouteTableRoutes(routeTableRouteItems);
        }

        [HttpPost]
        [Route("UpdateRouteTableRoute")]
        public UpdateOperationOutput<RouteTableRouteDetails> UpdateRouteTableRoute(RouteTableRoutesToEdit routeTableItem)
        {
            //return null;
            return _manager.UpdateRouteTableRoute(routeTableItem);
        }
        [HttpGet]
        [Route("DeleteRouteTableRoute")]
        public DeleteOperationOutput<object> DeleteRouteTableRoute(int routeTableId, string destination)
        {
            return _manager.DeleteRouteTableRoutes(routeTableId, destination);
        }
        [HttpGet]
        [Route("GetRouteTableRoutesOptions")]
        public RouteTableRoutesRuntimeEditor GetRouteTableRoutesOptions(int routeTableId, string destination)
        {

            return _manager.GetRouteTableRoutesOptions(routeTableId, destination);
        }

    }
}