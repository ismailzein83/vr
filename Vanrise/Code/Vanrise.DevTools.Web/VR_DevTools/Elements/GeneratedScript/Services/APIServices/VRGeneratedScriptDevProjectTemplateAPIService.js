(function (appControllers) {
    "use strict";
    columnsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Devtools_ModuleConfig', 'SecurityService'];
    function columnsAPIService(BaseAPIService, UtilsService, VR_Devtools_ModuleConfig, SecurityService) {

        var controller = "DevProjectTemplate";

      
        function GetVRDevProjectsInfo(connectionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GetVRDevProjectsInfo"), { connectionId: connectionId });
        }
      
        return {
            GetVRDevProjectsInfo: GetVRDevProjectsInfo
        };
    }
    appControllers.service("VR_Devtools_DevProjectTemplateAPIService", columnsAPIService);

})(appControllers);