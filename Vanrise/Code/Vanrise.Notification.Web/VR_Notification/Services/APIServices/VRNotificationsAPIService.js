
(function (appControllers) {

    "use strict";
    VRNotificationsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Notification_ModuleConfig'];

    function VRNotificationsAPIService(BaseAPIService, UtilsService, VR_Notification_ModuleConfig) {

        var controllerName = "VRNotification";

        function GetUpdatedVRNotifications(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetUpdatedVRNotifications'), input);
        }

        function GetBeforeIdVRNotifications(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetBeforeIdVRNotifications'), input);
        }

        return ({
            GetUpdatedVRNotifications: GetUpdatedVRNotifications,
            GetBeforeIdVRNotifications: GetBeforeIdVRNotifications
        });
    }

    appControllers.service('VR_Notification_VRNotificationsAPIService', VRNotificationsAPIService);

})(appControllers);