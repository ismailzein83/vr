using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRTile")]
    [JSONWithTypeAttribute]

    public class VRTileController : BaseAPIController
    {
        VRTileManager _manager = new VRTileManager();

        [HttpGet]
        [Route("GetTileExtendedSettingsConfigs")]
        public IEnumerable<VRTileExtendedSettingsConfig> GetTileExtendedSettingsConfigs()
        {
            return _manager.GetTileExtendedSettingsConfigs();
        }
    }
}