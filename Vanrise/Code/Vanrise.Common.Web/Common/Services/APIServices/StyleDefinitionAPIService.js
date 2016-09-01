
(function (appControllers) {

    "use strict";
    StyleDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function StyleDefinitionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

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


        function HasAddStyleDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddStyleDefinition']));
        }

        function HasUpdateStyleDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateStyleDefinition']));
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
            HasAddStyleDefinitionPermission: HasAddStyleDefinitionPermission,
            HasUpdateStyleDefinitionPermission:HasUpdateStyleDefinitionPermission,
            UpdateStyleDefinition: UpdateStyleDefinition,
            GetStyleFormatingExtensionConfigs: GetStyleFormatingExtensionConfigs,
            GetStyleDefinitionsInfo: GetStyleDefinitionsInfo
        });
    }

    appControllers.service('VRCommon_StyleDefinitionAPIService', StyleDefinitionAPIService);

})(appControllers);