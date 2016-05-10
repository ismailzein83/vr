(function (appControllers) {

    "use strict";

    ReportDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Analytics_ModuleConfig'];

    function ReportDefinitionAPIService(BaseAPIService, UtilsService, WhS_Analytics_ModuleConfig) {

        var controllerName = "ReportDefinition";

        return {
            GetAllReportDefinition: GetAllReportDefinition
        };

        function GetAllReportDefinition() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Analytics_ModuleConfig.moduleName, controllerName, "GetAllRDLCReportDefinition"));
        }
    }

    appControllers.service('WhS_Analytics_ReportDefinitionAPIService', ReportDefinitionAPIService);

})(appControllers);