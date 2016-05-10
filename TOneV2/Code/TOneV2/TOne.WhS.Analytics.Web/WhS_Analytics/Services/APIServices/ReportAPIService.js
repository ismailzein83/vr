(function (appControllers) {

    'use strict';

    ReportAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Analytics_ModuleConfig'];

    function ReportAPIService(BaseAPIService) {


        return {
            GetAllReportDefinition: GetAllReportDefinition
        };


        function GetAllReportDefinition() {
            return BaseAPIService.get("api/ReportDefintion/GetAllRDLCReportDefinition");
        }
    }

    appControllers.service('WhS_Analytics_ReportAPIService', ReportAPIService);

})(appControllers);