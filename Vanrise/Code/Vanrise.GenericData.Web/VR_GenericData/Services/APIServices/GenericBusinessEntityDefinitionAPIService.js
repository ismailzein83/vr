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
        function GetGenericBEViewDefinitionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBEViewDefinitionSettingsConfigs"));
        }
        function GetGenericBEEditorDefinitionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBEEditorDefinitionSettingsConfigs"));
        }
        function GetGenericBEFilterDefinitionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBEFilterDefinitionSettingsConfigs"));
        }
        function GetGenericBEActionDefinitionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBEActionDefinitionSettingsConfigs"));
        }
        function GetGenericBEExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBEExtendedSettingsConfigs"));
        }
        function GetGenericBEOnAfterSaveHandlerSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBEOnAfterSaveHandlerSettingsConfigs"));
        }
        function GetGenericBEOnBeforeInsertHandlerSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBEOnBeforeInsertHandlerSettingsConfigs"));
        }
        function GetGenericBESaveConditionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBESaveConditionSettingsConfigs"));
        }
        function GetGenericBEConditionSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBEConditionSettingsConfigs"));
        }
        function GetGenericBESerialNumberPartSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetGenericBESerialNumberPartSettingsConfigs"));
        }
        return ({
            GetGenericBEDefinitionSettings: GetGenericBEDefinitionSettings,
            GetGenericBEGridDefinition: GetGenericBEGridDefinition,
            GetGenericBEGridColumnAttributes: GetGenericBEGridColumnAttributes,
            GetIdFieldTypeForGenericBE: GetIdFieldTypeForGenericBE,
            GetGenericBEViewDefinitionSettingsConfigs: GetGenericBEViewDefinitionSettingsConfigs,
            GetGenericBEEditorDefinitionSettingsConfigs: GetGenericBEEditorDefinitionSettingsConfigs,
            GetGenericBEFilterDefinitionSettingsConfigs: GetGenericBEFilterDefinitionSettingsConfigs,
            GetGenericBEActionDefinitionSettingsConfigs: GetGenericBEActionDefinitionSettingsConfigs,
            GetGenericBEExtendedSettingsConfigs: GetGenericBEExtendedSettingsConfigs,
            GetGenericBEOnAfterSaveHandlerSettingsConfigs: GetGenericBEOnAfterSaveHandlerSettingsConfigs,
            GetGenericBEOnBeforeInsertHandlerSettingsConfigs: GetGenericBEOnBeforeInsertHandlerSettingsConfigs,
            GetGenericBESaveConditionSettingsConfigs: GetGenericBESaveConditionSettingsConfigs,
            GetGenericBEConditionSettingsConfigs: GetGenericBEConditionSettingsConfigs,
            GetGenericBESerialNumberPartSettingsConfigs: GetGenericBESerialNumberPartSettingsConfigs
        });
    }
    appControllers.service("VR_GenericData_GenericBEDefinitionAPIService", genericBusinessEntityDefinitionAPIService);
})(appControllers);