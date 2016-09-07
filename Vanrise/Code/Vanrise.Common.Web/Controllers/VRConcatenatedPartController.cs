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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRConcatenatedPart")]
    [JSONWithTypeAttribute]
    public class VRConcatenatedPartController:BaseAPIController
    {
        VRConcatenatedPartManager _manager = new VRConcatenatedPartManager();
        [HttpGet]
        [Route("GetConcatenatedPartSettingsConfigs")]
        public IEnumerable<ConcatenatedPartSettingsConfig> GetConcatenatedPartSettingsConfigs(string extensionType)
        {
            return _manager.GetConcatenatedPartSettingsConfigs(extensionType);
        }
    }

    public class VRConcatenatedPartManager
    {
        public IEnumerable<ConcatenatedPartSettingsConfig> GetConcatenatedPartSettingsConfigs(string extensionType)
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<ConcatenatedPartSettingsConfig>(extensionType);
        }
    }
}