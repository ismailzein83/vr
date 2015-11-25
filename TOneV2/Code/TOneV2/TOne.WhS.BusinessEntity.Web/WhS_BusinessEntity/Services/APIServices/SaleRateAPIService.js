(function (appControllers) {

    "use strict";
    saleRateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function saleRateAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredSaleRate(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SaleRate", "GetFilteredSaleRates"), input);
        }
       
        return ({
            GetFilteredSaleRate: GetFilteredSaleRate
        });
    }

    appControllers.service('WhS_BE_SaleRateAPIService', saleRateAPIService);

})(appControllers);