(function (appControllers) {

    "use strict";
    RealTimeReportAPIService.$inject = ['BaseAPIService', 'VR_Analytic_ModuleConfig', 'UtilsService', 'SecurityService'];

    function RealTimeReportAPIService(BaseAPIService, VR_Analytic_ModuleConfig, UtilsService, SecurityService) {
        var controllerName = 'RealTimeReport';

        function GetRealTimeReportsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetRealTimeReportsInfo"),
                {
                    filter: filter
                });
        }
        function GetRealTimeReportById(realTimeReportId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetRealTimeReportById"),
               {
                   realTimeReportId: realTimeReportId
               });
        }
       
        function AddRealTimeReport(realTimeReport) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'AddRealTimeReport'), realTimeReport);
        }

        function UpdateRealTimeReport(realTimeReport) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'UpdateRealTimeReport'), realTimeReport);
        }
        return ({
            GetRealTimeReportsInfo: GetRealTimeReportsInfo,
            GetRealTimeReportById: GetRealTimeReportById,
            UpdateRealTimeReport: UpdateRealTimeReport,
            AddRealTimeReport: AddRealTimeReport
        });
    }

    appControllers.service('VR_Analytic_RealTimeReportAPIService', RealTimeReportAPIService);

})(appControllers);