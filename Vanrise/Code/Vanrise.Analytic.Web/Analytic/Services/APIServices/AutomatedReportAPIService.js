(function (appControllers) {

    "use strict";
    automatedReportAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function automatedReportAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = 'AutomatedReport';

        function GetAutomatedReportSettings() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetAutomatedReportSettings'));
        }

        return ({
            GetAutomatedReportSettings: GetAutomatedReportSettings,
        });
    }

    appControllers.service('VR_Analytic_AutomatedReportAPIService', automatedReportAPIService);

})(appControllers);