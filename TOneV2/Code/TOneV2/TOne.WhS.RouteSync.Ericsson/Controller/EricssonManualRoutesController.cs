using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.RouteSync.Ericsson.Business;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Ericsson.Controller
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "EricssonManualRoutes")]
    [JSONWithTypeAttribute]
    public class EricssonManualRoutesController : BaseAPIController
    {
        EricssonManualRoutesManager _manager = new EricssonManualRoutesManager();

        [HttpGet]
        [Route("GetManualRouteActionTypeExtensionConfigs")]
        public IEnumerable<EricssonManualRouteActionConfig> GetManualRouteActionTypeExtensionConfigs()
        {
            return _manager.GetManualRouteActionTypeExtensionConfigs();
        }
        [HttpGet]
        [Route("GetManualRouteDestinationsTypeExtensionConfigs")]
        public IEnumerable<EricssonManualRouteDestinationsConfig> GetManualRouteDestinationsTypeExtensionConfigs()
        {
            return _manager.GetManualRouteDestinationsTypeExtensionConfigs();
        }
        [HttpGet]
        [Route("GetManualRouteOriginationsTypeExtensionConfigs")]
        public IEnumerable<EricssonManualRouteOriginationsConfig> GetManualRouteOriginationsTypeExtensionConfigs()
        {
            return _manager.GetManualRouteOriginationsTypeExtensionConfigs();
        }
        [HttpGet]
        [Route("GetSpecialRoutingTypeExtensionConfigs")]
        public IEnumerable<EricssonSpecialRoutingSettingConfig> GetSpecialRoutingTypeExtensionConfigs()
        {
            return _manager.GetSpecialRoutingTypeExtensionConfigs();
        }
    }
}