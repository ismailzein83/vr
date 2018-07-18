using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using NP.IVSwitch.Entities.RouteTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RouteTable")]
    [JSONWithTypeAttribute]
    public class RouteTableController : BaseAPIController
    {
        RouteTableManager _manager = new RouteTableManager();

        [HttpPost]
        [Route("GetFilteredRouteTables")]
        public object GetFilteredRouteTables(Vanrise.Entities.DataRetrievalInput<RouteTableQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredRouteTables(input));
        }

        [HttpPost]
        [Route("AddRouteTable")]
        public Vanrise.Entities.InsertOperationOutput<RouteTableDetails> AddRoute(RouteTableInput routeTableItem)
        {
            return _manager.AddRouteTable(routeTableItem);
        }

    }
}