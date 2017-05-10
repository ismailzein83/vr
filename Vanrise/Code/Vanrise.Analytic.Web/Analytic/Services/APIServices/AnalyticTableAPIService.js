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

        function GetRemoteAnalyticTablesInfo(connectionId,filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetRemoteAnalyticTablesInfo"),
                {
                    connectionId:connectionId,
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
        function HasAddAnalyticTablePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Analytic_ModuleConfig.moduleName, controllerName, ['AddAnalyticTable']));
        }

        
        function UpdateAnalyticTable(tableObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'UpdateAnalyticTable'), tableObj);
        }
        function HasEditAnalyticTablePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Analytic_ModuleConfig.moduleName, controllerName, ['UpdateAnalyticTable']));
        }
        return ({
            GetAnalyticTablesInfo: GetAnalyticTablesInfo,
            GetFilteredAnalyticTables: GetFilteredAnalyticTables,
            GetTableById: GetTableById,         
            AddAnalyticTable: AddAnalyticTable,
            HasAddAnalyticTablePermission: HasAddAnalyticTablePermission,
            UpdateAnalyticTable: UpdateAnalyticTable,
            HasEditAnalyticTablePermission: HasEditAnalyticTablePermission,
            GetRemoteAnalyticTablesInfo: GetRemoteAnalyticTablesInfo
        });
    }

    appControllers.service('VR_Analytic_AnalyticTableAPIService', AnalyticTableAPIService);

})(appControllers);