(function (appControllers) {

    "use strict";
    ReportGenerationCustomCodeSettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function ReportGenerationCustomCodeSettingsAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = 'ReportGenerationCustomCodeSettings';

        function GetReportGenerationCustomCodeSettingsInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetReportGenerationCustomCodeSettingsInfo'));
        }

        return ({
            GetReportGenerationCustomCodeSettingsInfo: GetReportGenerationCustomCodeSettingsInfo,
        });
    }

    appControllers.service('VR_Analytic_ReportGenerationCustomCodeSettingsAPIService', ReportGenerationCustomCodeSettingsAPIService);

})(appControllers);