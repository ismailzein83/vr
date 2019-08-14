(function (appControllers) {
    "use strict";
    columnsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Devtools_ModuleConfig', 'SecurityService'];
    function columnsAPIService(BaseAPIService, UtilsService, VR_Devtools_ModuleConfig, SecurityService) {

        var controller = "DevProjectTemplate";


        function GetVRDevProjectsInfo(connectionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GetVRDevProjectsInfo"), { connectionId: connectionId });
        }
        function GetDevProjectTemplates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GetDevProjectTemplates"), input);
        }
        function GetDevProjectTableNames() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GetDevProjectTableNames"));
        }
        return {
            GetVRDevProjectsInfo: GetVRDevProjectsInfo,
            GetDevProjectTemplates: GetDevProjectTemplates,
            GetDevProjectTableNames: GetDevProjectTableNames
        };
    }
    appControllers.service("VR_Devtools_DevProjectTemplateAPIService", columnsAPIService);

})(appControllers); 