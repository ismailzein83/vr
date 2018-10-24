(function (appControllers) {
    "use strict";
    tableAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Vanrise_Tools_ModuleConfig', 'SecurityService'];
    function tableAPIService(BaseAPIService, UtilsService, Vanrise_Tools_ModuleConfig, SecurityService) {

        var controller = "Table";

        function GetTablesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Vanrise_Tools_ModuleConfig.moduleName, controller, "GetTablesInfo"), { filter: filter });
        };


        return {
            GetTablesInfo: GetTablesInfo
        };
    };
    appControllers.service("Vanrise_Tools_TableAPIService", tableAPIService);

})(appControllers);