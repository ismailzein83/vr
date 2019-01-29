(function (appControllers) {

    "use strict";
    whSJazzTransactionTypeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzTransactionTypeAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "TransactionType";

        function GetAllTransactionTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllTransactionTypes'), {
            });
        }
        function GetTransactionTypesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetTransactionTypesInfo'), {
                filter:filter
            });
        }
        return ({
            GetAllTransactionTypes: GetAllTransactionTypes,
            GetTransactionTypesInfo: GetTransactionTypesInfo
        });
    }

    appControllers.service("WhS_Jazz_TransactionTypeAPIService", whSJazzTransactionTypeAPIService);
})(appControllers);