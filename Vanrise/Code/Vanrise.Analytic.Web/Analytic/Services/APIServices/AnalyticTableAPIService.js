﻿(function (appControllers) {

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
        function SaveAnalyticTableMeasureStyles(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'SaveAnalyticTableMeasureStyles'), input);
        }
        function HasEditAnalyticTablePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Analytic_ModuleConfig.moduleName, controllerName, ['UpdateAnalyticTable']));
        }
        function GetAnalyticTableConnectionId(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetAnalyticTableConnectionId"), input);
        }
        function GetClientAnalyitTableInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetClientAnalyitTableInfo"));
        }
        function GetAnalyticTableMergedMeasureStylesEditorRuntime(analyticTableId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetAnalyticTableMergedMeasureStylesEditorRuntime"), { analyticTableId: analyticTableId});
        }
       
        return ({
            GetAnalyticTablesInfo: GetAnalyticTablesInfo,
            GetFilteredAnalyticTables: GetFilteredAnalyticTables,
            GetTableById: GetTableById,         
            AddAnalyticTable: AddAnalyticTable,
            HasAddAnalyticTablePermission: HasAddAnalyticTablePermission,
            UpdateAnalyticTable: UpdateAnalyticTable,
            HasEditAnalyticTablePermission: HasEditAnalyticTablePermission,
            GetRemoteAnalyticTablesInfo: GetRemoteAnalyticTablesInfo,
            GetAnalyticTableConnectionId: GetAnalyticTableConnectionId,
            GetClientAnalyitTableInfo: GetClientAnalyitTableInfo,
            SaveAnalyticTableMeasureStyles: SaveAnalyticTableMeasureStyles,
            GetAnalyticTableMergedMeasureStylesEditorRuntime: GetAnalyticTableMergedMeasureStylesEditorRuntime
        });
    }

    appControllers.service('VR_Analytic_AnalyticTableAPIService', AnalyticTableAPIService);

})(appControllers);