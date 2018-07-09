(function (appControllers) {
    "use strict";

    lookUpBusinessEntityDefinitionAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_GenericData_ModuleConfig", "SecurityService"];

    function lookUpBusinessEntityDefinitionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig, SecurityService) {

        var controllerName = "LKUPBusinessEntityDefinition";

        function GetLookUpBESelectorRuntimeInfo(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetLookUpBESelectorRuntimeInfo"), { businessEntityDefinitionId: businessEntityDefinitionId });
        }

        function GetLookUpBEExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetLookUpBEExtendedSettingsConfigs"));
        }

        return ({
            GetLookUpBESelectorRuntimeInfo: GetLookUpBESelectorRuntimeInfo,
            GetLookUpBEExtendedSettingsConfigs: GetLookUpBEExtendedSettingsConfigs
        });
    }
    appControllers.service("VR_GenericData_LKUPBEDefinitionAPIService", lookUpBusinessEntityDefinitionAPIService);
})(appControllers);