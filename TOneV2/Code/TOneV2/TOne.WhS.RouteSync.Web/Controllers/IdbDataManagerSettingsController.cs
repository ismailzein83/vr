using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.RouteSync.Idb;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "IdbDataManagerSettings")]
    public class IdbDataManagerSettingsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetIdbDataManagerExtensionConfigs")]
        public IEnumerable<IdbDataManagerConfig> GetIdbDataManagerExtensionConfigs() 
        {
            IdbDataManagersConfigManager manager = new IdbDataManagersConfigManager();
            return manager.GetIdbDataManagerExtensionConfigs();
        }
    }
}