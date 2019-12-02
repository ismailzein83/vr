using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RemoteCommunicatorSettings")]
    [JSONWithTypeAttribute]
    public class VRCommon_RemoteCommunicatorController : BaseAPIController
    {
        [HttpGet]
        [Route("GetRemoteCommunicatorSettingsConfigs")]
        public IEnumerable<RemoteCommunicatorSettingsConfig> GetRemoteCommunicatorSettingsConfigs()
        {
            RemoteCommunicatorSettingsManager manager = new RemoteCommunicatorSettingsManager();
            return manager.GetRemoteCommunicatorSettingsConfigs();
        }
    }
}