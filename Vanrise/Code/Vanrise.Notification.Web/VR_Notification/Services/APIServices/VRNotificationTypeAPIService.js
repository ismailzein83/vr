
(function (appControllers) {

	"use strict";
	VRNotificationTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Notification_ModuleConfig'];

	function VRNotificationTypeAPIService(BaseAPIService, UtilsService, VR_Notification_ModuleConfig) {

		var controllerName = "VRNotificationType";

		function GetVRNotificationTypeDefinitionConfigSettings() {
			return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetVRNotificationTypeDefinitionConfigSettings"));
		}

		function GetVRNotificationTypeSettingsInfo(serializedFilter) {
			return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetVRNotificationTypeSettingsInfo"), {
				serializedFilter: serializedFilter
			});
		}

		function GetNotificationTypeSettings(notificationTypeId) {
			return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetNotificationTypeSettings"), {
				notificationTypeId: notificationTypeId
			});
		}

		function GetVRNotificationTypeLegendData(notificationTypeId) {
			return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetVRNotificationTypeLegendData'), {
				notificationTypeId: notificationTypeId
			});
		}

		return ({
			GetVRNotificationTypeDefinitionConfigSettings: GetVRNotificationTypeDefinitionConfigSettings,
			GetVRNotificationTypeSettingsInfo: GetVRNotificationTypeSettingsInfo,
			GetNotificationTypeSettings: GetNotificationTypeSettings,
			GetVRNotificationTypeLegendData: GetVRNotificationTypeLegendData
		});
	}

	appControllers.service('VR_Notification_VRNotificationTypeAPIService', VRNotificationTypeAPIService);

})(appControllers);