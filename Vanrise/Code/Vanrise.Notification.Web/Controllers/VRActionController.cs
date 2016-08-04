using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Notification.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRAction")]
    [JSONWithTypeAttribute]
    public class VRActionController : BaseAPIController
    {
        VRActionManager _manager = new VRActionManager();
        [HttpGet]
        [Route("GetVRActionConfigs")]
        public IEnumerable<VRActionConfig> GetVRActionConfigs(string extensionType)
        {
            return _manager.GetVRActionConfigs(extensionType);
        }
    }
}