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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRNotificationType")]
    [JSONWithTypeAttribute]
    public class VRNotificationTypeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetVRNotificationTypeDefinitionConfigSettings")]
        public IEnumerable<VRNotificationTypeConfig> GetVRNotificationTypeDefinitionConfigSettings()
        {
            VRNotificationTypeManager manager = new VRNotificationTypeManager();
            return manager.GetVRNotificationTypeDefinitionConfigSettings();
        }

        [HttpGet]
        [Route("GetVRNotificationTypeSettingsInfo")]
        public IEnumerable<VRNotificationTypeSettingsInfo> GetVRNotificationTypeSettingsInfo(string serializedFilter = null)
        {
            VRNotificationTypeManager manager = new VRNotificationTypeManager();
            VRNotificationTypeSettingsFilter deserializedFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<VRNotificationTypeSettingsFilter>(serializedFilter) : null;
            return manager.GetVRNotificationTypeSettingsInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetNotificationTypeSettings")]
        public VRNotificationTypeSettings GetNotificationTypeSettings(Guid notificationTypeId)
        {
            VRNotificationTypeManager manager = new VRNotificationTypeManager();
            return manager.GetNotificationTypeSettings(notificationTypeId);
        }


		[HttpGet]
		[Route("GetVRNotificationTypeLegendData")]
		public List<VRAlertLevelWithStyleSettings> GetVRNotificationTypeLegendData(Guid notificationTypeId)
		{
			VRNotificationTypeManager manager = new VRNotificationTypeManager();
			return manager.GetVRNotificationTypeLegendData(notificationTypeId);
		}
	}
}