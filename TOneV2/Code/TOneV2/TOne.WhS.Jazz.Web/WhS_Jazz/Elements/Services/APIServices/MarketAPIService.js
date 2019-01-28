(function (appControllers) {

    "use strict";
    whSJazzMarketCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzMarketCodeAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "Market";

        function GetAllMarketCodes() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllMarketCodes'), {
            });
        }
        function GetMarketCodesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetMarketCodesInfo'), {
                filter:filter
            });
        }
        return ({
            GetAllMarketCodes: GetAllMarketCodes,
            GetMarketCodesInfo: GetMarketCodesInfo
        });
    }

    appControllers.service("WhS_Jazz_MarketCodeAPIService", whSJazzMarketCodeAPIService);
})(appControllers);