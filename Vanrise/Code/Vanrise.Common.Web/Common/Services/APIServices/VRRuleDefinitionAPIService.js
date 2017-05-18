(function (appControllers) {

    "use strict";

    VRRuleDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRRuleDefinitionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "VRRuleDefinition";

        function GetVRRuleDefinitionExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRRuleDefinitionExtendedSettingsConfigs"));
        }


        return ({
            GetVRRuleDefinitionExtendedSettingsConfigs: GetVRRuleDefinitionExtendedSettingsConfigs
        });
    }

    appControllers.service('VRCommon_VRRuleDefinitionAPIService', VRRuleDefinitionAPIService);

})(appControllers);