using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.RouteSync.FreeRadius;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "IdbDataManagerSettings")]
    public class FreeRadiusPostgresController : BaseAPIController
    {
        [HttpGet]
        [Route("GetFreeRadiusPostgresDataManagerConfigs")]
        public IEnumerable<FreeRadiusPostgresDataManagerConfig> GetFreeRadiusPostgresDataManagerConfigs()
        {
            FreeRadiusPostgresManager manager = new FreeRadiusPostgresManager();
            return manager.GetFreeRadiusPostgresDataManagerConfigs();
        }
    }
}