(function (appControllers) {

    "use strict";
    textManipulationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function textManipulationAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controller = 'TextManipulation';

        function GetTextManipulationActionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetTextManipulationActionSettingsConfigs"));
        }

        return ({
            GetTextManipulationActionSettingsConfigs: GetTextManipulationActionSettingsConfigs,
        });
    }

    appControllers.service('VRCommon_TextManipulationAPIService', textManipulationAPIService);

})(appControllers);