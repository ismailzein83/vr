(function (appControllers) {

    'use strict';

    SalePricelistAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function SalePricelistAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = 'OtherSaleRate';

        function GetOtherSaleRates(query) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetOtherSaleRates'), query);
        }

        return {
            GetOtherSaleRates: GetOtherSaleRates
        };
    }

    appControllers.service('WhS_BE_OtherSaleRateAPIService', SalePricelistAPIService);

})(appControllers);