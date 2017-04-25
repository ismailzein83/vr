(function (appControllers) {

    "use strict";

    RingoReportSheetAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Ringo_ModuleConfig', 'SecurityService'];

    function RingoReportSheetAPIService(BaseAPIService, UtilsService, Retail_Ringo_ModuleConfig, SecurityService) {

        var controllerName = "RingoReportSheet";

        function DownloadMNPReport(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Ringo_ModuleConfig.moduleName, controllerName, 'DownloadMNPReport'), filter,
            {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        function DownloadTCRReport(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Ringo_ModuleConfig.moduleName, controllerName, 'DownloadTCRReport'), filter,
            {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });

        }
        return ({
            DownloadMNPReport: DownloadMNPReport,
            DownloadTCRReport: DownloadTCRReport
        });
    }

    appControllers.service('Retail_Ringo_RingoReportSheetAPIService', RingoReportSheetAPIService);

})(appControllers);