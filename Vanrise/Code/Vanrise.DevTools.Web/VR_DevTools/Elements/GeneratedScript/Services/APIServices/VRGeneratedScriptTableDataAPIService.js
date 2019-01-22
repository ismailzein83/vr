(function (appControllers) {
    "use strict";
    tableDataAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Devtools_ModuleConfig', 'SecurityService'];
    function tableDataAPIService(BaseAPIService, UtilsService, VR_Devtools_ModuleConfig, SecurityService) {

        var controller = "TableData";

        function GetFilteredTableData(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GetFilteredTableData"), input);
        }

        function GetSelectedTableData(query) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GetSelectedTableData"), query);
        }

        return {
            GetFilteredTableData: GetFilteredTableData,
            GetSelectedTableData: GetSelectedTableData
        };
    }
    appControllers.service("VR_Devtools_TableDataAPIService", tableDataAPIService);

})(appControllers);