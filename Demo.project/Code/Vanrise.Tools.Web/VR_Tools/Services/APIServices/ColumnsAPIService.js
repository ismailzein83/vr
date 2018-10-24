(function (appControllers) {
    "use strict";
    columnsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Vanrise_Tools_ModuleConfig', 'SecurityService'];
    function columnsAPIService(BaseAPIService, UtilsService, Vanrise_Tools_ModuleConfig, SecurityService) {

        var controller = "Columns";

        function GetColumnsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Vanrise_Tools_ModuleConfig.moduleName, controller, "GetColumnsInfo"), { filter: filter });
        };


        return {
            GetColumnsInfo: GetColumnsInfo
        };
    };
    appControllers.service("Vanrise_Tools_ColumnsAPIService", columnsAPIService);

})(appControllers);