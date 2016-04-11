(function (appControllers) {

    "use strict";
    saleRateAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function saleRateAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "SaleRate";

        function GetFilteredSaleRate(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSaleRates"), input);
        }

        return ({
            GetFilteredSaleRate: GetFilteredSaleRate
        });
    }

    appControllers.service("WhS_BE_SaleRateAPIService", saleRateAPIService);

})(appControllers);