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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRPop3MessageFilter")]
    [JSONWithTypeAttribute]
    public class VRPop3MessageFilterController : BaseAPIController
    {
        [HttpGet]
        [Route("GetVRPop3MessageFilterConfigs")]
        public IEnumerable<VRPop3MessageFilterConfig> GetVRPop3MessageFilterConfigs()
        {
            var configManager = new ExtensionConfigurationManager();
            return configManager.GetExtensionConfigurations<VRPop3MessageFilterConfig>(VRPop3MessageFilterConfig.EXTENSION_TYPE);
        }
    }
}