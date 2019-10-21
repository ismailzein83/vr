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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRDataRecordNotificationType")]
    [JSONWithTypeAttribute]
    public class VRDataRecordNotificationTypeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetDataRecordNotificationActionConfigSettings")]
        public IEnumerable<VRDataRecordNotificationActionConfig> GetDataRecordNotificationActionConfigSettings()
        {
            VRDataRecordNotificationTypeManager manager = new VRDataRecordNotificationTypeManager();
            return manager.GetDataRecordNotificationActionConfigSettings();
        }
    }
}