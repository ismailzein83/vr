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
    [RoutePrefix(Constants.ROUTE_PREFIX + "RouteTable")]
    [JSONWithTypeAttribute]
    public class RouteTableController : BaseAPIController
    {

        RouteTableManager _manager = new RouteTableManager();

        [HttpGet]
        [Route("GetRouteTablesInfo")]
        public IEnumerable<RouteTableInfo> GetRouteTablesInfo(string filter = null)
        {
            RouteTableFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<RouteTableFilter>(filter) : null;
            return _manager.GetRouteTablesInfo(deserializedFilter);
        }
    }
}