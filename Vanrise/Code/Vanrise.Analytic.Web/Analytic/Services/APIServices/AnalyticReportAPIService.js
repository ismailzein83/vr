(function (appControllers) {

    "use strict";
    AnalyticReportAPIService.$inject = ['BaseAPIService', 'VR_Analytic_ModuleConfig', 'UtilsService', 'SecurityService'];

    function AnalyticReportAPIService(BaseAPIService, VR_Analytic_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'AnalyticReport';

        function GetAnalyticReportsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetAnalyticReportsInfo"),
                {
                    filter: filter
                });
        }
        function GetAnalyticReportById(analyticReportId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetAnalyticReportById"),
               {
                   analyticReportId: analyticReportId
               });
        }
        function GetFilteredAnalyticReports(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetFilteredAnalyticReports"), input);
        }

        function AddAnalyticReport(analyticReport) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'AddAnalyticReport'), analyticReport);
        }

        function UpdateAnalyticReport(analyticReport) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'UpdateAnalyticReport'), analyticReport);
        }
        function GetAnalyticReportConfigTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetAnalyticReportConfigTypes'));
        }
        return ({
            GetAnalyticReportsInfo: GetAnalyticReportsInfo,
            GetFilteredAnalyticReports: GetFilteredAnalyticReports,
            GetAnalyticReportById: GetAnalyticReportById,
            UpdateAnalyticReport: UpdateAnalyticReport,
            AddAnalyticReport: AddAnalyticReport,
            GetAnalyticReportConfigTypes: GetAnalyticReportConfigTypes
        });
    }

    appControllers.service('VR_Analytic_AnalyticReportAPIService', AnalyticReportAPIService);

})(appControllers);