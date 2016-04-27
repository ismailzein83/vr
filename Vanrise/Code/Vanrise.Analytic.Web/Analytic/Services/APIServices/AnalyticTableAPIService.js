(function (appControllers) {

    "use strict";
    AnalyticTableAPIService.$inject = ['BaseAPIService', 'VR_Analytic_ModuleConfig', 'UtilsService', 'SecurityService'];

    function AnalyticTableAPIService(BaseAPIService, VR_Analytic_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'AnalyticTable';

        function GetAnalyticTablesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetAnalyticTablesInfo"),
                {
                    filter: filter
                });
        }
        function GetTableById(tableId)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetTableById"),
               {
                   tableId: tableId
               });
        }
        function GetFilteredAnalyticTables(input)
        {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetFilteredAnalyticTables"), input);
        }

        function AddAnalyticTable(tableObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'AddAnalyticTable'), tableObj);
        }

        function UpdateAnalyticTable(tableObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'UpdateAnalyticTable'), tableObj);
        }
        return ({
            GetAnalyticTablesInfo: GetAnalyticTablesInfo,
            GetFilteredAnalyticTables: GetFilteredAnalyticTables,
            GetTableById: GetTableById,
            UpdateAnalyticTable: UpdateAnalyticTable,
            AddAnalyticTable: AddAnalyticTable
        });
    }

    appControllers.service('VR_Analytic_AnalyticTableAPIService', AnalyticTableAPIService);

})(appControllers);