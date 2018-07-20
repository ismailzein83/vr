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

        function HasAddAnalyticReportPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Analytic_ModuleConfig.moduleName, controllerName, ['AddAnalyticReport']));
        }

        function UpdateAnalyticReport(analyticReport) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'UpdateAnalyticReport'), analyticReport);
        }

        function HasEditAnalyticReportPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Analytic_ModuleConfig.moduleName, controllerName, ['UpdateAnalyticReport']));
        }

        function GetAnalyticReportConfigTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetAnalyticReportConfigTypes'));
        }
        function CheckRecordStoragesAccess(analyticReportId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'CheckRecordStoragesAccess'), {
                analyticReportId: analyticReportId
            });
        }
        return ({
            GetAnalyticReportsInfo: GetAnalyticReportsInfo,
            GetFilteredAnalyticReports: GetFilteredAnalyticReports,
            GetAnalyticReportById: GetAnalyticReportById,
            UpdateAnalyticReport: UpdateAnalyticReport,
            HasEditAnalyticReportPermission:HasEditAnalyticReportPermission,
            AddAnalyticReport: AddAnalyticReport,
            HasAddAnalyticReportPermission:HasAddAnalyticReportPermission,
            GetAnalyticReportConfigTypes: GetAnalyticReportConfigTypes,
            CheckRecordStoragesAccess: CheckRecordStoragesAccess
        });
    }

    appControllers.service('VR_Analytic_AnalyticReportAPIService', AnalyticReportAPIService);

})(appControllers);