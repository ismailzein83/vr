(function (appControllers) {

    "use strict";
    VRNotificationViewAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Notification_ModuleConfig'];

    function VRNotificationViewAPIService(BaseAPIService, UtilsService, VR_Notification_ModuleConfig) {
        var controllerName = 'VRNotificationView';

        function GetVRNotificationViewSettings(viewId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetVRNotificationViewSettings"), { viewId: viewId });
        }
        return ({
            GetVRNotificationViewSettings: GetVRNotificationViewSettings
        });
    }

    appControllers.service('VR_Notification_VRNotificationViewAPIService', VRNotificationViewAPIService);

})(appControllers);