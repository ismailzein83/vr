
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
        return ({
            GetVRNotificationTypeDefinitionConfigSettings: GetVRNotificationTypeDefinitionConfigSettings,
            GetVRNotificationTypeSettingsInfo: GetVRNotificationTypeSettingsInfo
        });
    }

    appControllers.service('VR_Notification_VRNotificationTypeAPIService', VRNotificationTypeAPIService);

})(appControllers);