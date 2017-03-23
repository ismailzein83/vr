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

        //[HttpPost]
        //[Route("GetFirstPageVRNotifications")]
        //public VRNotificationUpdateOutput GetFirstPageVRNotifications(VRNotificationFirstPageInput input)
        //{
        //    return _manager.GetFirstPageVRNotifications(input);
        //}

        [HttpPost]
        [Route("GetUpdatedVRNotifications")]
        public VRNotificationUpdateOutput GetUpdatedVRNotifications(VRNotificationUpdateInput input)
        {
            return _manager.GetUpdatedVRNotifications(input);
        }

        [HttpPost]
        [Route("GetBeforeIdVRNotifications")]
        public List<VRNotificationDetail> GetBeforeIdVRNotifications(VRNotificationBeforeIdInput input)
        {
            return _manager.GetBeforeIdVRNotifications(input);
        }
    }
}