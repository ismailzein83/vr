
(function (appControllers) {

    "use strict";

    DAProfCalcOutputSettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function DAProfCalcOutputSettingsAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = "DAProfCalcOutputSettings";

        function GetOutputFields(dataAnalysisItemDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetOutputFields'), {
                dataAnalysisItemDefinitionId: dataAnalysisItemDefinitionId
            });
        }


        return ({
            GetOutputFields: GetOutputFields,
        });
    }

    appControllers.service('VR_Analytic_DAProfCalcOutputSettingsAPIService', DAProfCalcOutputSettingsAPIService);
})(appControllers);