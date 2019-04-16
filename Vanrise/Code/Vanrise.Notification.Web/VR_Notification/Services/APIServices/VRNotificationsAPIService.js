
(function (appControllers) {

    "use strict";

    VRNotificationsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Notification_ModuleConfig'];

    function VRNotificationsAPIService(BaseAPIService, UtilsService, VR_Notification_ModuleConfig) {

        var controllerName = "VRNotification";

        function GetFirstPageVRNotifications(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetFirstPageVRNotifications'), input);
        }

        function ExportVRNotifications(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'ExportVRNotifications'), input, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        function GetUpdatedVRNotifications(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetUpdatedVRNotifications'), input);
        }

        function GetBeforeIdVRNotifications(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetBeforeIdVRNotifications'), input);
        }

        return ({
            GetFirstPageVRNotifications: GetFirstPageVRNotifications,
            ExportVRNotifications: ExportVRNotifications,
            GetUpdatedVRNotifications: GetUpdatedVRNotifications,
            GetBeforeIdVRNotifications: GetBeforeIdVRNotifications
        });
    }

    appControllers.service('VR_Notification_VRNotificationsAPIService', VRNotificationsAPIService);

})(appControllers);