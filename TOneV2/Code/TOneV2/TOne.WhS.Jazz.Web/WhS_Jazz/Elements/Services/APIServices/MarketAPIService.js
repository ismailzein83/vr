(function (appControllers) {

    "use strict";
    whSJazzMarketAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzMarketAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "Market";

        function GetAllMarkets() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllMarkets'), {
            });
        }
        function GetMarketsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetMarketsInfo'), {
                filter:filter
            });
        }
        return ({
            GetAllMarkets: GetAllMarkets,
            GetMarketsInfo: GetMarketsInfo
        });
    }

    appControllers.service("WhS_Jazz_MarketAPIService", whSJazzMarketAPIService);
})(appControllers);