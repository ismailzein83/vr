(function (appControllers) {
    "use strict";
    tableAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Devtools_ModuleConfig', 'SecurityService'];
    function tableAPIService(BaseAPIService, UtilsService, VR_Devtools_ModuleConfig, SecurityService) {

        var controller = "Table";

        function GetTablesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GetTablesInfo"), { filter: filter });
        }


        return {
            GetTablesInfo: GetTablesInfo
        };
    }
    appControllers.service("VR_Devtools_TableAPIService", tableAPIService);

})(appControllers);