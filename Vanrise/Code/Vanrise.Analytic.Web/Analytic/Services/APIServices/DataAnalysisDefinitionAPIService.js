
(function (appControllers) {

    "use strict";

    DataAnalysisDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function DataAnalysisDefinitionAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = "DataAnalysisDefinition";


        function GetFilteredDataAnalysisDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetFilteredDataAnalysisDefinitions'), input);
        }

        function GetDataAnalysisDefinition(dataAnalysisDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetDataAnalysisDefinition'), {
                dataAnalysisDefinitionId: dataAnalysisDefinitionId
            });
        }

        function AddDataAnalysisDefinition(dataAnalysisDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'AddDataAnalysisDefinition'), dataAnalysisDefinitionItem);
        }

        function UpdateDataAnalysisDefinition(dataAnalysisDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'UpdateDataAnalysisDefinition'), dataAnalysisDefinitionItem);
        }

        function GetDataAnalysisDefinitionSettingsExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDataAnalysisDefinitionSettingsExtensionConfigs"));
        }

        function GetDataAnalysisDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetDataAnalysisDefinitionsInfo"), {
                filter: filter
            });
        }


        return ({
            GetFilteredDataAnalysisDefinitions: GetFilteredDataAnalysisDefinitions,
            GetDataAnalysisDefinition: GetDataAnalysisDefinition,
            AddDataAnalysisDefinition: AddDataAnalysisDefinition,
            UpdateDataAnalysisDefinition: UpdateDataAnalysisDefinition,
            GetDataAnalysisDefinitionSettingsExtensionConfigs: GetDataAnalysisDefinitionSettingsExtensionConfigs,
            GetDataAnalysisDefinitionsInfo: GetDataAnalysisDefinitionsInfo
        });
    }

    appControllers.service('VR_Analytic_DataAnalysisDefinitionAPIService', DataAnalysisDefinitionAPIService);
})(appControllers);