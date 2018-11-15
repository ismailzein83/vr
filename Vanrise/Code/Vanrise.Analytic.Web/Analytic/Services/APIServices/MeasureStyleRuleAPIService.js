(function (appControllers) {

    "use strict";
    measureStyleRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function measureStyleRuleAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = 'MeasureStyleRuleController';

        function GetAnalytictableKPISettings(analyticTableId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetAnalytictableKPISettings'), { analyticTableId: analyticTableId });
        }

        return ({
            GetAnalytictableKPISettings: GetAnalytictableKPISettings,
        });
    }

    appControllers.service('VR_Analytic_MeasureStyleRuleAPIService', measureStyleRuleAPIService);

})(appControllers); 