(function (appControllers) {

    'use strict';

    GenericUIRuntimeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericUIRuntimeAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetExtensibleBEItemRuntime: GetExtensibleBEItemRuntime,
            GetGenericManagementRuntime: GetGenericManagementRuntime,
            GetGenericEditorRuntime: GetGenericEditorRuntime,
            GetDataRecordTypesInfo: GetDataRecordTypesInfo,
            GetGenericEditorRuntimeSections: GetGenericEditorRuntimeSections,
            GetGenericEditorRuntimeRows: GetGenericEditorRuntimeRows
        };


        function GetGenericEditorRuntimeSections(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericUIRuntime', 'GetGenericEditorRuntimeSections'), input);
        }

        function GetExtensibleBEItemRuntime(dataRecordTypeId, businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericUIRuntime', 'GetExtensibleBEItemRuntime'), {
                dataRecordTypeId: dataRecordTypeId,
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }
        function GetGenericManagementRuntime(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericUIRuntime', 'GetGenericManagementRuntime'), {
                businessEntityDefinitionId: businessEntityDefinitionId,
            });
        }
        function GetGenericEditorRuntime(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericUIRuntime', 'GetGenericEditorRuntime'), {
                businessEntityDefinitionId: businessEntityDefinitionId,
            });
        }
        function GetDataRecordTypesInfo(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericUIRuntime', 'GetDataRecordTypesInfo'), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }
        function GetGenericEditorRuntimeRows(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericUIRuntime', 'GetGenericEditorRuntimeRows'), input);
        }
    }

    appControllers.service('VR_GenericData_GenericUIRuntimeAPIService', GenericUIRuntimeAPIService);

})(appControllers);