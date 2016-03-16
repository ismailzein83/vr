(function (appControllers) {

    'use strict';

    DataTransformationDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function DataTransformationDefinitionAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {
        return ({
            GetDataTransformationDefinition: GetDataTransformationDefinition,
            GetFilteredDataTransformationDefinitions: GetFilteredDataTransformationDefinitions,
            AddDataTransformationDefinition: AddDataTransformationDefinition,
            HasAddDataTransformationDefinition:HasAddDataTransformationDefinition,
            UpdateDataTransformationDefinition: UpdateDataTransformationDefinition,
            HasUpdateDataTransformationDefinition: HasUpdateDataTransformationDefinition,
            GetDataTransformationDefinitions: GetDataTransformationDefinitions,
            GetDataTransformationDefinitionRecords: GetDataTransformationDefinitionRecords,
            TryCompileSteps: TryCompileSteps,
            HasTryCompileSteps:HasTryCompileSteps,
            ExportCompilationResult: ExportCompilationResult
        });
        function GetFilteredDataTransformationDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataTransformationDefinition', 'GetFilteredDataTransformationDefinitions'), input);
        }

        function GetDataTransformationDefinition(dataTransformationDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataTransformationDefinition', 'GetDataTransformationDefinition'), { dataTransformationDefinitionId: dataTransformationDefinitionId });
        }
        function AddDataTransformationDefinition(dataTransformationDefinitionObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataTransformationDefinition', 'AddDataTransformationDefinition'), dataTransformationDefinitionObject);
        }
        function HasAddDataTransformationDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "DataTransformationDefinition", ['AddDataTransformationDefinition']));
        }
        function UpdateDataTransformationDefinition(dataTransformationDefinitionObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataTransformationDefinition', 'UpdateDataTransformationDefinition'), dataTransformationDefinitionObject);
        }
        function HasUpdateDataTransformationDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "DataTransformationDefinition", ['UpdateDataTransformationDefinition']));
        }

        function GetDataTransformationDefinitions(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataTransformationDefinition', 'GetDataTransformationDefinitions'), {
                filter: filter
            });
        }

        function GetDataTransformationDefinitionRecords(dataTransformationDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataTransformationDefinition', 'GetDataTransformationDefinitionRecords'), {
                dataTransformationDefinitionId: dataTransformationDefinitionId
            });
        }
        function TryCompileSteps(dataTransformationDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataTransformationDefinition', 'TryCompileSteps'), dataTransformationDefinition);
        }
        function HasTryCompileSteps() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "DataTransformationDefinition", ['TryCompileSteps']));
        }

        function ExportCompilationResult(dataTransformationDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataTransformationDefinition', 'ExportCompilationResult'),dataTransformationDefinition, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
    }

    appControllers.service('VR_GenericData_DataTransformationDefinitionAPIService', DataTransformationDefinitionAPIService);

})(appControllers);
