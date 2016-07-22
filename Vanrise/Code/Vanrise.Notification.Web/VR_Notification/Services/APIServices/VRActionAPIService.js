
(function (appControllers) {

    "use strict";
    VRActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Notification_ModuleConfig'];

    function VRActionAPIService(BaseAPIService, UtilsService, VR_Notification_ModuleConfig) {

        var controllerName = "VRAction";

        function GetVRActionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetVRActionConfigs"));
        }

        return ({
            GetVRActionConfigs: GetVRActionConfigs,
        });
    }

    appControllers.service('VR_Notification_VRActionAPIService', VRActionAPIService);

})(appControllers);