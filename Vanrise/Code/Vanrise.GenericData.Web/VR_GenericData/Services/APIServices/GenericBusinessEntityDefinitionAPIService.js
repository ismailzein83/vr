(function (appControllers) {
    "use strict";

    genericBusinessEntityDefinitionAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_GenericData_ModuleConfig", "SecurityService"];

    function genericBusinessEntityDefinitionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig, SecurityService) {

        var controllerName = "GenericBusinessEntityDefinition";

        function GetGenericBEDefinitionSettings(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBEDefinitionSettings"), { businessEntityDefinitionId: businessEntityDefinitionId });
        }

        function GetGenericBEGridDefinition(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBusinessEntityGridDefinition"), { businessEntityDefinitionId: businessEntityDefinitionId });
        }
        function GetGenericBEGridColumnAttributes(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBEGridColumnAttributes"), { businessEntityDefinitionId: businessEntityDefinitionId });
        }
        function GetIdFieldTypeForGenericBE(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetIdFieldTypeForGenericBE"), { businessEntityDefinitionId: businessEntityDefinitionId });
        }
        function GetGenericBEViewDefinitionSettingsConfigs(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBEViewDefinitionSettingsConfigs"));
        }
        function GetGenericBEEditorDefinitionSettingsConfigs(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBEEditorDefinitionSettingsConfigs"));
        }

        return ({
            GetGenericBEDefinitionSettings: GetGenericBEDefinitionSettings,
            GetGenericBEGridDefinition: GetGenericBEGridDefinition,
            GetGenericBEGridColumnAttributes: GetGenericBEGridColumnAttributes,
            GetIdFieldTypeForGenericBE: GetIdFieldTypeForGenericBE,
            GetGenericBEViewDefinitionSettingsConfigs: GetGenericBEViewDefinitionSettingsConfigs,
            GetGenericBEEditorDefinitionSettingsConfigs: GetGenericBEEditorDefinitionSettingsConfigs
        });
    }
    appControllers.service("VR_GenericData_GenericBEDefinitionAPIService", genericBusinessEntityDefinitionAPIService);
})(appControllers);