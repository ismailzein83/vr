(function (appControllers) {

    "use strict";
    VRDataRecordNotificationTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Notification_ModuleConfig'];

    function VRDataRecordNotificationTypeAPIService(BaseAPIService, UtilsService, VR_Notification_ModuleConfig) {

        var controllerName = "VRDataRecordNotificationType";

        function GetDataRecordNotificationActionConfigSettings() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetDataRecordNotificationActionConfigSettings"));
        }

        return ({
            GetDataRecordNotificationActionConfigSettings: GetDataRecordNotificationActionConfigSettings
        });
    }

    appControllers.service('VR_Notification_VRDataRecordNotificationTypeAPIService', VRDataRecordNotificationTypeAPIService);

})(appControllers);