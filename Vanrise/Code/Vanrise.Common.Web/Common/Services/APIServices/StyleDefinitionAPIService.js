
(function (appControllers) {

    "use strict";
    StyleDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function StyleDefinitionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "StyleDefinition";

        //function GetStyleDefinitionSettingsTemplateConfigs() {
        //    return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetStyleDefinitionSettingsTemplateConfigs"));
        //}

        function GetFilteredStyleDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredStyleDefinitions'), input);
        }

        function GetStyleDefinition(StyleDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetStyleDefinition'), {
                StyleDefinitionId: StyleDefinitionId
            });
        }

        //function GetStyleDefinitionsInfo(filter) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetStyleDefinitionsInfo"), {
        //        filter: filter
        //    });
        //}

        function AddStyleDefinition(StyleDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddStyleDefinition'), StyleDefinitionItem);
        }

        function UpdateStyleDefinition(StyleDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateStyleDefinition'), StyleDefinitionItem);
        }

        return ({
            //GetStyleDefinitionSettingsTemplateConfigs: GetStyleDefinitionSettingsTemplateConfigs,
            GetFilteredStyleDefinitions: GetFilteredStyleDefinitions,
            GetStyleDefinition: GetStyleDefinition,
            //GetStyleDefinitionsInfo: GetStyleDefinitionsInfo,
            AddStyleDefinition: AddStyleDefinition,
            UpdateStyleDefinition: UpdateStyleDefinition,
        });
    }

    appControllers.service('VRCommon_StyleDefinitionAPIService', StyleDefinitionAPIService);

})(appControllers);