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
    [RoutePrefix(Constants.ROUTE_PREFIX + "RoutingDatabase")]
    public class RoutingDatabaseController : BaseAPIController
    {
        [HttpGet]
        [Route("GetRoutingDatabaseInfo")]
        public IEnumerable<RoutingDatabaseInfo> GetRoutingDatabaseInfo(string serializedFilter)
        {
            RoutingDatabaseInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<RoutingDatabaseInfoFilter>(serializedFilter) : null;
            RoutingDatabaseManager manager = new RoutingDatabaseManager();
            return manager.GetRoutingDatabaseInfo(filter);
        }
    }
}