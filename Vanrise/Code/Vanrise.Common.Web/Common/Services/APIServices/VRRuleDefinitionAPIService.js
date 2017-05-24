(function (appControllers) {

    "use strict";

    VRRuleDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRRuleDefinitionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "VRRuleDefinition";

        function GetVRRuleDefinitionExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRRuleDefinitionExtendedSettingsConfigs"));
        }

        function GetVRRuleDefinitionsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRRuleDefinitionsInfo"), {
                serializedFilter: serializedFilter
            });
        }


        return ({
            GetVRRuleDefinitionExtendedSettingsConfigs: GetVRRuleDefinitionExtendedSettingsConfigs,
            GetVRRuleDefinitionsInfo: GetVRRuleDefinitionsInfo
        });
    }

    appControllers.service('VRCommon_VRRuleDefinitionAPIService', VRRuleDefinitionAPIService);

})(appControllers);