﻿
(function (appControllers) {

    "use strict";

    DataAnalysisItemDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function DataAnalysisItemDefinitionAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = "DataAnalysisItemDefinition";


        function GetFilteredDataAnalysisItemDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetFilteredDataAnalysisItemDefinitions'), input);
        }

        function GetDataAnalysisItemDefinition(dataAnalysisItemDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetDataAnalysisItemDefinition'), {
                DataAnalysisItemDefinitionId: dataAnalysisItemDefinitionId
            });
        }

        function AddDataAnalysisItemDefinition(dataAnalysisItemDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'AddDataAnalysisItemDefinition'), dataAnalysisItemDefinitionItem);
        }

        function UpdateDataAnalysisItemDefinition(dataAnalysisItemDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'UpdateDataAnalysisItemDefinition'), dataAnalysisItemDefinitionItem);
        }

        function GetDARecordAggregateExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDARecordAggregateExtensionConfigs"));
        }

        function GetTimeRangeFilterExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetTimeRangeFilterExtensionConfigs"));
        }

        function GetDataAnalysisItemDefinitionsInfo(filter, dataAnalysisDefinisitonId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDataAnalysisItemDefinitionsInfo"), {
                filter: filter,
                dataAnalysisDefinisitonId: dataAnalysisDefinisitonId
            });
        }

        function GetDataAnalysisItemDefinitionsHavingParameters(parametersType) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDataAnalysisItemDefinitionsHavingParameters"), { type: parametersType } );
        }

        return ({
            GetFilteredDataAnalysisItemDefinitions: GetFilteredDataAnalysisItemDefinitions,
            GetDataAnalysisItemDefinition: GetDataAnalysisItemDefinition,
            AddDataAnalysisItemDefinition: AddDataAnalysisItemDefinition,
            UpdateDataAnalysisItemDefinition: UpdateDataAnalysisItemDefinition,
            GetDARecordAggregateExtensionConfigs: GetDARecordAggregateExtensionConfigs,
            GetTimeRangeFilterExtensionConfigs: GetTimeRangeFilterExtensionConfigs,
            GetDataAnalysisItemDefinitionsInfo: GetDataAnalysisItemDefinitionsInfo,
            GetDataAnalysisItemDefinitionsHavingParameters: GetDataAnalysisItemDefinitionsHavingParameters
        });
    }

    appControllers.service('VR_Analytic_DataAnalysisItemDefinitionAPIService', DataAnalysisItemDefinitionAPIService);
})(appControllers);