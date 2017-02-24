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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRNotificationView")]
    [JSONWithTypeAttribute]
    public class VRNotificationViewController : BaseAPIController
    {
        [HttpGet]
        [Route("GetVRNotificationViewSettings")]
        public IEnumerable<VRNotificationTypeConfig> GetVRNotificationViewSettings(Guid viewId)
        {
            VRNotificationViewManager manager = new VRNotificationViewManager();
            return null;// manager.GetVRNotificationViewSettings(viewId);
        }
    }
}