using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Notification.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRNotification")]
    [JSONWithTypeAttribute]
    public class VRNotificationController : BaseAPIController
    {
        [HttpPost]
        [Route("GetUpdatedVRNotifications")]
        public List<VRNotificationDetail> GetUpdatedVRNotifications(VRNotificationUpdateQuery input)
        {
            VRNotificationManager manager = new VRNotificationManager();
            return manager.GetUpdatedVRNotifications(input);
        }

        [HttpPost]
        [Route("GetBeforeIdVRNotifications")]
        public List<VRNotificationDetail> GetBeforeIdVRNotifications(VRNotificationBeforeIdQuery input)
        {
            VRNotificationManager manager = new VRNotificationManager();
            return manager.GetBeforeIdVRNotifications(input);
        }

    }
}