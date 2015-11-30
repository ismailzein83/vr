using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RPRoute")]
    public class RPRouteController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredRPRoutes")]
        public object GetFilteredRPRoutes(Vanrise.Entities.DataRetrievalInput<RPRouteQuery> input)
        {
            RPRouteManager manager = new RPRouteManager();
            return GetWebResponse(input, manager.GetFilteredRPRoutes(input));
        }

        [HttpGet]
        [Route("GetRPRouteOptionSupplier")]
        public RPRouteOptionSupplierDetail GetRPRouteOptionSupplier(int routingProductId, long saleZoneId, int supplierId)
        {
            RPRouteManager manager = new RPRouteManager();
            return manager.GetRPRouteOptionSupplier(routingProductId, saleZoneId, supplierId);
        }
    }
}