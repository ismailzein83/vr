(function (appControllers) {

    "use strict";
    saleRateAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function saleRateAPIService(BaseAPIService, UtilsService, whSBeModuleConfig) {

        var controllerName = "SaleRate";
        function GetSaleAreaSettingsData() {
            return BaseAPIService.get(UtilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetSaleAreaSettingsData"));
        }
        function GetFilteredSaleRate(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetFilteredSaleRates"), input);
        }

        return ({
            GetFilteredSaleRate: GetFilteredSaleRate,
            GetSaleAreaSettingsData: GetSaleAreaSettingsData
        });
    }

    appControllers.service("WhS_BE_SaleRateAPIService", saleRateAPIService);

})(appControllers);