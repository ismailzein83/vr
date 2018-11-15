(function (appControllers) {

    "use strict";
    kpiSettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function kpiSettingsAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = 'KPISettingsController';

        function GetAnalytictableKPISettings(analyticTableId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetAnalytictableKPISettings'), { analyticTableId: analyticTableId});
        }

        return ({
            GetAnalytictableKPISettings: GetAnalytictableKPISettings,
        });
    }

    appControllers.service('VR_Analytic_KPISettingsAPIService', kpiSettingsAPIService);

})(appControllers);