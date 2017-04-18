using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Notification.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRNotification")]
    [JSONWithTypeAttribute]
    public class VRNotificationController : BaseAPIController
    {
        VRNotificationManager _manager = new VRNotificationManager();

        VRNotificationTypeManager _typeManager = new VRNotificationTypeManager();

        [HttpPost]
        [Route("GetFirstPageVRNotifications")]
        public object GetFirstPageVRNotifications(VRNotificationFirstPageInput input)
        {
            if (!_typeManager.DoesUserHaveViewAccess(input.NotificationTypeId))
                return GetUnauthorizedResponse();
            return _manager.GetFirstPageVRNotifications(input);
        }

        [HttpPost]
        [Route("GetUpdatedVRNotifications")]
        public object GetUpdatedVRNotifications(VRNotificationUpdateInput input)
        {
            if (!_typeManager.DoesUserHaveViewAccess(input.NotificationTypeId))
                return GetUnauthorizedResponse();
            return _manager.GetUpdatedVRNotifications(input);
        }

        [HttpPost]
        [Route("GetBeforeIdVRNotifications")]
        public object GetBeforeIdVRNotifications(VRNotificationBeforeIdInput input)
        {
            if (!_typeManager.DoesUserHaveViewAccess(input.NotificationTypeId))
                return GetUnauthorizedResponse();
            return _manager.GetBeforeIdVRNotifications(input);
        }
    }
}