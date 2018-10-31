(function (appControllers) {
    "use strict";
    tableAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Tools_ModuleConfig', 'SecurityService'];
    function tableAPIService(BaseAPIService, UtilsService, VR_Tools_ModuleConfig, SecurityService) {

        var controller = "Table";

        function GetTablesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Tools_ModuleConfig.moduleName, controller, "GetTablesInfo"), { filter: filter });
        };


        return {
            GetTablesInfo: GetTablesInfo
        };
    };
    appControllers.service("VR_Tools_TableAPIService", tableAPIService);

})(appControllers);