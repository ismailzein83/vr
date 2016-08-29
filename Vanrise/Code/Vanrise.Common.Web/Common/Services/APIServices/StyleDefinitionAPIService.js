
(function (appControllers) {

    "use strict";
    StyleDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function StyleDefinitionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "StyleDefinition";


        function GetFilteredStyleDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredStyleDefinitions'), input);
        }

        function GetStyleDefinition(styleDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetStyleDefinition'), {
                StyleDefinitionId: styleDefinitionId
            });
        }

        function AddStyleDefinition(styleDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddStyleDefinition'), styleDefinitionItem);
        }

        function UpdateStyleDefinition(styleDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateStyleDefinition'), styleDefinitionItem);
        }

        function GetStyleFormatingExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetStyleFormatingExtensionConfigs"));
        }

        function GetStyleDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetStyleDefinitionsInfo"), {
                filter: filter
            });
        }


        return ({
            GetFilteredStyleDefinitions: GetFilteredStyleDefinitions,
            GetStyleDefinition: GetStyleDefinition,
            AddStyleDefinition: AddStyleDefinition,
            UpdateStyleDefinition: UpdateStyleDefinition,
            GetStyleFormatingExtensionConfigs: GetStyleFormatingExtensionConfigs,
            GetStyleDefinitionsInfo: GetStyleDefinitionsInfo
        });
    }

    appControllers.service('VRCommon_StyleDefinitionAPIService', StyleDefinitionAPIService);

})(appControllers);