
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

        //function GetDataAnalysisItemDefinitionsInfo(filter) {
        //    return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDataAnalysisItemDefinitionsInfo"), {
        //        filter: filter
        //    });
        //}


        return ({
            GetFilteredDataAnalysisItemDefinitions: GetFilteredDataAnalysisItemDefinitions,
            GetDataAnalysisItemDefinition: GetDataAnalysisItemDefinition,
            AddDataAnalysisItemDefinition: AddDataAnalysisItemDefinition,
            UpdateDataAnalysisItemDefinition: UpdateDataAnalysisItemDefinition,
            GetDARecordAggregateExtensionConfigs: GetDARecordAggregateExtensionConfigs,
            //GetDataAnalysisItemDefinitionsInfo: GetDataAnalysisItemDefinitionsInfo
        });
    }

    appControllers.service('VR_Analytic_DataAnalysisItemDefinitionAPIService', DataAnalysisItemDefinitionAPIService);
})(appControllers);