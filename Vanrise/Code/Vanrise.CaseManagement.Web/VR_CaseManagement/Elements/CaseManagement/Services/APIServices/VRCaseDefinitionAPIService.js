(function (appControllers) {
    "use strict";

    vrCaseDefinitionAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_CaseManagement_ModuleConfig", "SecurityService"];

    function vrCaseDefinitionAPIService(BaseAPIService, UtilsService, VR_CaseManagement_ModuleConfig, SecurityService) {

        var controllerName = "VRCaseDefinition";

        function GetVRCaseGridDefinition(vrCaseDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_CaseManagement_ModuleConfig.moduleName, controllerName, "GetVRCaseGridDefinition"), { vrCaseDefinitionId: vrCaseDefinitionId });
        }

        return ({
            GetVRCaseGridDefinition: GetVRCaseGridDefinition,
        });
    }
    appControllers.service("VR_CaseManagement_VRCaseDefinitionAPIService", vrCaseDefinitionAPIService);
})(appControllers);