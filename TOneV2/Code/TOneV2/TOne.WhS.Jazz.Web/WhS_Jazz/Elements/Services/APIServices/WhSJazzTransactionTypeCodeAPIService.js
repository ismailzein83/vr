(function (appControllers) {

    "use strict";
    whSJazzTransactionTypeCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzTransactionTypeCodeAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "WhSJazzTransactionTypeCode";

        function GetAllTransactionTypeCodes() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllTransactionTypeCodes'), {
            });
        }
        function GetTransactionTypeCodesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetTransactionTypeCodesInfo'), {
                filter:filter
            });
        }
        return ({
            GetAllTransactionTypeCodes: GetAllTransactionTypeCodes,
            GetTransactionTypeCodesInfo: GetTransactionTypeCodesInfo
        });
    }

    appControllers.service("WhS_Jazz_TransactionTypeCodeAPIService", whSJazzTransactionTypeCodeAPIService);
})(appControllers);