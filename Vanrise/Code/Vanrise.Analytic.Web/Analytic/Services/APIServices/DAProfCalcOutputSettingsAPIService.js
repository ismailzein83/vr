(function (appControllers) {

    "use strict";

    DAProfCalcOutputSettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function DAProfCalcOutputSettingsAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = "DAProfCalcOutputSettings";

        function GetInputFields(dataAnalysisDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetInputFields'), {
                dataAnalysisDefinitionId: dataAnalysisDefinitionId
            });
        }

        function GetFilteredOutputFields(dataAnalysisItemDefinitionId, filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetFilteredOutputFields'), {
                dataAnalysisItemDefinitionId: dataAnalysisItemDefinitionId,
                filter: filter
            });
        }

        return {
            GetInputFields: GetInputFields,
            GetFilteredOutputFields: GetFilteredOutputFields
        };
    }

    appControllers.service('VR_Analytic_DAProfCalcOutputSettingsAPIService', DAProfCalcOutputSettingsAPIService);
})(appControllers);