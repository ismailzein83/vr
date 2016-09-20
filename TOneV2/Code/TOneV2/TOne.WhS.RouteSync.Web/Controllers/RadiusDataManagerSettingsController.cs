using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.RouteSync.Business;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Radius;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RadiusDataManagerSettings")]
    public class RadiusDataManagerSettingsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetRadiusDataManagerExtensionConfigs")]
        public IEnumerable<RadiusDataManagerConfig> GetRadiusDataManagerExtensionConfigs()
        {
            RadiusDataManagersConfigManager manager = new RadiusDataManagersConfigManager();
            return manager.GetRadiusDataManagerExtensionConfigs();
        }
    }
}