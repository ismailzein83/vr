
(function (appControllers) {

    "use strict";
    VRActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Notification_ModuleConfig'];

    function VRActionAPIService(BaseAPIService, UtilsService, VR_Notification_ModuleConfig) {

        var controllerName = "VRAction";

        function GetVRActionConfigs(extensionType) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetVRActionConfigs"), {
                extensionType: extensionType
            });
        }

        return ({
            GetVRActionConfigs: GetVRActionConfigs,
        });
    }

    appControllers.service('VR_Notification_VRActionAPIService', VRActionAPIService);

})(appControllers);