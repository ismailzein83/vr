using NetworkProvision.Business;
using NetworkProvision.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;

namespace NetworkProvision.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "HandlerType")]
    [JSONWithTypeAttribute]
    public class NetworkProvisionHandlerTypeController : BaseAPIController
    {
        NetworkProvisionHandlerTypeManager _manager = new NetworkProvisionHandlerTypeManager();

        [HttpGet]
        [Route("GetHandlerTypeExtendedSettingsConfigs")]
        public IEnumerable<NetworkProvisionHandlerTypeExtendedSettingsConfig> GetHandlerTypeExtendedSettingsConfigs()
        {
            return _manager.GetHandlerTypeExtendedSettingsConfigs();
        }

        [HttpPost]
        [Route("TryCompileCustomCode")]
        public CustomCodeCompilationOutput TryCompileCustomCode(CustomCodeNetworkProvisionHandlerType customCode)
        {
            return _manager.TryCompileCustomCode(customCode);
        }

    }
}