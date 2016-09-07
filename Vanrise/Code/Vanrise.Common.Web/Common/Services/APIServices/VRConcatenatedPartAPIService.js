
(function (appControllers) {

    "use strict";
    VRConcatenatedPartAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRConcatenatedPartAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "VRConcatenatedPart";

        function GetConcatenatedPartSettingsConfigs(extensionType) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetConcatenatedPartSettingsConfigs"), {
                extensionType: extensionType
            });
        }

        return ({
            GetConcatenatedPartSettingsConfigs: GetConcatenatedPartSettingsConfigs,
        });
    }

    appControllers.service('VRCommon_VRConcatenatedPartAPIService', VRConcatenatedPartAPIService);

})(appControllers);