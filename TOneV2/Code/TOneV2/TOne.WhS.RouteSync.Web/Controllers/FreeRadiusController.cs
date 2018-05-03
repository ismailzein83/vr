using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.RouteSync.FreeRadius;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "FreeRadius")]
    public class FreeRadiusController : BaseAPIController
    {
        [HttpGet]
        [Route("GetFreeRadiusDataManagerConfigs")]
        public IEnumerable<FreeRadiusDataManagerConfig> GetFreeRadiusDataManagerConfigs()
        {
            FreeRadiusManager manager = new FreeRadiusManager();
            return manager.GetFreeRadiusDataManagerConfigs();
        }
    }
}