using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.RouteSync.Business;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RouteSyncDefinition")]
    public class RouteSyncDefinitionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetRouteSyncDefinitionsInfo")]
        public IEnumerable<RouteSyncDefinitionInfo> GetRouteSyncDefinitionsInfo()
        {
            RouteSyncDefinitionManager manager = new RouteSyncDefinitionManager();
            return manager.GetRouteSyncDefinitionsInfo();
        }
    }
}