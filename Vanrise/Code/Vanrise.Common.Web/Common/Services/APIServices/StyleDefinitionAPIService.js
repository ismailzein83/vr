
(function (appControllers) {

    "use strict";
    StyleDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function StyleDefinitionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "StyleDefinition";

        function GetFilteredStyleDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredStyleDefinitions'), input);
        }

        function GetStyleDefinition(StyleDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetStyleDefinition'), {
                StyleDefinitionId: StyleDefinitionId
            });
        }

        function AddStyleDefinition(StyleDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddStyleDefinition'), StyleDefinitionItem);
        }

        function UpdateStyleDefinition(StyleDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateStyleDefinition'), StyleDefinitionItem);
        }

        function GetStyleFormatingExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetStyleFormatingExtensionConfigs"));
        }


        return ({
            GetFilteredStyleDefinitions: GetFilteredStyleDefinitions,
            GetStyleDefinition: GetStyleDefinition,
            AddStyleDefinition: AddStyleDefinition,
            UpdateStyleDefinition: UpdateStyleDefinition,
            GetStyleFormatingExtensionConfigs: GetStyleFormatingExtensionConfigs
        });
    }

    appControllers.service('VRCommon_StyleDefinitionAPIService', StyleDefinitionAPIService);

})(appControllers);