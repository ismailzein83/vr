﻿(function (appControllers) {

    'use strict';

    DataTransformationDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataTransformationDefinitionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return ({
            GetDataTransformationDefinition: GetDataTransformationDefinition,
            GetFilteredDataTransformationDefinitions: GetFilteredDataTransformationDefinitions,
            AddDataTransformationDefinition: AddDataTransformationDefinition,
            UpdateDataTransformationDefinition: UpdateDataTransformationDefinition,
            GetDataTransformationDefinitions: GetDataTransformationDefinitions,
            GetDataTransformationDefinitionRecords:GetDataTransformationDefinitionRecords
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
        function UpdateDataTransformationDefinition(dataTransformationDefinitionObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'DataTransformationDefinition', 'UpdateDataTransformationDefinition'), dataTransformationDefinitionObject);
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
        
    }

    appControllers.service('VR_GenericData_DataTransformationDefinitionAPIService', DataTransformationDefinitionAPIService);

})(appControllers);
