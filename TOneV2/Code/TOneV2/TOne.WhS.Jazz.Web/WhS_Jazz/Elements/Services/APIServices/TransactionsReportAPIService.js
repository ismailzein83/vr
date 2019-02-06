(function (appControllers) {

    "use strict";
    whSJazzTransactionsReportAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzTransactionsReportAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "TransactionsReport";

        function DownloadTransactionsReports(processInstanceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'DownloadTransactionsReports'), {
                processInstanceId: processInstanceId
            }, {
                    returnAllResponseParameters: true,
                    responseTypeAsBufferArray: true
                });
        }
        return ({
            DownloadTransactionsReports: DownloadTransactionsReports
        });
    }

    appControllers.service("WhS_Jazz_TransactionsReportAPIService", whSJazzTransactionsReportAPIService);
})(appControllers);