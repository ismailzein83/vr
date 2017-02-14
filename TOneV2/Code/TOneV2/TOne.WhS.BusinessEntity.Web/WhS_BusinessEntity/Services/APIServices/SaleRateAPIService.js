(function (appControllers) {

    "use strict";
    saleRateAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function saleRateAPIService(BaseAPIService, UtilsService, whSBeModuleConfig) {

        var controllerName = "SaleRate";
        function GetPrimarySaleEntity() {
            return BaseAPIService.get(UtilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetPrimarySaleEntity"));
        }
        function GetFilteredSaleRate(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetFilteredSaleRates"), input);
        }

        return ({
            GetFilteredSaleRate: GetFilteredSaleRate,
            GetPrimarySaleEntity: GetPrimarySaleEntity
        });
    }

    appControllers.service("WhS_BE_SaleRateAPIService", saleRateAPIService);

})(appControllers);