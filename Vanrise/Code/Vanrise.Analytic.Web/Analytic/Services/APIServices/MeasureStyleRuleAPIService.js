(function (appControllers) {

    "use strict";
    measureStyleRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function measureStyleRuleAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = 'MeasureStyleRuleController';

        function GetMeasureStyleRuleEditorRuntime(measureStuleRuleEditorRuntimeInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetMeasureStyleRuleEditorRuntime'), measureStuleRuleEditorRuntimeInput);
        }
        function GetMeasureStyleRuleDetail(measureStyleRuleInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetMeasureStyleRuleDetail'), measureStyleRuleInput);
        }

        return ({
            GetMeasureStyleRuleEditorRuntime: GetMeasureStyleRuleEditorRuntime,
            GetMeasureStyleRuleDetail: GetMeasureStyleRuleDetail
        });
    }

    appControllers.service('VR_Analytic_MeasureStyleRuleAPIService', measureStyleRuleAPIService);

})(appControllers);  