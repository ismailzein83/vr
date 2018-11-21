(function (appControllers) {

    "use strict";
    ReportGenerationCustomCodeSettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function ReportGenerationCustomCodeSettingsAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = 'ReportGenerationCustomCode';

        function GetReportGenerationCustomCodeSettingsInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetReportGenerationCustomCodeSettingsInfo'));
        }
        function TryCompileCustomCode(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'TryCompileCustomCode'), input);
        }
        return ({
            GetReportGenerationCustomCodeSettingsInfo: GetReportGenerationCustomCodeSettingsInfo,
            TryCompileCustomCode: TryCompileCustomCode
        });
    }

    appControllers.service('VR_Analytic_ReportGenerationCustomCodeAPIService', ReportGenerationCustomCodeSettingsAPIService);

})(appControllers);